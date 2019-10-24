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
    bool addLabel = true;
    void Start()
    {
        DebugUIBuilder.instance.AddLabel("Emergency!");
        DebugUIBuilder.instance.Show();
        pinScript = pin.GetComponent<PullPin>();
        handleScript = handle.GetComponent<Handle>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!pinScript)
        {
            pinScript = pin.GetComponent<PullPin>();
        }
        if (!handleScript)
        {
            handleScript = handle.GetComponent<Handle>();
        }
        if (pinScript.Extinguisher.isGrabbed && state == 0)
        {
            addLabel = true;
            state = 1; //Pull the pin
            stateChangeTime = Time.time;
        }
        else if (pinScript.detached && state == 1 && stateChangeTime - Time.time <= -2)
        {
            addLabel = true;
            state = 2; //Aim the nozzle
            stateChangeTime = Time.time;

        }
        else if (state == 2 && stateChangeTime - Time.time <= - 3)
        {
            addLabel = true;
            state = 3; //Squeeze the handle!
            stateChangeTime = Time.time;
        }
        else if (state == 3 && stateChangeTime - Time.time <= -3)
        {
            addLabel = true;
            state = 4; //Sweep the fire!
            stateChangeTime = Time.time;
        }
        if(addLabel)
        {
            setUIText();
            addLabel = false;
        }
        
    }

    private void setUIText()
    {
        switch (state) {
            case 0:
                DebugUIBuilder.instance.AddLabel("Pick up the fire extinguisher.");
                break;
            case 1:
                DebugUIBuilder.instance.AddLabel("Pull the pin!");
                break;
            case 2:
                DebugUIBuilder.instance.AddLabel("Aim the nozzle!");
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
