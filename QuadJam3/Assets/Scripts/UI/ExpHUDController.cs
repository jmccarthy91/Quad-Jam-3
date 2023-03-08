using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ExpHUDController : MonoBehaviour
{
    public RectTransform expMeter;

    [Range(0f, 1f)]
    public float expFill;

    void Start()
    {
        // Init Full
        expFill = 0f;
    }
    
    // for testing
    void Update()
    {
        UpdateExpBar();
    }

    void UpdateExpBar()
    {
        expMeter.localScale = new Vector3(expFill, 1f, 1f);
    }

    void ResetExpBar()
    {
        expFill = 0f;
    }

    void AddExp(int exp, int maxExp)
    {
        expFill +=  exp / maxExp;

        if (expFill >= 1f)
        {   
            int excedent = Mathf.RoundToInt(expFill * maxExp - maxExp);
            // Trigger level up event?
            int newMaxExp = Mathf.RoundToInt(maxExp * 2); // TODO: Change this
            expFill = excedent / newMaxExp;
        }
    }
}