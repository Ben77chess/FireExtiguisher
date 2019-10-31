using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nozzle : MonoBehaviour
{
    private OVRGrabbable nozzleGrabbable;
    private GameObject Grabber;
    private Vector3 initPositon;
    private Quaternion initRotation;
    // Start is called before the first frame update
    void Start()
    {
        nozzleGrabbable = gameObject.GetComponent<OVRGrabbable>();
        initPositon = transform.localPosition;
        initRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (nozzleGrabbable.isGrabbed && Grabber != null)
        {
            gameObject.GetComponent<FixedJoint>().connectedBody = Grabber.GetComponent<Rigidbody>();
        }
        else
        {
            transform.localPosition = initPositon;
            transform.localRotation = initRotation;
            gameObject.GetComponent<FixedJoint>().connectedBody = transform.parent.Find("Body").GetComponent<Rigidbody>();
        }
    }

    void OnTriggerStay(Collider otherCollider)
    {
        if (otherCollider.gameObject.GetComponentInParent<OVRGrabber>())
        {
            Grabber = otherCollider.transform.parent.gameObject;
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
