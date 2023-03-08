using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Remove Update and use event instead

public class HealthHUDController : MonoBehaviour
{
    public RectTransform heathBar;
    private RectTransform[] healthUnits;

    // For testing 
    public PlayerController playerController;

    void Start()
    {
        healthUnits = heathBar.GetComponentsInChildren<RectTransform>(true);

        // Initialize health bar
        UpdateHealthBar(playerController.currentHearts);
    }
    
    
    void Update() // For testing
    {
        UpdateHealthBar(playerController.currentHearts);
    }

    void UpdateHealthBar(float health)
    {
        for (int i = 0; i < healthUnits.Length; i++)
        {
            if (i <= health)
            {
                healthUnits[i].gameObject.SetActive(true);
            }
            else
            {
                healthUnits[i].gameObject.SetActive(false);
            }
        }
    }
}
