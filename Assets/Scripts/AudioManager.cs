using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class AudioManager : SingletonMonoBehaviour<AudioManager> {
    private Dictionary<string, AudioSource> _bgmSourceDic = new Dictionary<string, AudioSource> ();
    private AudioSource _seSource = null;
    private Dictionary<string, AudioClip> _seClipDic = new Dictionary<string, AudioClip> ();

    private const string PrefsKeyMasterVolume = "master_volume";

    private bool isInitialized = false;

    protected override void Awake () {
        base.Awake ();
    }

    public void Initialize () {
        if(isInitialized) return;
        isInitialized = true;

        object[] bgmData = Resources.LoadAll ("Audio/Exclude/BGM");
        object[] seData = Resources.LoadAll ("Audio/Exclude/SE");

        foreach (AudioClip bgm in bgmData) {
            Debug.Log(bgm);
            _bgmSourceDic[bgm.name] = gameObject.AddComponent<AudioSource> ();
            _bgmSourceDic[bgm.name].clip = bgm;
            _bgmSourceDic[bgm.name].volume = GetMasterVolume ();
        }

        foreach (AudioClip se in seData) {
            _seClipDic[se.name] = se;
        }

        _seSource = gameObject.AddComponent<AudioSource> ();
        _seSource.volume = GetMasterVolume ();
    }

    /// <summary>
    /// seをならす
    /// </summary>
    public void PlaySE (string name, float volume = 1) {
        if (!_seClipDic.ContainsKey (name)) return;

        _seSource.PlayOneShot (_seClipDic[name] as AudioClip, volume * GetMasterVolume ());
    }

    /// <summary>
    /// bgmをならす
    /// </summary>
    public void PlayBGM (string name, bool isLoop = true, float volume = 1) {
        if (!_bgmSourceDic.ContainsKey (name)) return;
        if (_bgmSourceDic[name].isPlaying) return;

        _bgmSourceDic[name].loop = isLoop;
        _bgmSourceDic[name].volume = volume * GetMasterVolume ();
        _bgmSourceDic[name].Play ();
    }

    /// <summary>
    /// bgmを止める
    /// </summary>
    public void StopBGM (string name) {
        if (!_bgmSourceDic.ContainsKey (name)) return;
        if (!_bgmSourceDic[name].isPlaying) return;

        _bgmSourceDic[name].Stop ();
    }

    public void SetMasterVolume (float value) {
        PlayerPrefs.SetFloat (PrefsKeyMasterVolume, value);

        foreach (var item in _bgmSourceDic.Values) {
            item.volume = GetMasterVolume ();
        }
        _seSource.volume = GetMasterVolume ();
    }

    public float GetMasterVolume () {
        float volume = 0.5f;
        if (PlayerPrefs.HasKey (PrefsKeyMasterVolume)) volume = PlayerPrefs.GetFloat (PrefsKeyMasterVolume);
        return volume;
    }
}