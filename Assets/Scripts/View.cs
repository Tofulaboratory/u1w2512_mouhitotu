using UnityEngine;

public class View : MonoBehaviour
{
    [SerializeField]
    private GameObject nigiyaka1;

    private void UpdateState()
    {
        //TODO インゲームステート適用
    }

    public void SetActiveNigiyaka1(bool isActivate)
    {
        nigiyaka1.SetActive(isActivate);
    }
}
