using UnityEngine;

public class ButtonStart : MonoBehaviour
{
    public void StartGame()
    {
        // GameManager.Instance.Restart();
        ScenesManager.LoadGame();
    }
}
