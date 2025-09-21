using UnityEngine;
using UnityEngine.UI; // Image를 사용하기 위해 필요

public class GameFavorabilitySlider : MonoBehaviour
{
    // ▼▼▼ 제어할 대상을 Scrollbar에서 Image로 변경 ▼▼▼
    [SerializeField] private Image favorabilityFillImage;

    [SerializeField] private float maxFavorabilityToShow = 1000f;

    void OnEnable()
    {
        UpdateFillAmount();
    }

    private void UpdateFillAmount()
    {
        if (favorabilityFillImage == null)
        {
            Debug.LogError("오류: favorabilityFillImage가 Inspector에 연결되지 않았습니다!");
            return;
        }

        int currentFavor = PlayerDataManager.Instance.Favorability;

        // ▼▼▼ 값을 설정하는 로직을 Image의 Fill Amount에 맞게 변경 ▼▼▼
        float ratio = (float)currentFavor / maxFavorabilityToShow;
        
        // Image의 fillAmount 값(0~1)을 조절하여 채워지는 양을 결정합니다.
        favorabilityFillImage.fillAmount = Mathf.Clamp01(ratio);
    }
}