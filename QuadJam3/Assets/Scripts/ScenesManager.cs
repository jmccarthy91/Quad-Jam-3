using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScenesManager : MonoBehaviour
{
    public static ScenesManager Instance;

    public string mainMenuSceneName = "MainMenu";
    public string gameSceneName = "Sandbox";
    public string gameOverSceneName = "GameOver";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void LoadMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuSceneName);
    }

    public void LoadGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName);
    }

    public void LoadGameOver()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(gameOverSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }


}
