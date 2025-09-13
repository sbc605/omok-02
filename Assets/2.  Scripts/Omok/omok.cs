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

    // �ʱ�ȭ(��ǥ�� ��Ʈ�ѷ� ���)
    public void InitMarker(int r, int c, OmokController controller)
    {
        row = r;
        col = c;
        omokController = controller;
        SetMarker(MarkerType.None);
    }

    // ��Ŀ ����
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

    // ��Ŀ ���� ����
    public MarkerType GetMarker() => currentMarker;

    // ��ġ ó��
    private void OnMouseDown()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) // UI Ŭ�� ����
            return;

        omokController.OnCellClicked(row, col);
    }
}
