using UnityEngine;
using TMPro;
using System.Collections; // 코루틴을 사용하기 위해 추가

public class FavorabilityDisplayUI : MonoBehaviour
{
    [Header("UI 텍스트 연결")]
    [SerializeField] private TextMeshProUGUI currentFavorText;
    [SerializeField] private TextMeshProUGUI entryFeeText;

    // 씬이 활성화될 때마다 UI 갱신 코루틴을 시작합니다.
    void OnEnable()
    {
        // 텍스트를 즉시 비워서 이전 씬의 정보가 보이는 것을 방지
        if (currentFavorText != null) currentFavorText.text = "";
        if (entryFeeText != null) entryFeeText.text = "";

        StartCoroutine(UpdateUIAfterDelay());
    }

    // 한 프레임 늦게 UI를 업데이트하는 코루틴
    private IEnumerator UpdateUIAfterDelay()
    {
        // 모든 Start, Awake, OnSceneLoad가 실행될 때까지 딱 한 프레임만 기다립니다.
        yield return null; 

        // 현재 호감도 표시
        if (currentFavorText != null)
        {
            int currentFavor = PlayerDataManager.Instance.Favorability;
            currentFavorText.text = $"보유 호감도: {currentFavor}";
        }

        // 입장료 표시
        if (entryFeeText != null)
        {
            RankData currentRank = GameManager.Instance.GetCurrentRank();
            if (currentRank != null)
            {
                if (currentRank.entryFee > 0)
                {
                    entryFeeText.text = $"차감 호감도: -{currentRank.entryFee}";
                }
                else
                {
                    entryFeeText.text = "무료";
                }
            }
            else
            {
                entryFeeText.text = "급수 정보 없음";
            }
        }
    }
}