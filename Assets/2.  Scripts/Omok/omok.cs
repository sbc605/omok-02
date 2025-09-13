using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SpriteRenderer))]
public class Omok : MonoBehaviour
{
    [SerializeField] private Sprite whiteSprite;
    [SerializeField] private Sprite blackSprite;
    [SerializeField] private SpriteRenderer markerSR;

    public enum MarkerType { None, White, Black }
    private MarkerType currentMarker = MarkerType.None;

    private int row, col;
    private OmokController omokController;

    private void Awake()
    {
        markerSR = GetComponent<SpriteRenderer>();
    }

    // 초기화(좌표와 컨트롤러 등록)
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

        switch (marker)
        {
            case MarkerType.None:
                markerSR.sprite = null;
                break;
            case MarkerType.White:
                markerSR.sprite = whiteSprite;
                break;
            case MarkerType.Black:
                markerSR.sprite = blackSprite;
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
}
