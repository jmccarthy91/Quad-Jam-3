using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonCredits : MonoBehaviour
{
    private Button btn;
    private string currentScene;
    
    void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);

        //sm = ScenesManager.Instance;
        currentScene = SceneManager.GetActiveScene().name;
    }

    public void OnClick()
    {
        
        if(currentScene == ScenesManager.mainMenuSceneName )
        {
            MainMenuUIManager.Instance.OnCreditsOpen();
            MainMenuUIManager.Instance.OnMainClose();
        }
    }
}
