using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDataManager : Singleton<PlayerDataManager>
{
    private const string FavorabilitySaveKey = "PlayerFavorability";

    public int Favorability { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        // 게임이 처음 시작될 때 저장된 데이터를 불러옵니다.
        LoadData();
    }

    // 호감도를 증가시키는 함수
    public void AddFavorability(int amount)
    {
        Favorability += amount;
        Debug.Log($"호감도 {amount} 증가! 현재 호감도: {Favorability}");
        // 호감도가 변경되었으므로 즉시 저장합니다.
        SaveData();
    }

    // 호감도를 감소시키는 함수 (저장 여부를 선택할 수 있도록 수정)
    public bool DecreaseFavorability(int amount, bool doSave = true)
    {
        if (Favorability < amount)
        {
            Debug.Log("호감도가 부족합니다.");
            return false;
        }

        Favorability -= amount;
        Debug.Log($"호감도 {amount} 감소. 현재 호감도: {Favorability}");
        
        // doSave가 true일 때만 파일에 저장합니다.
        if (doSave)
        {
            SaveData();
        }
        return true;
    }

    // 디바이스에서 데이터를 불러오는 함수
    private void LoadData()
    {
        // "PlayerFavorability"라는 키로 저장된 정수 값을 불러옵니다.
        // 만약 저장된 값이 없다면(최초 실행 시), 기본값으로 100을 사용합니다.
        Favorability = PlayerPrefs.GetInt(FavorabilitySaveKey, 100);
        Debug.Log($"데이터 불러오기 완료. 현재 호감도: {Favorability}");
    }

    // 디바이스에 데이터를 저장하는 함수
    private void SaveData()
    {
        // 현재 Favorability 값을 "PlayerFavorability" 키로 저장합니다.
        PlayerPrefs.SetInt(FavorabilitySaveKey, Favorability);
        // 변경사항을 디스크에 즉시 기록하도록 요청합니다 (안전장치).
        PlayerPrefs.Save();
        Debug.Log("데이터 저장 완료.");
    }

    // (이전에 추가했던 OnSceneLoad 함수는 그대로 유지)
    protected override void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        // PlayerDataManager는 씬이 바뀔 때 특별히 할 일이 없으므로 비워둡니다.
    }
}