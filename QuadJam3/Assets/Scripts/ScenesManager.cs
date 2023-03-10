using UnityEngine;

public class ScenesManager : MonoBehaviour
{
    public static ScenesManager Instance;

    public const string mainMenuSceneName = "MainMenu";
    public const string gameSceneName = "Main";
    public const string gameOverSceneName = "EndMenu";

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

    public static void LoadMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuSceneName);
    }

    public static void LoadGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName);
        GameManager.Instance.Restart();
    }

    public static void LoadGameOver()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(gameOverSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
