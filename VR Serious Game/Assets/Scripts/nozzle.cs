using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nozzle : MonoBehaviour
{
    private OVRGrabbable Extinguisher;
    private GameObject Grabber;
    private Vector3 initPositon;
    private Quaternion initRotation;
    private bool isHeld;
    // Start is called before the first frame update
    void Start()
    {
        Extinguisher = transform.parent.gameObject.GetComponent<OVRGrabbable>();
        initPositon = transform.localPosition;
        initRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponentInParent<OVRGrabber>()  )
        {
            Grabber = other.gameObject;
        }
    }

    void OnTriggerStay(Collider otherCollider)
    {
        if (OVRInput.Get(OVRInput.Button.PrimaryThumbstick) && Extinguisher.isGrabbed && Grabber != null && !isHeld)
        {
            gameObject.AddComponent<Rigidbody>();
            gameObject.AddComponent<FixedJoint>();
            gameObject.GetComponent<FixedJoint>().connectedBody = Grabber.GetComponentInParent<Rigidbody>();
            isHeld = true;
        }
        if (!OVRInput.Get(OVRInput.Button.PrimaryThumbstick) && isHeld)
        {
            transform.localPosition = initPositon;
            transform.localRotation = initRotation;
            Destroy(gameObject.GetComponent<FixedJoint>());
            Destroy(gameObject.GetComponent<Rigidbody>());
            isHeld = false;
        }
    }

    void OnTriggerExit(Collider otherCollider)
    {
        if (otherCollider.gameObject.GetComponent<OVRGrabber>())
        {
            Grabber = null;
        }
    }
}
