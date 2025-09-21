using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Header("UI 요소 연결")]
    public Slider bgmSlider;
    public Slider sfxSlider;

    void Start()
    {
        if (bgmSlider != null)
        {
            // 슬라이더 값이 바뀔 때마다 SoundManager의 함수를 호출하도록 연결
            bgmSlider.onValueChanged.AddListener(SoundManager.Instance.SetBGMVolume);
        }
        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.AddListener(SoundManager.Instance.SetSFXVolume);
        }
    }
}