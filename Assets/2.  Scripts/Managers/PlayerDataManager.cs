using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDataManager : Singleton<PlayerDataManager>
{
    public int Favorability { get; private set; }

    // 부모의 Awake를 호출하여 싱글톤 및 DontDestroyOnLoad 설정을 실행합니다.
    protected override void Awake()
    {
        base.Awake();
        // TODO: 나중에 여기에 저장된 데이터 불러오는 기능 추가 (3단계)
        LoadData();
    }

    // 호감도를 증가시키는 함수
    public void AddFavorability(int amount)
    {
        Favorability += amount;
        Debug.Log($"호감도 {amount} 증가! 현재 호감도: {Favorability}");
        // TODO: 변경된 값을 저장하는 기능 추가 (3단계)
        SaveData();
    }

    // 호감도를 감소시키는 함수
    public bool DecreaseFavorability(int amount)
    {
        // 가진 호감도가 지불할 양보다 적으면 실패
        if (Favorability < amount)
        {
            Debug.Log("호감도가 부족합니다.");
            // TODO: 부족 알림 UI 띄우기 (3단계)
            return false;
        }

        Favorability -= amount;
        Debug.Log($"호감도 {amount} 감소. 현재 호감도: {Favorability}");
        // TODO: 변경된 값을 저장하는 기능 추가 (3단계)
        SaveData();
        return true;
    }

    // (3단계에서 구현할 저장 및 불러오기 함수들의 뼈대)
    private void SaveData()
    {
        // PlayerPrefs.SetInt("PlayerFavorability", Favorability);
    }

    private void LoadData()
    {
        // Favorability = PlayerPrefs.GetInt("PlayerFavorability", 100); // 예: 기본값 100
    }
    


    // 싱글톤 규칙을 위해서 아래 함수 추가 
    protected override void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        // PlayerDataManager는 씬이 바뀔 때 특별히 할 일이 없으므로,
        // 계약 이행을 위해 내용은 비워둔 채로 함수만 만들어줍니다.
    }
}