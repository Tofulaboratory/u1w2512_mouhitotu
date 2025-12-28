using Cysharp.Threading.Tasks;
using UnityEngine;

public class SugersController : MonoBehaviour
{
    [SerializeField]
    private SugarUnit sugerUnitPrefab;

    async void Start()
    {
        for(int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 24; j++)
            {
                await CreateSugerUnit(new SugarEntity(){state = SugarState.White, positionIdx = new Vector2Int(j, i)});
            }
            //await UniTask.Delay(100);
        }
    }

    public async UniTask CreateSugerUnit(SugarEntity entity)
    {
        var sugarUnit = Instantiate(sugerUnitPrefab, transform.position, Quaternion.identity);
        await sugarUnit.InitializeAsync(entity);
        await sugarUnit.FallAsync(entity);
    }
}
