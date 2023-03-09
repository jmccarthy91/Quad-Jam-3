using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonStart : MonoBehaviour
{
    public void StartGame()
    {
        GameManager.Instance.Restart();
        ScenesManager.Instance.LoadGame();
    }
}
