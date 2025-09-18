using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Timer : MonoBehaviour
{
    private float time = 10;
    private float currTime = 0;

    [SerializeField] Image bgImage; // 배경 이미지

    [SerializeField] Image charImage; // 캐릭터 이미지
    [SerializeField] Sprite newCharImage; // 변경할 캐릭터 이미지

    void Start()
    {
        currTime = time;
        bgImage.type = Image.Type.Filled;
        bgImage.fillMethod = Image.FillMethod.Horizontal;
        bgImage.fillOrigin = (int)Image.OriginHorizontal.Left; // 오->왼 방향으로 사라짐
        bgImage.fillAmount = 1;
    }

    void Update()
    {
        TimePass();
    }

    void TimePass()
    {
        currTime -= Time.deltaTime;
        bgImage.fillAmount = currTime / time;

        if (currTime <= 5)
        {
            ChangeImage();
        }

        if (currTime <= 0)
        {
            currTime = time;
        }
    }

    void ChangeImage()
    {
        charImage.transform.localScale = Vector3.one;
        charImage.transform.rotation = Quaternion.identity;

        charImage.transform.DOLocalRotate(new Vector3(0f, 0f, 90f), 0.25f, RotateMode.Fast).SetEase(Ease.InQuad).OnComplete(() =>
        {
            charImage.sprite = newCharImage;
            charImage.transform.DOLocalRotate(Vector3.zero, 0.25f, RotateMode.Fast).SetEase(Ease.OutQuad);
        });
    }
}
