using UnityEngine;

public class BlockController : MonoBehaviour
{
    [Header("Board Layout")]
    [SerializeField] private float tileSize = 1f;    // 타일 하나 간격
    [SerializeField] private float offsetX = 0f;     // 보드 원점 X
    [SerializeField] private float offsetY = 0f;     // 보드 원점 Y

    public Vector3 GetWorldPosition(int row, int col)
    {
        float x = col * tileSize + offsetX;
        float y = row * tileSize + offsetY;
        return new Vector3(x, y, 0f);
    }

    //  월드 좌표(마우스 클릭 위치)를 그리드 좌표로 변환하는 함수 추가 ▼▼▼
    public Vector2Int GetBoardPosition(Vector3 worldPosition)
    {
        // 오프셋을 빼서 원점 기준으로 맞춘 뒤, 타일 크기로 나누어 그리드 상의 위치를 계산
        int col = Mathf.RoundToInt((worldPosition.x - offsetX) / tileSize);
        int row = Mathf.RoundToInt((worldPosition.y - offsetY) / tileSize);

        return new Vector2Int(row, col);
    }
}