using UnityEngine;

public class ScoreManager : SingletonMonoBehaviour<ScoreManager>
{
    [System.NonSerialized]
    public int Score;

    [System.NonSerialized]
    public int Level;

    public void Initialize()
    {
        Score = 0;
        Level = 1;
    }
}