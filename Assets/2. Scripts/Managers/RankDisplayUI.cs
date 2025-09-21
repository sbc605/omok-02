using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위해 필요

public class RankDisplayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rankText; // 등급을 표시할 UI 텍스트

    void Start()
    {
        // GameManager 인스턴스에서 현재 급수 정보를 가져옵니다.
        RankData currentRank = GameManager.Instance.GetCurrentRank();
        
        // 급수 정보가 정상적으로 있다면 UI 텍스트를 업데이트합니다.
        if (currentRank != null)
        {
            rankText.text = $"나의 급수는 '{currentRank.rankName}' 입니다";
        }
        else
        {
            // 아직 급수가 결정되지 않은 경우 (예: 게임 첫 실행)
            rankText.text = "급수 정보 없음";
            // GameManager가 급수를 결정하도록 요청할 수도 있습니다.
            // GameManager.Instance.DetermineCurrentRank(); // 필요하다면 이 코드의 주석을 해제
        }
    }
}