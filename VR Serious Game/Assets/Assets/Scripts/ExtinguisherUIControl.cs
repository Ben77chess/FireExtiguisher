using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtinguisherUIControl : MonoBehaviour
{
    // Start is called before the first frame update
    private int state = 0;
    public GameObject handle, pin;
    PullPin pinScript;
    Handle handleScript;
    float stateChangeTime = -2; 
    void Start()
    {
        DebugUIBuilder.instance.AddLabel("Label");
        pinScript = pin.GetComponent<PullPin>();
        handleScript = handle.GetComponent<Handle>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pinScript.Extinguisher.isGrabbed && state == 0)
        {
            state = 1; //Pull the pin
        }
        else if (pinScript.detached && state == 1)
        {
            state = 2; //Aim the nozzle
            stateChangeTime = Time.time;

        }
        else if (state == 2 && stateChangeTime - Time.time <= - 2)
        {
            state = 3; //Squeeze the handle!
            stateChangeTime = Time.time;
        }
        else if (state == 3 && stateChangeTime - Time.time <= -2)
        {
            state = 4; //Sweep the fire!
            stateChangeTime = Time.time;
        }
    }

    private void setUIText()
    {
        switch (state) {
            case 0:
                DebugUIBuilder.instance.AddLabel("Pick up the fire extinguisher.");
                break;
            case 1:
                DebugUIBuilder.instance.AddLabel("Pull the pin!.");
                break;
            case 2:
                DebugUIBuilder.instance.AddLabel("Aim the nozzle!.");
                break;
            case 3:
                DebugUIBuilder.instance.AddLabel("Squeeze the handle!");
                break;
            case 4:
                DebugUIBuilder.instance.AddLabel("Sweep over the fire!");
                break;
            default:
                DebugUIBuilder.instance.AddLabel("Default");
                break;
        }

    }
}
