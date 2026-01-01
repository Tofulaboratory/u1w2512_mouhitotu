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

    private ReactiveCollection<SugarEntity> sugarEntities;
    private int GetLandablePositionIdx(int xIdx)
    {
        var yIdx = sugarEntities
            ?.Where(item => item.positionIdx?.Value.x == xIdx)
            ?.Max(item => item.positionIdx?.Value.y);
        return (yIdx ?? -1) + 1;
    }

    private CancellationTokenSource _cts;
    private SugarUnit currentSugarUnit;
    private IngameState ingameState;

    void Start()
    {
        _cts = new CancellationTokenSource();
        sugarFactory = new SugarFactory();
        sugarEntities = new ReactiveCollection<SugarEntity>();
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
                var targetX = currentSugarUnit.Entity.positionIdx.Value.x - 1;
                currentSugarUnit.MoveToSide(
                    -1,
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
                var targetX = currentSugarUnit.Entity.positionIdx.Value.x + 1;
                currentSugarUnit.MoveToSide(
                    1,
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
            break;

            case IngameState.CreateSugar:
                var xIdx = UnityEngine.Random.Range(0,24);
                CreateSugerUnitAsync(new Vector2Int(xIdx, GetLandablePositionIdx(xIdx))).Forget();
                MoveSugarUnitAsync(()=>{
                    ingameState = IngameState.CreateSugar;
                }).Forget();
                ingameState = IngameState.FallSugar;
            break;

            case IngameState.FallSugar:
            break;

            case IngameState.ChangeStateSugar:
                ingameState = IngameState.FallSugar;
                if(currentSugarUnit.Entity.isMoving.Value)
                {
                    MoveSugarUnitAsync(()=>{
                        ingameState = IngameState.CreateSugar;
                    }).Forget();
                }
                else
                {
                    currentSugarUnit.FallFastAsync(()=>{
                        ingameState = IngameState.CreateSugar;
                    }).Forget();
                }
            break;

            case IngameState.End:
            break;
        }
    }

    public async UniTask CreateSugerUnitAsync(Vector2Int positionIdx)
    {
        var entity = sugarFactory.CreateSuger(positionIdx);
        sugarEntities.Add(entity);

        currentSugarUnit = Instantiate(sugerUnitPrefab, transform.position, Quaternion.identity);
        await currentSugarUnit.InitializeAsync(entity);
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
