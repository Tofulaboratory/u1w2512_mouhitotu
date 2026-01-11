using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UniRx;

public class View : MonoBehaviour
{
    [SerializeField]
    private GameObject nigiyaka1;

    [SerializeField]
    private GameObject nigiyaka2;

    [SerializeField]
    private GameObject title;

    [SerializeField]
    private GameObject timer;

    [SerializeField]
    private TextMeshProUGUI timerText;

    [SerializeField]
    private GameObject levelIndicator;

    [SerializeField]
    private TextMeshProUGUI levelIndicatorText;

    [SerializeField]
    private GameObject pressSpace;

    [SerializeField]
    private GameObject result;

    [SerializeField]
    private TextMeshProUGUI resultScoreText;

    [SerializeField]
    private TextMeshProUGUI resultLevelText;

    [SerializeField]
    private GameObject levelCenter;

    [SerializeField]
    private TextMeshProUGUI levelCenterText;

    [SerializeField]
    private GameObject levelUpAvailableText;

    public void InitializeForScoreManager()
    {
        ScoreManager.Instance.GameTime.Subscribe(time => timerText.text = $"{time}").AddTo(this).AddTo(this);
    }

    public void SetLevelText()
    {
        levelIndicatorText.text = $"LEVEL {ScoreManager.Instance.Level}";
        levelCenterText.text = $"LEVEL {ScoreManager.Instance.Level}";
        resultLevelText.text = $"LEVEL {ScoreManager.Instance.Level}";
    }

    public void SetActivelevelUpAvailableText(bool isActive)
    {
        levelUpAvailableText.SetActive(isActive);
    }

    public void UpdateState(IngameState state)
    {
        switch(state)
        {
            case IngameState.Title:
            nigiyaka1.SetActive(true);
            nigiyaka2.SetActive(false);
            title.SetActive(true);
            timer.SetActive(false);
            levelIndicator.SetActive(false);
            pressSpace.SetActive(true);
            result.SetActive(false);
            levelCenter.SetActive(false);
            levelUpAvailableText.SetActive(false);
            break;

            case IngameState.PrepareBegin:
            nigiyaka1.SetActive(false);
            title.SetActive(false);
            timer.SetActive(true);
            levelIndicator.SetActive(true);
            pressSpace.SetActive(false);

            nigiyaka2.SetActive(true);
            nigiyaka2.GetComponent<Animator>().SetTrigger("NigiyakaMove1");
            levelCenter.SetActive(true);
            SetLevelText();
            break;

            case IngameState.Begin:
            nigiyaka2.SetActive(false);          
            levelCenter.SetActive(false);
            break;

            case IngameState.CreateSugar:
            break;

            case IngameState.FallSugar:
            break;

            case IngameState.SideMoveSugar:
            break;

            case IngameState.FastFallSugar:
            break;

            case IngameState.PrepareToNextLevel:
            nigiyaka2.SetActive(true);
            nigiyaka2.GetComponent<Animator>().SetTrigger("NigiyakaMove1");
            levelCenter.SetActive(true);
            SetLevelText();
            break;

            case IngameState.ToNextLevel:
            nigiyaka2.SetActive(false);            
            levelCenter.SetActive(false);
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
