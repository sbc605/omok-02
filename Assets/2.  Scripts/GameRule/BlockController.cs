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
}