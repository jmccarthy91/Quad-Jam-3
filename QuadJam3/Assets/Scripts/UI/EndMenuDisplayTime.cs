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
        int month = ((int)(time) / 30);
        int year = (int)(time) / 365;        

        transform.GetComponent<Text>().text = "You got home after " + month + " months or " + year + " years.";
    }
}
