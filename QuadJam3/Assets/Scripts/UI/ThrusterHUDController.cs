using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrusterHUDController : MonoBehaviour
{
    public RectTransform thrusterMeter;
    [Range(0f, 1f)]
    public float tankFill;
    [SerializeField] private bool isFilling = false;
    [SerializeField] private bool isDraining = false;
    private float maxFlightTime;
    private float refillTime;

    void Start()
    {
        HUDEventsManager.EventsHUD.onJetpackStarted += OnJetpackStarted;
        HUDEventsManager.EventsHUD.onJetpackEnded += OnJetpackEnded;
        // Init Full
        tankFill = 1f;
    }

    void FixedUpdate()
    {
        if (isFilling)
        {
            isDraining = false;
            RefillFuel(maxFlightTime * 1000); // I had it 1500 miliseconds. Check values
            if (tankFill >= 1f)
            {
                tankFill = 1f;
                isFilling = false;
            }
        }
        else if (isDraining)
        {
            isFilling = false;
            DrainFuel(refillTime);
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

    void DrainFuel(int timeMs)
    {
        tankFill -= ( 1f / (timeMs / (Time.fixedDeltaTime * 1000)) );
    }

    void RefillFuel(int timeMs)
    {
        tankFill += ( 1f / (timeMs / (Time.fixedDeltaTime * 1000)) );
    }

    void OnJetpackStarted(float maxFlightTime)
    {
        isFilling = false;
        this.maxFlightTime = maxFlightTime;
        isDraining = true;
    }

    void OnJetpackEnded(float refillTime = 3.0f)
    {
        isDraining = false;
        this.refillTime = refillTime;
        isFilling = true;
    }

    void OnDestroy()
    {
        HUDEventsManager.EventsHUD.onJetpackStarted -= OnJetpackStarted;
        HUDEventsManager.EventsHUD.onJetpackEnded -= OnJetpackEnded;
    }

}
