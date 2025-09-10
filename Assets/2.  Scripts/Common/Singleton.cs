using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class Singleton<T> : MonoBehaviour where T : Component
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<T>();

                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    instance = obj.AddComponent<T>();
                }
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
            // 씬 전환시 호출되는 액션 메서드 할당
            SceneManager.sceneLoaded += OnSceneLoad;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected abstract void OnSceneLoad(Scene scene, LoadSceneMode mode);
}
