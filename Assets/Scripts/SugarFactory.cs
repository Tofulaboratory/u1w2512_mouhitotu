using UnityEngine;

public class SugarFactory
{
    public SugarEntity CreateSuger(Vector2Int positionIdx, bool isPreInit)
    {
        var entity = new SugarEntity(isPreInit);
        entity.state.Value = (SugarState)Random.Range(0,(int)SugarState.LEBGTH);
        entity.positionIdx.Value = positionIdx;
        return entity;
    }
}