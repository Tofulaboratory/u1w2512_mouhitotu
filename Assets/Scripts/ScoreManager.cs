using System;
using Cysharp.Threading.Tasks;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreManager : SingletonMonoBehaviour<ScoreManager>
{
    [NonSerialized]
    public int Score;

    [NonSerialized]
    public int Level;

    [NonSerialized]
    public ReactiveProperty<int> GameTime;

    [NonSerialized]
    public ReactiveProperty<float> AddGameTime;

    private CompositeDisposable disposable = new CompositeDisposable();

    public void Initialize()
    {
        disposable.Clear();

        Score = 0;
        Level = 1;
        GameTime = new ReactiveProperty<int>(Const.TIME_DURATION_LEVEL);
        AddGameTime = new ReactiveProperty<float>();
        AddGameTime.Subscribe(value => {
            if(value >= 1)
            {
                var add = (int)value;
                AddGameTime.Value -= add;
                GameTime.Value += add;
            }
        }).AddTo(disposable);
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