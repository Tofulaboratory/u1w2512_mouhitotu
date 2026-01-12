using UnityEngine;

public class SugarFactory
{
    public SugarEntity CreateSuger(Vector2Int positionIdx, bool isPreInit, bool isGhost)
    {
        var entity = new SugarEntity(isPreInit, isGhost);
        entity.state.Value = (SugarState)Random.Range(0,(int)SugarState.LEBGTH);
        entity.positionIdx.Value = positionIdx;
        return entity;
    }
}