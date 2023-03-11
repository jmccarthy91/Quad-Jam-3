using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingBtnToEndGame : MonoBehaviour
{
    public void EndGame()
    {
        GameManager.Instance.EndGame();
    }
}
