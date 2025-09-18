using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(SpriteRenderer))]
public class Omok : MonoBehaviour
{
    [SerializeField] private Sprite whiteSprite;
    [SerializeField] private Sprite blackSprite;
    [SerializeField] private Sprite forbiddenSprite; // X마크 스프라이트 할당 - 이재현
    [SerializeField] private Sprite previewSprite;
    [SerializeField] private SpriteRenderer markerSR;

    public enum MarkerType { None, White, Black, Preview, Forbidden } // Forbidden(x마크)추가 - 이재현
    private MarkerType currentMarker = MarkerType.None;

    private int row, col;
    private OmokController omokController;

    private void Awake()
    {
        markerSR = GetComponent<SpriteRenderer>();
    }

    // 초기화(좌표 및 컨트롤러 설정)
    public void InitMarker(int r, int c, OmokController controller)
    {
        row = r;
        col = c;
        omokController = controller;
        SetMarker(MarkerType.None);
    }

    // 마커 설정
    public void SetMarker(MarkerType marker)
    {
        currentMarker = marker;

        // 기존 Tween 제거
        markerSR.DOKill();
        transform.DOKill();

        switch (marker)
        {
            case MarkerType.None:
                markerSR.sprite = null;
                break;
            case MarkerType.White:
                markerSR.sprite = whiteSprite;
                markerSR.color = Color.white; // 투명도 초기화
                DropAnimation();
                break;
            case MarkerType.Black:
                markerSR.sprite = blackSprite;
                markerSR.color = Color.white; // 투명도 초기화
                DropAnimation();
                break;
            case MarkerType.Forbidden: // 금수 case 추가 - 이재현
                markerSR.sprite = forbiddenSprite;
                markerSR.color = Color.white;
                break;
            case MarkerType.Preview:
                markerSR.sprite = previewSprite;
                markerSR.color = Color.white; // 투명도 초기화
                SelectCursorAnim();
                break;

        }
    }

    // 마커 상태 전달
    public MarkerType GetMarker() => currentMarker;

    // 터치 처리
    private void OnMouseDown()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) // UI 클릭 무시
            return;

        omokController.OnCellClicked(row, col);
    }

    // 착수 애니메이션
    private void DropAnimation()
    {
        // 시작 상태
        transform.localScale = Vector3.zero;
        transform.rotation = Quaternion.identity;

        Sequence seq = DOTween.Sequence(); // Sequence: Tweener 여러개 제어

        // 커짐
        seq.Append(transform.DOScale(Vector3.one * 1f, 0.8f).SetEase(Ease.OutBack));
        
        // 크기 원래대로 돌아옴
        seq.Append(transform.DOScale(Vector3.one * 0.5f, 0.9f).SetEase(Ease.OutBounce));

        // 전체 시간동안 5번 회전
        transform.DORotate(new Vector3(0, 1800, 1800), 1.7f, RotateMode.FastBeyond360).SetEase(Ease.OutQuint);
    }

    // 임시 선택 애니메이션
    private void SelectCursorAnim()
    {       
        if (markerSR.sprite == previewSprite)
        {
            markerSR.DOFade(0.3f, 0.3f).SetLoops(-1, LoopType.Yoyo);

            transform.localScale = Vector3.one * 0.5f;
            transform.DOScale(Vector3.one * 0.6f, 0.3f).SetLoops(-1, LoopType.Yoyo);
        }       
    }
}
