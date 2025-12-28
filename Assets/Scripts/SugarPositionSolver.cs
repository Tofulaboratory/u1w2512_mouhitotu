using UnityEngine;

public static class SugarPositionSolver
{
    private static Vector3 basePosition = new Vector3(-14f, -1.93f, 73.6f);
    private static float positionIntervalX = 1.174f;
    private static float positionIntervalY = 1f;
    private static float beginPointPositionIdx = 20;

    public static Vector3 GetSugarPosition(SugarEntity entity)
    {
        return basePosition + new Vector3(
            entity.positionIdx.x * positionIntervalX,
            entity.positionIdx.y * positionIntervalY,
            0
        );
    }

    public static Vector3 GetSugarBeginPosition(SugarEntity entity)
    {
        return basePosition + new Vector3(
            entity.positionIdx.x * positionIntervalX,
            beginPointPositionIdx * positionIntervalY,
            0
        );
    }
}
