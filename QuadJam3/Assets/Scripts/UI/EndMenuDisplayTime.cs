using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndMenuDisplayTime : MonoBehaviour
{
    void Start()
    {
        float time = GameManager.Instance.inGameTime;
        int day = (int)(time) % 30;
        int month = ((int)(time) / 30) % 12;
        int year = (int)(time) / 365;        

        if (year <= 50)
        {
            transform.GetComponent<Text>().text = "You made it home after only " + year + " years and " + month + " months.\nCongratulations, your wife is still alive!";
        }
        else
        {
            transform.GetComponent<Text>().text = "You made it home after only " + year + " years and " + month + " months.\nYour wife has died of old age...";
        }
    }
}
