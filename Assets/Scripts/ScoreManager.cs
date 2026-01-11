using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class ScoreManager : SingletonMonoBehaviour<ScoreManager>
{
    [NonSerialized]
    public int Score;

    [NonSerialized]
    public int Level;

    [NonSerialized]
    public ReactiveProperty<int> GameTime;

    public void Initialize()
    {
        Score = 0;
        Level = 1;
        GameTime = new ReactiveProperty<int>(Const.TIME_DURATION_LEVEL);
    }

    public async UniTask BeginTimer(Action onEnd)
    {
        while (true)
        {
            await UniTask.Delay(1000);
            GameTime.Value--;
            if(GameTime.Value <= 0)
            {
                onEnd?.Invoke();
                return;
            }
        }
    }

    public void ResetTimer()
    {
        GameTime.Value = Const.TIME_DURATION_LEVEL;
    }
}