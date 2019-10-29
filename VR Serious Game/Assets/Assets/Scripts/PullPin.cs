using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullPin : MonoBehaviour
{
    public OVRGrabbable Extinguisher;
    protected GameObject Grabber;
    public bool detached = false;
    public static bool pinReleased = false;
    // Start is called before the first frame update
    void Start()
    {
        Extinguisher = transform.parent.Find("Body").gameObject.GetComponent<OVRGrabbable>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (OVRInput.Get(OVRInput.Button.PrimaryThumbstick)  && detached && !pinReleased)
        //{
        //    transform.position = Grabber.transform.position;
        //    transform.rotation = Grabber.transform.rotation;
        //}
        if (!OVRInput.Get(OVRInput.Button.PrimaryThumbstick) && detached)
        {
            pinReleased = true;
            Destroy(this.gameObject.GetComponent<FixedJoint>());
            //this.gameObject.SetActive(false);
        }
    }

    void OnTriggerStay(Collider otherCollider)
    {
        if (!detached && Extinguisher.isGrabbed && OVRInput.Get(OVRInput.Button.PrimaryThumbstick))
        {
            this.transform.parent = null;
            Grabber = otherCollider.gameObject;
            //Grabber.GetComponentInParent<Rigidbody>();
            gameObject.GetComponent<FixedJoint>().connectedBody = Grabber.GetComponentInParent<Rigidbody>();
            //Destroy(this.gameObject.GetComponent<FixedJoint>());
            detached = true;
        }
    }
}
