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

    public async UniTask InitializeAsync(SugarEntity entity, CancellationToken ct = new CancellationToken())
    {
        this.entity = entity;

        model.SetActive(false);
        SetMaterial();
        await MoveToBeginPosition(ct);
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
                this.entity.IsDead = true;
                return;
            }

            await UniTask.Yield();
        }
    }

    public async UniTask FallAsync(Action onComplete, CancellationToken ct = new CancellationToken())
    {
        await MoveToTargetPositionY(1f,ct);
        LandedAsync(ct).Forget();
        if(!ct.IsCancellationRequested)
        {
            onComplete?.Invoke();
        }
    }

    public async UniTask FallFastAsync(Action onComplete, CancellationToken ct = new CancellationToken())
    {
        await MoveToTargetPosition(0.1f, ct);
        LandedAsync(ct).Forget();
        if(!ct.IsCancellationRequested)
        {
            onComplete?.Invoke();
        }
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

    private async UniTask LandedAsync(CancellationToken ct = new CancellationToken())
    {
        var prevScale = transform.localScale;
        await transform.DOScale(prevScale * 1.2f, 0.05f).ToUniTask(cancellationToken:ct);
        await transform.DOScale(prevScale, 0.05f).ToUniTask(cancellationToken:ct);
    }

    private async UniTask MoveToTargetPositionY(float interval, CancellationToken ct = new CancellationToken())
    {
        await transform.DOMoveY(SugarPositionSolver.GetSugarTargetPosition(entity).y, interval).ToUniTask(cancellationToken:ct);
    }

    private async UniTask MoveToTargetPosition(float interval, CancellationToken ct = new CancellationToken())
    {
        await transform.DOMove(SugarPositionSolver.GetSugarTargetPosition(entity),interval).ToUniTask(cancellationToken:ct);
    }

    private async UniTask MoveToBeginPosition(CancellationToken ct = new CancellationToken())
    {
        await transform.DOMove(SugarPositionSolver.GetSugarBeginPosition(entity), 0f).ToUniTask(cancellationToken:ct);
    }
}
