using DG.Tweening;
using UnityEngine;

public class SettingPanelController : MonoBehaviour
{
    [SerializeField] RectTransform settingImage;

    void OnEnable()
    {
        settingImage.localScale = Vector3.zero;

        Show();
    }

    void Show()
    {
        settingImage.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }
}
