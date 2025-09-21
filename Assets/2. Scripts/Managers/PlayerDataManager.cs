using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDataManager : Singleton<PlayerDataManager>
{
    private const string FavorabilitySaveKey = "PlayerFavorability";

    [Header("디버그 옵션")]
    [Tooltip("체크하고 게임을 시작하면, 아래 '초기 호감도' 값으로 덮어쓰고 저장합니다.")]
    [SerializeField] private bool overrideFavorability = false;
    [SerializeField] private int initialFavorability = 100;

    public int Favorability { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        LoadData();
    }

    public void AddFavorability(int amount)
    {
        Favorability += amount;
        Debug.Log($"호감도 {amount} 증가! 현재 호감도: {Favorability}");
        SaveData();
    }

    public bool DecreaseFavorability(int amount, bool doSave = true)
    {
        if (Favorability < amount)
        {
            Debug.Log("호감도가 부족합니다.");
            return false;
        }

        Favorability -= amount;
        Debug.Log($"호감도 {amount} 감소. 현재 호감도: {Favorability}");
        
        if (doSave)
        {
            SaveData();
        }
        return true;
    }

    private void LoadData()
    {
        // ▼▼▼ Inspector의 덮어쓰기 옵션을 확인하는 로직 추가 ▼▼▼
        if (overrideFavorability)
        {
            Favorability = initialFavorability;
            SaveData(); // 덮어쓴 값을 파일에도 즉시 저장
            Debug.LogWarning($"[디버그] 호감도를 Inspector 값({Favorability})으로 강제 덮어썼습니다.");
        }
        else
        {
            // 평소에는 저장된 값을 불러옵니다.
            Favorability = PlayerPrefs.GetInt(FavorabilitySaveKey, initialFavorability);
            Debug.Log($"데이터 불러오기 완료. 현재 호감도: {Favorability}");
        }
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt(FavorabilitySaveKey, Favorability);
        PlayerPrefs.Save();
        Debug.Log("데이터 저장 완료.");
    }

    protected override void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        // 내용은 비워둡니다.
    }
}