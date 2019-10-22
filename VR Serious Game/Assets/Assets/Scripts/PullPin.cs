using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullPin : MonoBehaviour
{
    protected OVRGrabbable Extinguisher;
    protected OVRGrabber Grabber;
    private bool detached = false;
    public static bool pinReleased = false;
    // Start is called before the first frame update
    void Start()
    {
        Extinguisher = transform.parent.gameObject.GetComponent<OVRGrabbable>();
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.Get(OVRInput.Button.PrimaryThumbstick)  && detached && Grabber != null && !pinReleased)
        {
            transform.position = Grabber.transform.position;
        }
        if (!OVRInput.Get(OVRInput.Button.PrimaryThumbstick) && detached)
        {
            pinReleased = true;
            this.enabled = false;
        }
    }

    void OnTriggerStay(Collider otherCollider)
    {
        if (OVRInput.Get(OVRInput.Button.PrimaryThumbstick) && Extinguisher.isGrabbed)
        {
            this.transform.parent = null;
            Grabber = otherCollider.GetComponent<OVRGrabber>();
            detached = true;
        }
    }
}
