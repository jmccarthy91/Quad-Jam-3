using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ModalMainMenu : MonoBehaviour
{

    void Start()
    {
        if(SceneManager.GetActiveScene().name == ScenesManager.mainMenuSceneName){
            MainMenuUIManager.Instance.onMainOpen += OnMainOpen;
            MainMenuUIManager.Instance.onMainClose += OnMainClose;
        }
        else if (SceneManager.GetActiveScene().name == ScenesManager.gameOverSceneName){
            EndGameUIManager.Instance.onMainOpen += OnMainOpen;
            EndGameUIManager.Instance.onMainClose += OnMainClose;
        }
    }

    void OnDestory()
    {
        if(SceneManager.GetActiveScene().name == ScenesManager.mainMenuSceneName){
            MainMenuUIManager.Instance.onMainOpen -= OnMainOpen;
            MainMenuUIManager.Instance.onMainClose -= OnMainClose;
        }
        else if (SceneManager.GetActiveScene().name == ScenesManager.gameOverSceneName){
            EndGameUIManager.Instance.onMainOpen -= OnMainOpen;
            EndGameUIManager.Instance.onMainClose -= OnMainClose;
        }
    }

    void OnMainOpen()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }

    void OnMainClose()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
