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
    private GameObject remainIndicator;

    [SerializeField]
    private TextMeshProUGUI remainIndicatorText;

    [SerializeField]
    private GameObject pressSpace;

    [SerializeField]
    private GameObject result;

    [SerializeField]
    private TextMeshProUGUI resultLevelText;

    [SerializeField]
    private GameObject levelCenter;

    [SerializeField]
    private TextMeshProUGUI levelCenterText;

    [SerializeField]
    private GameObject levelUpAvailableText;

    [SerializeField]
    private GameObject addTimeTextGameObject;

    [SerializeField]
    private TextMeshProUGUI addTimeText;

    private CompositeDisposable disposable = new CompositeDisposable();

    private int addTimeCount = 0;

    public void DisplayAddTimeText(float time) => DisplayAddTimeTextAsync(time).Forget();
    private async UniTask DisplayAddTimeTextAsync(float time)
    {
        if (time < 1) return;
        var t = (int)time;
        addTimeCount++;

        addTimeTextGameObject.SetActive(true);
        addTimeText.text = $"+{t}";
        var prevFontSize = 35f;
        addTimeText.fontSize = prevFontSize * 1.5f;
        await UniTask.Delay(100);
        addTimeText.fontSize = prevFontSize;
        await UniTask.Delay(900);
        addTimeCount--;
        if (addTimeCount <= 0)
        {
            addTimeTextGameObject.SetActive(false);
        }
    }

    public void InitializeForScoreManager()
    {
        disposable.Clear();
        ScoreManager.Instance.GameTime.Subscribe(time =>
        {
            timerText.text = $"{time}";
        }).AddTo(disposable);
        ScoreManager.Instance.AddGameTime.Subscribe(time => {
            DisplayAddTimeText(time);
        }).AddTo(disposable);
    }

    public void SetLevelText()
    {
        levelCenterText.text = $"LEVEL {ScoreManager.Instance.Level}";
        resultLevelText.text = $"LEVEL {ScoreManager.Instance.Level}";
    }

    public void SetActivelevelUpAvailableText(bool isActive)
    {
        levelUpAvailableText.SetActive(isActive);
    }

    public void SetRemainSugarCount(int count) => remainIndicatorText.text = $"残り{count}/5";

    public void UpdateState(IngameState state)
    {
        switch (state)
        {
            case IngameState.Title:
                nigiyaka1.SetActive(true);
                nigiyaka2.SetActive(false);
                title.SetActive(true);
                timer.SetActive(false);
                remainIndicator.SetActive(false);
                pressSpace.SetActive(true);
                result.SetActive(false);
                levelCenter.SetActive(false);
                levelUpAvailableText.SetActive(false);
                break;

            case IngameState.PrepareBegin:
                nigiyaka1.SetActive(false);
                title.SetActive(false);
                timer.SetActive(true);
                remainIndicator.SetActive(true);
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