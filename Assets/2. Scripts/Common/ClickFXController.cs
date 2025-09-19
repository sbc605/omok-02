using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickFXController : MonoBehaviour
{
    [SerializeField] private Image effectPrefab;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Sprite[] frames;

    private SpriteRenderer board; // 오목판
    private Transform resultPanel; // 결과 패널


    void Update()
    {
        ScreenTouch();
    }

    void ScreenTouch()
    {

#if UNITY_EDITOR || UNITY_STANDALONE // 유니티 에디터, PC 마우스 클릭
        if (Input.GetMouseButtonDown(0))
        {
            CreateEffect(Input.mousePosition);
        }
#endif
        if (Input.touchCount > 0) // 모바일 터치
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                CreateEffect(touch.position);
            }
        }
    }

    void CreateEffect(Vector2 pos)
    {
        if (board != null && resultPanel != null)
        {
            // 결과 패널이 비활성화된 상태
            if (!IsResultPanelActive())
            {
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(pos);
                worldPos.z = board.transform.position.z;
                if (board.bounds.Contains(worldPos)) return; // 오목판 내부는 이펙트 생성x
            }
        }

        var effect = Instantiate(effectPrefab, canvas.transform);
        effect.rectTransform.localScale = Vector3.one * 0.5f;

        // 스크린 좌표를 UI 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, pos, canvas.worldCamera, out Vector2 localPos);
        effect.rectTransform.anchoredPosition = localPos;

        StartCoroutine(PlayEffect(effect));
    }

    IEnumerator PlayEffect(Image effect)
    {
        float time = 0.05f; // 각 프레임의 지속 시간
        for (int i = 0; i < frames.Length; i++)
        {
            effect.sprite = frames[i];
            yield return new WaitForSeconds(time);
        }
        Destroy(effect.gameObject);
    }

    private bool IsResultPanelActive()
    {
        if (resultPanel == null) return false;

        foreach (Transform child in resultPanel)
        {
            if (child.gameObject.activeSelf)
                return true;
        }

        return false;
    }

    // 게임씬에 오면 보드, 결과패널 연결
    public void SetGameSceneSprite(SpriteRenderer boardSR, Transform result)
    {
        board = boardSR;
        resultPanel = result;
    }
}
