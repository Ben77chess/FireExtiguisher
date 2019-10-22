using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extingusher : MonoBehaviour
{
    bool canFire = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnCollisionEnter(Collision target)
    {
        if (target.gameObject.tag.Equals("Pin") == true)
        {
            //collisionTime = Time.time;
        }
    }

    void OnCollisionStay(Collision target)
    {
        canFire = true;
        if (target.gameObject.tag.Equals("Pin") == true)
        {
            canFire = false;
        }
    }
}
