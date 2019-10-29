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
    float stateChangeTime = -2, toggleUITime = -1;
    bool addLabel = true;
    Vector3 UIPos;
    Quaternion UIRot;
    void Start()
    {
        DebugUIBuilder.instance.AddLabel("Emergency!");
        if (PlayerPrefs.GetInt("HideUI") != 1)
        {
            DebugUIBuilder.instance.Show();
            PlayerPrefs.SetInt("HideUI", 0);
        }
        else
        {
            DebugUIBuilder.instance.Show();
            UIPos = DebugUIBuilder.instance.transform.localPosition;
            DebugUIBuilder.instance.Hide();
        }
        pinScript = pin.GetComponent<PullPin>();
        handleScript = handle.GetComponent<Handle>();
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.Get(OVRInput.Button.One) && toggleUITime - Time.time <= -1)
        {
            toggleUITime = Time.time;
            if(PlayerPrefs.GetInt("HideUI") == 0)
            {
                UIPos = DebugUIBuilder.instance.gameObject.GetComponent<RectTransform>().localPosition;
                UIRot = DebugUIBuilder.instance.gameObject.GetComponent<RectTransform>().localRotation;
                PlayerPrefs.SetInt("HideUI", 1);
                DebugUIBuilder.instance.Hide();
            } else
            {
                PlayerPrefs.SetInt("HideUI", 0);
                DebugUIBuilder.instance.Show();
                DebugUIBuilder.instance.gameObject.GetComponent<RectTransform>().localPosition.Set(UIPos.x, UIPos.y, UIPos.z);
                DebugUIBuilder.instance.gameObject.GetComponent<RectTransform>().localRotation.Set(UIRot.x, UIRot.y, UIRot.z, UIRot.w);
            }
            PlayerPrefs.Save();
        }
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
        else if (state == 4 && stateChangeTime - Time.time <= -7)
        {
            addLabel = true;
            state = 5; //Reset the scene.
            stateChangeTime = Time.time;
        }
        else if (state == 5 && stateChangeTime - Time.time <= -7)
        {
            addLabel = true;
            state = 6; //Hide the UI
            stateChangeTime = Time.time;
            PlayerPrefs.SetInt("HideUI", 1);
            PlayerPrefs.Save();
        }
        if (addLabel)
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
                DebugUIBuilder.instance.AddLabel("Pull the pin! (Use the left thumbstick)");
                break;
            case 2:
                DebugUIBuilder.instance.AddLabel("Aim the nozzle!");
                break;
            case 3:
                DebugUIBuilder.instance.AddLabel("Squeeze the handle! (Use the left thumbstick)");
                break;
            case 4:
                DebugUIBuilder.instance.AddLabel("Sweep over the fire!");
                break;
            case 5:
                DebugUIBuilder.instance.AddLabel("Touch the pink cube and press left thumbstick to reset the scene.");
                break;
            case 6:
                DebugUIBuilder.instance.Hide();
                break;
            default:
                DebugUIBuilder.instance.AddLabel("Default");
                break;
        }

    }
}
