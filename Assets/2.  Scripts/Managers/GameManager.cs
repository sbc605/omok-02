using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


public class GameManager : Singleton<GameManager>
{
    private AudioSource bgmPlayer;

    [System.Serializable]
    public class SceneMusic
    {
        public string sceneName;
        public AudioClip musicClip;
    }

    public List<SceneMusic> sceneMusicList;


    protected override void Awake()
    {
        base.Awake(); 

        // Debug.Log("===== GameManager AWAKE =====");
        bgmPlayer = gameObject.AddComponent<AudioSource>();
        bgmPlayer.loop = true;
    }

    protected override void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        // 1. 이 함수가 호출되는지 확인
        // Debug.Log($"===== Scene Loaded: {scene.name} =====");

        AudioClip clipToPlay = null;

        // 2. sceneMusicList에 설정된 값들을 모두 확인
        // Debug.Log($"Checking {sceneMusicList.Count} music entries.");
        foreach (var sm in sceneMusicList)
        {
            // Debug.Log($"Entry: SceneName='{sm.sceneName}', Clip='{(sm.musicClip != null ? sm.musicClip.name : "NULL")}'");
            if (sm.sceneName == scene.name)
            {
                clipToPlay = sm.musicClip;
                // 3. 일치하는 클립을 찾았는지 확인
                Debug.Log($"Success: Found matching clip '{clipToPlay.name}' for this scene.");
                break;
            }
        }

        if (clipToPlay != null)
        {
            // 4. 실제로 음악을 재생하려고 하는지 확인
            Debug.Log("Clip is not null, attempting to play music.");
            if (bgmPlayer.clip != clipToPlay)
            {
                bgmPlayer.clip = clipToPlay;
                bgmPlayer.Play();
                // Debug.Log($"Now playing: {bgmPlayer.clip.name}, Volume: {bgmPlayer.volume}");
            }
            else
            {
                Debug.Log("Music is already the correct one, doing nothing.");
            }
        }
        else
        {
            // 5. 일치하는 음악을 못 찾았는지 확인
            Debug.LogWarning($"Warning: No music clip found for scene '{scene.name}'. Stopping music.");
            bgmPlayer.Stop();
        }
    }

    public void LoadMainScene()
    {
        SceneManager.LoadScene("Main");
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene("Game");
    }

    public void LoadSettingScene()
    {
        SceneManager.LoadScene("Setting");
    }
}