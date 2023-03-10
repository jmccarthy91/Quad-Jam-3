using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    bool gameHasEnded = false;
    public float inGameTime;
    float gameTimeToRealTime = 525600;      // # of minutes in a year, 1 minute of real time = 1 year of in game time

    private bool isInGame = false;

    private void Awake()
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

    private void OnDestroy() {
        if (Instance == this) {
            Instance = null;
        }
    }

    public void EndGame()
    {
        if(gameHasEnded == false)
        {   
            isInGame = false;
            gameHasEnded = true;
            Debug.Log("GAME OVER");
            ScenesManager.LoadGameOver();
        }
    }

    void Update()
    {   
      Debug.Log(isInGame);
        if(isInGame)
        {
            inGameTime += Time.time * gameTimeToRealTime;
            HUDEventsManager.EventsHUD.OnTimeUpdate(inGameTime);
            Debug.Log(inGameTime);
        }
    }

    public void Restart()
    {   
        gameHasEnded = false;
        inGameTime = 0.0f;
        isInGame = true;
    }
}
