using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultPanelController : MonoBehaviour
{
    [Header("결과 이미지/텍스트")]
    [SerializeField] private Image winImage;    // 승리 시 켤 이미지
    [SerializeField] private Image loseImage;   // 패배 시 켤 이미지
    [SerializeField] private Image drawImage;   // 무승부 시 켤 이미지
    
    [Header("애니메이션 대상")]
    [SerializeField] RectTransform resultImageContainer; // 각 Win, Draw, Lose 이미지의 부모
    [SerializeField] TMP_Text resultText;
    [SerializeField] Button replayButton;
    [SerializeField] Button exitButton;

    private Sequence seq;

    // GameManager가 호출할 함수
    public void ShowResult(GameLogic.GameResult result)
    {
        Debug.Log(">> ResultPanelController: 결과 표시 명령 수신 완료!");
        // 모든 결과 이미지를 일단 끈다
        winImage.gameObject.SetActive(false);
        loseImage.gameObject.SetActive(false);
        drawImage.gameObject.SetActive(false);

        // 결과에 맞는 이미지만 켠다
        if (result == GameLogic.GameResult.Win)
            winImage.gameObject.SetActive(true);
        else if (result == GameLogic.GameResult.Lose)
            loseImage.gameObject.SetActive(true);
        else if (result == GameLogic.GameResult.Draw)
            drawImage.gameObject.SetActive(true);
            
        // 패널 전체를 활성화하여 등장 애니메이션(OnEnable)을 실행시킨다
        gameObject.SetActive(true);
    }
    
    // '다시하기' 버튼이 눌렸을 때 호출될 함수
    public void OnReplayButtonClicked()
    {
        // GameManager에게 게임 재시작을 요청
        GameManager.Instance.RestartGame();
    }

    // '나가기' 버튼이 눌렸을 때 호출될 함수
    public void OnExitButtonClicked()
    {
        // GameManager에게 메인 씬으로 이동을 요청
        GameManager.Instance.LoadMainScene();
    }

    #region 기존 애니메이션 코드 (수정 불필요)

    void OnEnable()
    {
        resultImageContainer.anchoredPosition = Vector2.zero;

        var offset = resultImageContainer.offsetMin;
        offset.y = 680;
        resultImageContainer.offsetMin = offset;

        ShowAnimation();
    }

    void ShowAnimation()
    {
        // ... (기존 애니메이션 코드는 그대로 사용) ...
    }
    
    private void Update()
    {
        // ... (기존 애니메이션 스킵 코드는 그대로 사용) ...
    }
    #endregion
}