using UnityEngine;

// 이 클래스는 유니티 메뉴에서 이 데이터 파일을 직접 생성할 수 있게 해줍니다.
[CreateAssetMenu(fileName = "New Rank Data", menuName = "Omok/Rank Data")]
public class RankData : ScriptableObject
{
    [Header("급수 이름")]
    public string rankName; // 예: 초심자, 숙련자, 고수

    [Header("요구 호감도 (이 점수 이상이면 해당 급수)")]
    public int requiredFavorability;

    [Header("AI 난이도")]
    public SimpleAI.Difficulty aiDifficulty;

    [Header("보상 및 패널티")]
    public int winBonus; // 승리 시 획득 호감도
    public int losePenalty; // 패배 시 차감 호감도
    public int entryFee; // 입장료
}