using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtinguisherScript : MonoBehaviour
{
    public ParticleSystem fireRetardent;
    public List<ParticleCollisionEvent> collisionEvents;

    // Start is called before the first frame update
    void Start()
    {
        fireRetardent = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnParticleCollision(GameObject other)
    {
        int nCollisionEvents = fireRetardent.GetCollisionEvents(other, collisionEvents);

        if (!other.CompareTag("Fire"))
            return;

        other.GetComponent<Fire>().Extinguish();
    }
}
