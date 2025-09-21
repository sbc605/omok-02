using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SoundManager : Singleton<SoundManager>
{
    [Header("오디오 믹서")]
    public AudioMixer masterMixer;

    [Header("BGM 설정")]
    private AudioSource bgmPlayer;
    public List<GameManager.SceneMusic> sceneMusicList;

    protected override void Awake()
    {
        base.Awake();
        bgmPlayer = gameObject.AddComponent<AudioSource>();
        bgmPlayer.loop = true;

        var gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            sceneMusicList = gameManager.sceneMusicList;
        }
        
        if(masterMixer != null)
        {
            bgmPlayer.outputAudioMixerGroup = masterMixer.FindMatchingGroups("Master/BGM")[0];
        }
    }
    
    public void SetBGMVolume(float volume)
    {
        float db = (volume <= 0.0001f) ? -80f : Mathf.Log10(volume) * 20;
        masterMixer.SetFloat("BGMVolume", db);
    }

    public void SetSFXVolume(float volume)
    {
        float db = (volume <= 0.0001f) ? -80f : Mathf.Log10(volume) * 20;
        masterMixer.SetFloat("SFXVolume", db);
    }
    
    protected override void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (sceneMusicList == null) return;
        AudioClip clipToPlay = null;
        foreach (var sm in sceneMusicList)
        {
            if (sm.sceneName == scene.name)
            {
                clipToPlay = sm.musicClip;
                break;
            }
        }

        if (clipToPlay != null && bgmPlayer.clip != clipToPlay)
        {
            bgmPlayer.clip = clipToPlay;
            bgmPlayer.Play();
        }
        else if (clipToPlay == null)
        {
            bgmPlayer.Stop();
        }
    }
}