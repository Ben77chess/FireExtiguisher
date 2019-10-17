using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extingusher : MonoBehaviour
{
    float collisionTime = 0;
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
            collisionTime = Time.time;
            Destroy(gameObject);
        }
    }

    void OnCollisionStay(Collision target)
    {
        if (target.gameObject.tag.Equals("Pin") == true)
        {
            if (Time.time - collisionTime > 2)
            {
                Destroy(target.gameObject);
            }
        }
    }
}
