using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultPanelController : MonoBehaviour
{
    [SerializeField] RectTransform resultImage;

    [SerializeField] TMP_Text resultText;

    [SerializeField] Button restartButton;
    [SerializeField] Button exitButton;

    void Start()
    {
        resultImage.anchoredPosition = Vector2.zero;

        var offset = resultImage.offsetMin;
        offset.y = 680;
        resultImage.offsetMin = offset;

        Show();
    }

    public void Show()
    {       
        Sequence seq = DOTween.Sequence();

        var targetPos = resultImage.offsetMin;
        targetPos.y = -590;
        seq.Append(resultImage.DOMove(targetPos, 1.0f).SetEase(Ease.OutBounce));

        // 1. 이미지 위->아래로 내려옴
        

        // DOTween.To(() => resultImage.offsetMin, set => resultImage.offsetMin = set, targetPos, 1.0f).SetEase(Ease.OutBounce);

        // 2. 텍스트 좌->우 나타남
        //resultText.color = new Color(resultText.color.r, resultText.color.g, resultText.color.b, 0);
        //resultText.DOFade(1, 1.0f).SetDelay(1.0f);

        resultText.rectTransform.anchoredPosition = new Vector2(-600, 0f);
        resultText.rectTransform.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutBounce);


        // 3. 버튼 위->아래 바운스 등장 
    }
}

