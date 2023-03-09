using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    bool gameHasEnded = false;
    public float inGameTime;
    float gameTimeToRealTime = 525600;      // # of minutes in a year, 1 minute of real time = 1 year of in game time

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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
            gameHasEnded = true;
            Debug.Log("GAME OVER");
            //show game over screen
        }
    }

    public void Update()
    {
        inGameTime = Time.time * gameTimeToRealTime;
    }

    void Restart()
    {    
        inGameTime = 0.0f;
        SceneManager.LoadScene("Sandbox"); //may need to rename this or move to scene manager
    }
}
