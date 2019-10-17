using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    private ParticleSystem[] m_Systems;

    private void Start()
    {
        m_Systems = GetComponentsInChildren<ParticleSystem>();
    }


    public void Extinguish()
    {
        foreach (var system in m_Systems)
        {
            var emission = system.emission;
            emission.enabled = false;
        }
    }
}
