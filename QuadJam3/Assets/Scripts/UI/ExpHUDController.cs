using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ExpHUDController : MonoBehaviour
{
    public RectTransform expMeter;

    [Range(0f, 1f)]
    public float expFill;
    public float expExponent = 1.5f;
    private float currentExp = 0.0f;

    void Start()
    {
        HUDEventsManager.EventsHUD.onExpereinceChange += OnExpereinceChange;
        // Init Full
        expFill = 0f;
    }
    
    void OnExpereinceChange(int newExp, int maxExpLvl)
    {
        AddExp(newExp, maxExpLvl);
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
            InGameUIManager.Instance.OnModalUpgradeOpen();
            int newMaxExp = Mathf.RoundToInt(maxExp * expExponent);
            expFill = excedent / newMaxExp;
        }
    }

    private void OnDestroy()
    {
        HUDEventsManager.EventsHUD.onExpereinceChange -= OnExpereinceChange;
    }
}