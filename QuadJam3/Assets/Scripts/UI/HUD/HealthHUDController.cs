using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Remove Update and use event instead

public class HealthHUDController : MonoBehaviour
{
    public RectTransform heathBar;
    public int startingHealth = 5;
    private RectTransform[] healthUnits;

    void Start()
    {
        healthUnits = heathBar.GetComponentsInChildren<RectTransform>(true);

        HUDEventsManager.EventsHUD.onHealthChange += OnHealthChange;

        UpdateHealthBar(startingHealth);
    }
    
    void OnHealthChange(int currentHealth)
    {
        UpdateHealthBar(currentHealth);
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

    void OnDestroy() {
        HUDEventsManager.EventsHUD.onHealthChange -= OnHealthChange;
    }
}
