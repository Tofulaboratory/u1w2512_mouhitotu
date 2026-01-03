using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using UniRx;
using UnityEngine.UI;

public class SugarUnit : MonoBehaviour
{
    [SerializeField]
    private GameObject model;

    [SerializeField]
    private GameObject ghostModel;

    [SerializeField]
    private Image gaugeUI;

    [SerializeField]
    private MeshRenderer meshRenderer;

    [SerializeField]
    private Material[] materials;

    private SugarEntity entity;
    public SugarEntity Entity => entity;

    private CancellationTokenSource _cts = new CancellationTokenSource();

    private float remainFallTime = -1;
    private async UniTask CountRemainFallTimeAsync(float initFallTime, CancellationToken ct)
    {
        if(remainFallTime == -1 || remainFallTime > initFallTime)
        {
            remainFallTime = initFallTime;
        }

        while(!ct.IsCancellationRequested)
        {
            remainFallTime -= Time.deltaTime;
            if(remainFallTime <= 0)
            {
                remainFallTime = -1;
                return;
            }

            await UniTask.Yield();
        }
    }

    public void Initialize(SugarEntity entity, CancellationToken ct = new CancellationToken())
    {
        this.entity = entity;

        model.SetActive(false);
        SetMaterial();
        MoveToBeginPosition(ct);
        model.SetActive(true);

        Entity.IsWaitCombo.Subscribe(value =>
        {
            if(value)
            {
                gaugeUI.gameObject.SetActive(true);
                MoveGaugeAsync().Forget();
            }
        }).AddTo(this);

        Entity.WaitComboGaugeNum.Subscribe(value =>
        {
            gaugeUI.fillAmount = value / Const.SUGAR_GAUGE_DURATION;
        }).AddTo(this);
    }

    private async UniTask MoveGaugeAsync()
    {
        while(true)
        {
            Entity.WaitComboGaugeNum.Value -= Time.deltaTime;
            if(Entity.WaitComboGaugeNum.Value <= 0)
            {
                this.entity.IsDead.Value = true;
                return;
            }

            await UniTask.Yield();
        }
    }

    public async UniTask FallAsync(Action onComplete, CancellationToken ct = new CancellationToken())
    {
        CountRemainFallTimeAsync(Const.FALL_DURATION, ct).Forget();
        await MoveToTargetPositionY(remainFallTime,ct);
        if(!ct.IsCancellationRequested)
        {
            LandedAsync(ct).Forget();
            onComplete?.Invoke();
        }
    }

    public async UniTask FallFastAsync(Action onComplete, CancellationToken ct = new CancellationToken())
    {
        CountRemainFallTimeAsync(Const.FALL_FAST_DURATION, ct).Forget();
        await MoveToTargetPosition(remainFallTime, ct);
        if(!ct.IsCancellationRequested)
        {
            LandedAsync(ct).Forget();
            onComplete?.Invoke();
        }
    }

    public void FallImmedatelyAsync(Action onComplete)
    {
        MoveToTargetPosition(0).Forget();
        LandedAsync().Forget();
        onComplete?.Invoke();
    }

    public void DecidedPosition()
    {
        entity.IsMoving.Value = false;
    }

    public void SetVisibleGhost(bool isVisible)
    {
        ghostModel.SetActive(isVisible);
    }

    public void MoveToSide(int directionX, Vector2Int target)
    {
        entity.positionIdx.Value = target;
        transform.DOMove(SugarPositionSolver.GetSugarSidePosition(transform.position,directionX), 0f);
    }

    private void SetMaterial()
    {
        var index = (int)entity.state.Value;
        meshRenderer.material = materials[index];
    }

    private async UniTask LandedAsync(CancellationToken? ct = null)
    {
        var _ct = ct == null ? new CancellationToken() : _cts.Token;
        if(_ct.IsCancellationRequested)
        {
            return;
        }

        var prevScale = transform.localScale;
        await transform.DOScale(prevScale * 1.2f, 0.05f).ToUniTask(cancellationToken:_ct);
        await transform.DOScale(prevScale, 0.05f).ToUniTask(cancellationToken:_ct);
    }

    public async UniTask ExplodeAsync(Action onComplete, CancellationToken ct = new CancellationToken())
    {
        var duration = 0.1f;
        var prevScale = transform.localScale;
        DoThinMaterial(duration, ct).Forget();
        await transform.DOScale(prevScale * 3f, duration).ToUniTask(cancellationToken:ct);
        onComplete?.Invoke();
    }

    private async UniTask DoThinMaterial(float duration, CancellationToken ct = new CancellationToken())
    {
        float _time = 1;
        while(!ct.IsCancellationRequested)
        {
            _time -= Time.deltaTime / duration;

            meshRenderer.material.color = new Color(
                meshRenderer.material.color.r,
                meshRenderer.material.color.g,
                meshRenderer.material.color.b,
                _time
            );

            if(_time <= 0)
            {
                return;
            }
            await UniTask.Yield();
        }
    }

    private async UniTask MoveToTargetPositionY(float interval, CancellationToken ct = new CancellationToken())
    {
        await transform.DOMoveY(SugarPositionSolver.GetSugarTargetPosition(entity).y, interval).ToUniTask(cancellationToken:ct);
    }

    private async UniTask MoveToTargetPosition(float interval, CancellationToken ct = new CancellationToken())
    {
        await transform.DOMove(SugarPositionSolver.GetSugarTargetPosition(entity),interval).ToUniTask(cancellationToken:ct);
    }

    private void MoveToBeginPosition(CancellationToken ct = new CancellationToken())
    {
        transform.DOMove(SugarPositionSolver.GetSugarBeginPosition(entity), 0f).ToUniTask(cancellationToken:ct).Forget();
    }

    void OnDestroy()
    {
        _cts.Cancel();
    }
}
