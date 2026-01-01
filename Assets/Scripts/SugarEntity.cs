using UniRx;
using UnityEngine;

public class SugarEntity
{
    public ReactiveProperty<SugarState> state;
    public ReactiveProperty<bool> isMoving;
    public ReactiveProperty<Vector2Int> positionIdx;

    public SugarEntity()
    {
        state = new ReactiveProperty<SugarState>();
        isMoving = new ReactiveProperty<bool>(true);
        this.positionIdx = new ReactiveProperty<Vector2Int>();
    }
}