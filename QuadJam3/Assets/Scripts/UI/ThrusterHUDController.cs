using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Remove Update and use events for activating instead

public class ThrusterHUDController : MonoBehaviour
{
    public RectTransform thrusterMeter;
    public bool isFilling = false; // Public for testing
    public bool isDraining = false; // Public for testing
    [Range(0f, 1f)]
    public float tankFill;

    void Start()
    {
        // Init Full
        tankFill = 1f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isFilling)
        {
            isDraining = false;
            RefillFuel(1500);
            if (tankFill >= 1f)
            {
                tankFill = 1f;
                isFilling = false;
            }
        }

        if (isDraining)
        {
            isFilling = false;
            EmptyFuel(3000);
            if (tankFill <= 0f)
            {
                tankFill = 0f;
                isDraining = false;
            }
        }
    }

    void Update()
    {
        if(isDraining || isFilling)
        {
          UpdateThrusterBar();
        }
    }

    void UpdateThrusterBar()
    {
        thrusterMeter.localScale = new Vector3(tankFill, 1f, 1f);
    }
    
    void EmptyFuel(int timeMs)
    {
        // Calculate how much fuel to remove to empty the tank from 1 to 0 in n milliseconds inside FixedUpdate
        tankFill -= ( 1f / (timeMs / (Time.fixedDeltaTime * 1000)) );
    }

    void RefillFuel(int timeMs)
    {
        // Calculate how much fuel to add to fill the tank from 0 to 1 in n milliseconds inside FixedUpdate
        tankFill += ( 1f / (timeMs / (Time.fixedDeltaTime * 1000)) );
    }

}
