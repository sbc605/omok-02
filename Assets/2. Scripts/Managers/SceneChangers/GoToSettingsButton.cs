using UnityEngine;
using UnityEngine.UI; 

// 이 스크립트는 Button 컴포넌트가 있는 오브젝트에만 붙일 수 있도록 강제합니다.
[RequireComponent(typeof(Button))]
public class GoToSettingsButton : MonoBehaviour
{
    void Start()
    {
        // 1. 이 스크립트가 붙어있는 게임 오브젝트의 Button 컴포넌트를 가져옵니다.
        Button button = GetComponent<Button>();

        // 2. 버튼의 onClick 이벤트에 리스너(실행할 함수)를 코드로 추가합니다.
        button.onClick.AddListener(LoadSettingsScene);
    }

    // 3. GameManager의 함수를 대신 호출해 줄 함수입니다.
    void LoadSettingsScene()
    {
        GameManager.Instance.LoadSettingScene();
    }
}