using UniRx;
using UnityEngine;

public class SugarEntity
{
    public ReactiveProperty<SugarState> state;
    public ReactiveProperty<bool> IsMoving;
    public bool IsFreeze;
    public ReactiveProperty<bool> IsWaitCombo;
    public bool IsDead;
    public ReactiveProperty<Vector2Int> positionIdx;
    public ReactiveProperty<float> WaitComboGaugeNum;
    public int ChainId = -1;

    public SugarEntity()
    {
        state = new ReactiveProperty<SugarState>();
        IsMoving = new ReactiveProperty<bool>(true);
        IsWaitCombo = new ReactiveProperty<bool>(false);
        positionIdx = new ReactiveProperty<Vector2Int>();
        WaitComboGaugeNum = new ReactiveProperty<float>(Const.SUGAR_GAUGE_DURATION);
    }

    public bool IsNeighbor(Vector2Int positionIdx)
    {
        return Vector2Int.Distance(this.positionIdx.Value, positionIdx) <= 1;
    }
}