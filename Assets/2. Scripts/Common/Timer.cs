using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Timer : MonoBehaviour
{
    private float time = 30;
    private float currTime = 0;

    [SerializeField] Image bgImage; // 배경 이미지

    [SerializeField] Image charImage; // 캐릭터 이미지 UI
    [SerializeField] Sprite originCharImage; // 기본 캐릭터 이미지
    [SerializeField] Sprite newCharImage; // 변경할 캐릭터 이미지

    private bool isFlip = false;
    private bool isNewImage = false;

    void Start()
    {
        currTime = time;
        bgImage.type = Image.Type.Filled;
        bgImage.fillMethod = Image.FillMethod.Horizontal;
        bgImage.fillOrigin = (int)Image.OriginHorizontal.Left; // 오->왼 방향으로 사라짐
        bgImage.fillAmount = 1;

        charImage.sprite = originCharImage;
    }

    void Update()
    {
        TimePass();
    }

    void TimePass()
    {
        currTime -= Time.deltaTime;
        bgImage.fillAmount = currTime / time;

        if (currTime <= 15 && !isFlip)
        {
            ChangeImage();
            isFlip = true;
        }

        if (currTime <= 0)
        {
            currTime = time;
            isFlip = false;
        }
    }

    void ChangeImage()
    {
        RectTransform rt = charImage.rectTransform;
        rt.localScale = Vector3.one;
        rt.localRotation = Quaternion.identity;

        Sequence seq = DOTween.Sequence();

        // 1. 앞면에서 180도 회전 + 크기 키우기
        seq.Append(
            rt.DOScale(1.2f, 0.25f).SetEase(Ease.OutQuad)
        );
        seq.Join(
            rt.DOLocalRotate(new Vector3(0, 90, 0), 0.25f).SetEase(Ease.InQuad)
        );

        // 2. 회전 중간에 이미지 교체(반복)
        seq.AppendCallback(() =>
        {
           if (isNewImage)
                charImage.sprite = originCharImage;
            else
                charImage.sprite = newCharImage;

            isNewImage = !isNewImage;
        });

        // 3. 다시 180도 회전 + 크기 되돌리기
        seq.Append(
            rt.DOLocalRotate(Vector3.zero, 0.25f).SetEase(Ease.OutQuad)
        );
        seq.Join(
            rt.DOScale(1f, 0.25f).SetEase(Ease.InQuad)
        );
    }
}
