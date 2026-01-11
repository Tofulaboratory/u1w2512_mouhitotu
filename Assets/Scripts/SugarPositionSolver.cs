using UnityEngine;

public static class SugarPositionSolver
{
    private static Vector3 basePosition = new Vector3(-14f, -1.93f, 73.6f);
    private static float positionIntervalX = 1.174f;
    private static float positionIntervalY = 1f;
    private static float beginPointPositionIdx = 16f;

    public static Vector3 GetSugarTargetPosition(SugarEntity entity)
    {
        return basePosition + new Vector3(
            entity.positionIdx.Value.x * positionIntervalX,
            entity.positionIdx.Value.y * positionIntervalY,
            0
        );
    }

    public static Vector3 GetSugarBeginPosition(SugarEntity entity)
    {
        return basePosition + new Vector3(
            entity.positionIdx.Value.x * positionIntervalX,
            beginPointPositionIdx * positionIntervalY,
            0
        );
    }

    public static Vector3 GetSugarSidePosition(Vector3 beforePos, int slideIdx)
    {
        return beforePos + new Vector3(
            positionIntervalX * slideIdx,
            0,
            0
        );
    }
}
