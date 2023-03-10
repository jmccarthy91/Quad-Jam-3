using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndMenuDisplayTime : MonoBehaviour
{
    void Start()
    {
        float time = GameManager.Instance.inGameTime * 80;
        int year = (int)(time) / 365;        

        if (year <= 50)
        {
            transform.GetComponent<Text>().text = "You made it home after just " + year + " years. Congratulations, your wife is still alive!";
        }
        else
        {
            transform.GetComponent<Text>().text = "You made it home after " + year + " long years. Your wife has died of old age...";
        }
    }
}
