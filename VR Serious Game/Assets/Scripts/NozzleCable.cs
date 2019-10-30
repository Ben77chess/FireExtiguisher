using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NozzleCable : MonoBehaviour
{
    [SerializeField] private GameObject cablePrefab;
    [SerializeField] private GameObject start;
    [SerializeField] private GameObject end;
    [SerializeField] private float segmentLength;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 direction = end.transform.position - start.transform.position;
        int count = Mathf.FloorToInt(direction.magnitude / segmentLength);
        for (int i = 0; i<= count; i++)
        {
            Vector3 nextPosition = start.transform.position + direction.normalized * segmentLength * i;
            GameObject nextCable = Instantiate(cablePrefab, nextPosition, Quaternion.LookRotation(direction, Vector3.up), this.transform);
            if (i == 0)
            {
                nextCable.AddComponent<FixedJoint>();
                nextCable.GetComponent<FixedJoint>().connectedBody = start.GetComponent<Rigidbody>();
            }
            else
            {
                nextCable.AddComponent<HingeJoint>();
                nextCable.GetComponent<HingeJoint>().connectedBody = transform.GetChild(i - 1).GetComponent<Rigidbody>();
                nextCable.GetComponent<HingeJoint>().autoConfigureConnectedAnchor = false;
                nextCable.GetComponent<HingeJoint>().connectedAnchor = new Vector3(0, 0, 0);
                JointSpring hingeSpring = nextCable.GetComponent<HingeJoint>().spring;
                hingeSpring.damper = 50;
            }
            if (i == count)
            {
                nextCable.AddComponent<FixedJoint>();
                nextCable.GetComponent<FixedJoint>().connectedBody = end.GetComponent<Rigidbody>();
                end.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
