using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ModalCredits : MonoBehaviour
{
    void Start()
    {
        if(SceneManager.GetActiveScene().name == ScenesManager.mainMenuSceneName){
            MainMenuUIManager.Instance.onCreditsOpen += OnCreditsOpen;
            MainMenuUIManager.Instance.onCreditsClose += OnCreditsClose;
        }
        else if (SceneManager.GetActiveScene().name == ScenesManager.gameOverSceneName){
            EndGameUIManager.Instance.onCreditsOpen += OnCreditsOpen;
            EndGameUIManager.Instance.onCreditsClose += OnCreditsClose;
        }
        
    }

    void OnDestory()
    {
        if(SceneManager.GetActiveScene().name == ScenesManager.mainMenuSceneName){
            MainMenuUIManager.Instance.onCreditsOpen -= OnCreditsOpen;
            MainMenuUIManager.Instance.onCreditsClose -= OnCreditsClose;
        }
        else if (SceneManager.GetActiveScene().name == ScenesManager.gameOverSceneName){
            EndGameUIManager.Instance.onCreditsOpen -= OnCreditsOpen;
            EndGameUIManager.Instance.onCreditsClose -= OnCreditsClose;
        }
    }

    void OnCreditsOpen()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }

    void OnCreditsClose()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
