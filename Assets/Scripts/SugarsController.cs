using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// TODO 合間を見て整理
/// </summary>
public class SugersController : MonoBehaviour
{
    [SerializeField]
    private SugarUnit sugerUnitPrefab;

    private SugarFactory sugarFactory;

    private ReactiveCollection<SugarUnit> sugarUnits;

    private int newestChainId = 0;

    private int GetLandablePositionIdx(int xIdx)
    {
        var yIdx = sugarUnits
            ?.Where(item => item.Entity.positionIdx?.Value.x == xIdx)
            ?.Max(item => item.Entity.positionIdx?.Value.y);
        return (yIdx ?? -1) + 1;
    }

    private void FireChainSugar(SugarUnit unit)
    {
        for(int i = sugarUnits.Count() - 1; i >= 0; i--)
        {
            var item = sugarUnits[i];
            if(item.Entity.IsFreeze &&
                item.Entity.ChainId != unit.Entity.ChainId &&
                item.Entity.IsNeighbor(unit.Entity.positionIdx.Value) &&
                item.Entity.state.Value == unit.Entity.state.Value)
            {
                item.Entity.IsWaitCombo.Value = true;
                item.Entity.WaitComboGaugeNum.Value = Const.SUGAR_GAUGE_DURATION;
                item.Entity.ChainId = unit.Entity.ChainId;
                FireChainSugar(item);
            }
        }
    }

    private void CheckAndFireSugar(SugarUnit unit)
    {
        for(int i = sugarUnits.Count() - 1; i >= 0; i--)
        {
            var item = sugarUnits[i];
            if(item.Entity.IsFreeze &&
                item.Entity.IsNeighbor(unit.Entity.positionIdx.Value) &&
                item.Entity.state.Value == unit.Entity.state.Value)
            {
                item.Entity.IsWaitCombo.Value = true;
                item.Entity.WaitComboGaugeNum.Value = Const.SUGAR_GAUGE_DURATION;
                item.Entity.ChainId = newestChainId;
                unit.Entity.IsWaitCombo.Value = true;
                unit.Entity.WaitComboGaugeNum.Value = Const.SUGAR_GAUGE_DURATION;
                unit.Entity.ChainId = newestChainId;
                newestChainId++;

                FireChainSugar(item);
            }
        }
    }

    private void BreakDownSugars()
    {
        foreach(var unit in sugarUnits.OrderBy(item => item.Entity.positionIdx.Value.y))
        {
            var target = sugarUnits
                .Where(item => item.Entity.positionIdx.Value.x == unit.Entity.positionIdx.Value.x)
                .Where(item => item.Entity.positionIdx.Value.y < unit.Entity.positionIdx.Value.y);

            if(target.Count() > 0)
            {
                var targetYIdx = target.Max(item => item.Entity.positionIdx.Value.y);
                if(unit.Entity.positionIdx.Value.y > targetYIdx + 1)
                {
                    unit.Entity.positionIdx.Value = new Vector2Int(
                        unit.Entity.positionIdx.Value.x,
                        targetYIdx + 1
                    );
                    unit.FallFastAsync(()=>{}).Forget();
                }
            }
        }
    }

    private CancellationTokenSource _cts;
    private SugarUnit currentSugarUnit;
    private IngameState ingameState;

    void Start()
    {
        _cts = new CancellationTokenSource();
        sugarFactory = new SugarFactory();
        sugarUnits = new ReactiveCollection<SugarUnit>();
    }

    void Update()
    {
        if(Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            currentSugarUnit.DecidedPosition();
            ingameState = IngameState.ChangeStateSugar;
            _cts.Cancel();
        }

        if(Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            //TODO 移動先のy座標を見て移動可能か判断する
            if(true)
            {
                var diff = -1;
                int targetX = currentSugarUnit.Entity.positionIdx.Value.x + diff;
                if(targetX < 0)
                {
                    diff += Const.FIELD_X_RANGE;
                    targetX += Const.FIELD_X_RANGE;
                }
                currentSugarUnit.MoveToSide(
                    diff,
                    new Vector2Int(targetX, GetLandablePositionIdx(targetX))
                );
                ingameState = IngameState.ChangeStateSugar;
                _cts.Cancel();
            }
        }

        if(Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            //TODO 移動先のy座標を見て移動可能か判断する
            if(true)
            {
                var diff = 1;
                int targetX = currentSugarUnit.Entity.positionIdx.Value.x + diff;
                if(targetX >= Const.FIELD_X_RANGE)
                {
                    diff -= Const.FIELD_X_RANGE;
                    targetX -= Const.FIELD_X_RANGE;
                }
                currentSugarUnit.MoveToSide(
                    diff,
                    new Vector2Int(targetX, GetLandablePositionIdx(targetX))
                );
                ingameState = IngameState.ChangeStateSugar;
                _cts.Cancel();
            }
        }

        //TODO 管理箇所を変更
        switch(ingameState)
        {
            case IngameState.Begin:
                ingameState = IngameState.CreateSugar;
                BuildFieldSugarUnit();
            break;

            case IngameState.CreateSugar:
                var xIdx = UnityEngine.Random.Range(0, Const.FIELD_X_RANGE);
                CreateSugarUnit(new Vector2Int(xIdx, GetLandablePositionIdx(xIdx)),false);
                MoveSugarUnitAsync(()=>{
                    FreezeSugar();
                }).Forget();
                ingameState = IngameState.FallSugar;
            break;

            case IngameState.FallSugar:
            break;

            case IngameState.ChangeStateSugar:
                ingameState = IngameState.FallSugar;
                if(currentSugarUnit.Entity.IsMoving.Value)
                {
                    MoveSugarUnitAsync(()=>{
                        FreezeSugar();
                    }).Forget();
                }
                else
                {
                    currentSugarUnit.FallFastAsync(()=>{
                        FreezeSugar();
                    }).Forget();
                }
            break;

            case IngameState.End:
            break;
        }
    }

    private void CreateSugarUnit(Vector2Int positionIdx, bool isPreInit)
    {
        var entity = sugarFactory.CreateSuger(positionIdx, isPreInit);
        var unit = Instantiate(sugerUnitPrefab, transform.position, Quaternion.identity);
        sugarUnits.Add(unit);

        if(isPreInit)
        {
            unit.Initialize(entity);
            unit.FallImmedatelyAsync(() =>
            {
                entity.IsFreeze = true;
            });
        }
        else
        {
            currentSugarUnit = unit;
            currentSugarUnit.Initialize(entity);
        }

        entity.IsDead.Subscribe(value =>
        {
            if(value)
            {
                unit.ExplodeAsync(() =>
                {
                    sugarUnits.Remove(unit);
                    Destroy(unit.gameObject);
                    BreakDownSugars();
                }).Forget();
            }
        }).AddTo(unit);
    }

    private void BuildFieldSugarUnit()
    {
        for(int i = 0; i < Const.FIELD_X_RANGE; i++)
        {
            for(int j = 0; j < Const.FIELD_Y_RANGE - 1; j++)
            {
                CreateSugarUnit(new Vector2Int(i, j), true);
            }
        }
    }

    private void FreezeSugar()
    {
        CheckAndFireSugar(currentSugarUnit);
        currentSugarUnit.Entity.IsFreeze = true;
        ingameState = IngameState.CreateSugar;
    }

    private async UniTask MoveSugarUnitAsync(Action onComplete)
    {
        _cts = new CancellationTokenSource();
        await currentSugarUnit.FallAsync(onComplete, _cts.Token);
    }

    void OnDestroy()
    {
        _cts.Dispose();
    }
}
