using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class omok : MonoBehaviour
{
    [SerializeField] private Sprite whiteSprite;
    [SerializeField] private Sprite blackSprite;

    public enum BlockColor { None, White, Black }

    private int omokIndex;
}
