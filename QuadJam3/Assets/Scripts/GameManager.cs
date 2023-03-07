using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    bool gameHasEnded = false;
    public float inGameTime;
    float gameTimeToRealTime = 525600;      // # of minutes in a year, 1 minute of real time = 1 year of in game time

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
        SceneManager.LoadScene("Sandbox");               //may need to rename this or move to scene manager
    }
}
