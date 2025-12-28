using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class SugarUnit : MonoBehaviour
{
    [SerializeField]
    private GameObject model;

    [SerializeField]
    private MeshRenderer meshRenderer;

    [SerializeField]
    private Material[] materials;

    public async UniTask InitializeAsync(SugarEntity entity)
    {
        model.SetActive(false);
        SetMaterial(entity);
        await MoveToBeginPosition(entity);
        model.SetActive(true);
    }

    public async UniTask FallAsync(SugarEntity entity)
    {
        await MoveTo(entity);
        LandedAsync().Forget();
    }

    private void SetMaterial(SugarEntity entity)
    {
        var index = Random.Range(0, materials.Length);
        meshRenderer.material = materials[index];
    }

    private async UniTask LandedAsync()
    {
        var prevScale = transform.localScale;
        await transform.DOScale(prevScale * 1.2f, 0.05f).AsyncWaitForCompletion();
        await transform.DOScale(prevScale, 0.05f).AsyncWaitForCompletion();
    }

    private async UniTask MoveTo(SugarEntity entity)
    {
        await transform.DOMove(SugarPositionSolver.GetSugarPosition(entity), 0.1f).AsyncWaitForCompletion();
    }

    private async UniTask MoveToBeginPosition(SugarEntity entity)
    {
        await transform.DOMove(SugarPositionSolver.GetSugarBeginPosition(entity), 0f).AsyncWaitForCompletion();
    }
}
