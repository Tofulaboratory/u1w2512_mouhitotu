using UnityEngine;

public class SugarFactory
{
    public SugarEntity CreateSuger()
    {
        return new SugarEntity();
    }
}