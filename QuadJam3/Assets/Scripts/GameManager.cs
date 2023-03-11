using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    bool gameHasEnded = false;
    public float inGameTime;
    [SerializeField]  float gameTimeToRealTime = 15;      // # of real seconds = 1 year in game
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
        if(isInGame)
        {
            inGameTime += Time.deltaTime;
            if(HUDEventsManager.EventsHUD != null)
            {
                HUDEventsManager.EventsHUD.OnTimeUpdate(inGameTime * gameTimeToRealTime);
            }
            
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
