using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class Timer : MonoBehaviour
{
    private float time = 10;
    private float currTime = 0;

    [SerializeField] Image timeBar;
    private float originWidth;

    public event Action OnTimeOver;

    void Start()
    {
        originWidth = timeBar.rectTransform.sizeDelta.x;
        ResetTimer();
    }

    void Update()
    {
        TimePass();
    }

    void TimePass()
    {
        if (currTime > 0)
        {
            currTime -= Time.deltaTime;
            if (currTime <= 0)
            {
                currTime = 0;
                OnTimeOver?.Invoke();
            }
        }

        // 0 ~ 1 비율 계산
        float ratio = Mathf.Clamp01(currTime / time);
        timeBar.rectTransform.sizeDelta = new Vector2(originWidth * ratio, timeBar.rectTransform.sizeDelta.y);
    }

    public void ResetTimer()
    {
        currTime = time;
        timeBar.rectTransform.sizeDelta = new Vector2(originWidth, timeBar.rectTransform.sizeDelta.y);
    }
    
    // 타이머 정지 (게임 종료 시)
    public void StopTimer()
    {
        this.enabled = false; // Update 함수를 멈추는 가장 간단한 방법
    }
}
