using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultPanelController : MonoBehaviour
{
    [Header("Panel Components")]
    [SerializeField] RectTransform resultImage; // 각 Win, Draw, Lose 중 하나
    [SerializeField] TMP_Text resultText;
    [SerializeField] Button replayButton;
    [SerializeField] Button exitButton;

    private Sequence seq;

    void OnEnable()
    {
        resultImage.anchoredPosition = Vector2.zero;

        var offset = resultImage.offsetMin;
        offset.y = 680;
        resultImage.offsetMin = offset;

        Show();
    }

    public void Show()
    {       
        seq = DOTween.Sequence();

        // 1. 이미지 위->아래로 내려옴
        var startPos = resultImage.offsetMin;
        var targetPos = new Vector2(startPos.x, -590);
        
        seq.Append(DOTween.To(() => resultImage.offsetMin, set => resultImage.offsetMin = set, targetPos, 0.6f).SetEase(Ease.OutBounce));


        // 2. 텍스트 좌->우 나타남 + 페이드        
        resultText.rectTransform.anchoredPosition = new Vector2(-600, 0f);
        resultText.alpha = 0;
        seq.Insert(0.4f, resultText.rectTransform.DOAnchorPos(Vector2.zero, 0.8f).SetEase(Ease.OutBounce));
        seq.Join(resultText.DOFade(1, 0.6f)); // 페이드인

        // 3. 버튼 
        // 버튼은 alpha 제어가 안돼서 CanvasGroup으로 alpha 조절
        CanvasGroup replayGroup = replayButton.GetComponent<CanvasGroup>();
        CanvasGroup exitGroup = exitButton.GetComponent<CanvasGroup>();
        if (replayGroup == null) replayGroup = replayButton.gameObject.AddComponent<CanvasGroup>();
        if (exitGroup == null) exitGroup = exitButton.gameObject.AddComponent<CanvasGroup>();

        // 초기값: 위쪽 배치 + 투명
        replayButton.transform.localPosition = new Vector3(0, 0, 0);
        exitButton.transform.localPosition = new Vector3(0, -100, 0);
        replayGroup.alpha = 0;
        exitGroup.alpha = 0;

        // 페이드 + 위->아래 바운스 등장
        seq.Insert(0.8f, replayButton.transform.DOLocalMoveY(-68, 0.7f).SetEase(Ease.OutBounce));
        seq.Join(replayGroup.DOFade(1, 0.6f)); // 페이드인
        seq.Join(exitButton.transform.DOLocalMoveY(-210, 0.9f).SetEase(Ease.OutBounce));
        seq.Join(exitGroup.DOFade(1, 0.6f)); // 페이드인
    }

    // 화면 터치시 애니메이션 스킵
    private void Update()
    {
        if (seq != null && seq.IsActive() && !seq.IsComplete())
        {
            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
            {
                seq.Complete(true);
            }
        }
    }
}

