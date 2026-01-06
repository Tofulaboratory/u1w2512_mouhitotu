using UnityEngine;

public class View : MonoBehaviour
{
    [SerializeField]
    private GameObject nigiyaka1;

    [SerializeField]
    private GameObject title;

    [SerializeField]
    private GameObject timer;

    [SerializeField]
    private GameObject levelIndicator;

    [SerializeField]
    private GameObject pressSpace;

    [SerializeField]
    private GameObject result;

    public void UpdateState(IngameState state)
    {
        switch(state)
        {
            case IngameState.Title:
            nigiyaka1.SetActive(true);
            title.SetActive(true);
            timer.SetActive(false);
            levelIndicator.SetActive(false);
            pressSpace.SetActive(true);
            result.SetActive(false);
            break;

            case IngameState.Begin:
            nigiyaka1.SetActive(false);
            title.SetActive(false);
            timer.SetActive(true);
            levelIndicator.SetActive(true);
            pressSpace.SetActive(false);
            break;

            case IngameState.CreateSugar:
            break;

            case IngameState.FallSugar:
            break;

            case IngameState.ChangeStateSugar:
            break;

            case IngameState.End:
            break;

            case IngameState.Result:
            result.SetActive(true);
            break;

            default:
            break;
        }
    }
}
