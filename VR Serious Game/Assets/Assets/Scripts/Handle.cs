using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handle : MonoBehaviour
{
    private ParticleSystem foamParticle;
    private bool isShooting;
    private Animator anim;
    private AudioSource [] sounds;
    // Start is called before the first frame update
    void Start()
    {
        sounds = gameObject.GetComponentsInParent<AudioSource>();
        foamParticle = transform.parent.Find("Particle_Foam").gameObject.GetComponent<ParticleSystem>();
        anim = GetComponent<Animator>();
        isShooting = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (OVRInput.Get(OVRInput.Button.PrimaryThumbstick) && PullPin.pinReleased)
        {
            isShooting = true;
            foamParticle.Play(true);
            anim.SetBool("squeezed", true);
            foreach (AudioSource sound in sounds)
            {
                if(!sound.isPlaying)
                {
                    sound.Play();
                }
            }
        }
        if (!OVRInput.Get(OVRInput.Button.PrimaryThumbstick) && isShooting)
        {
            isShooting = false;
            foamParticle.Stop(true);
            anim.SetBool("squeezed", false);
            foreach (AudioSource sound in sounds)
            {
                sound.Stop();
            }
        }
    }
}
