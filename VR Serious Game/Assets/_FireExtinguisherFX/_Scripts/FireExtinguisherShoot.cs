using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireExtinguisherShoot : MonoBehaviour
{
    private ParticleSystem mParticle;
    private bool isPlaying;
    // Start is called before the first frame update
    void Start()
    {
        if (mParticle == null)
        {
            mParticle = this.GetComponent<ParticleSystem>();
            mParticle.Stop(true);
        }

        isPlaying = false;
    }

    // Update is called once per frame
    void Update()
    {
        OnFireShooting();
    }

    void OnFireShooting()
    {
        if (Input.GetKey(KeyCode.F))
        {
            if (isPlaying == false)
            {
                isPlaying = true;
                mParticle.Play(true);
            }
        }
        
        if (Input.GetKeyUp(KeyCode.F))
        {
            isPlaying = false;
            mParticle.Stop(true);
        }
    }
}
