using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public bool m_Spreads = true;
    public float m_SpreadRadius = 1f;
    public float m_RadiusVariation = 0.1f;
    public float m_SpreadTime = 5f;
    private float m_TimeAlive = 0;
    public float m_TimeToFullSize = 1f;

    public static int g_FireCap = 100;
    public static int g_FireCount = 0;

    private float m_TimeSinceSpread = 0;
    private float m_NextSpreadTime;
    private float m_RelativeScale = 1f;
    public float m_ScaleSpeed = 0.2f;
    public float m_ExtinguishSpeed = 0.2f;
    private ParticleSystem[] m_Systems;

    private void Start()
    {
        m_Systems = GetComponentsInChildren<ParticleSystem>();
        m_NextSpreadTime = m_SpreadTime;
        m_TimeAlive = 0;
        ScaleParticles(0.2f);
    }

    private void Update()
    {
        m_TimeAlive += Time.deltaTime;

        if (m_RelativeScale < 1f)
        {
            ScaleParticles(1f + m_ScaleSpeed * Time.deltaTime);
        }

        m_TimeSinceSpread += Time.deltaTime;
        if (m_TimeSinceSpread >= m_NextSpreadTime)
        {
            m_TimeSinceSpread = 0;
            if (m_Spreads)
            {
                Spread();
            }
        }
    }

    public void Extinguish()
    {
        ScaleParticles(1f - m_ExtinguishSpeed);

        if (m_RelativeScale < 0.1f)
            Destroy(gameObject);
    }

    private void Spread()
    {
        if (g_FireCount >= g_FireCap)
            return;

        if (m_RelativeScale < 1f)
            return;

        g_FireCount++;
        GameObject newFire = Instantiate(gameObject);
        Fire fireScript = newFire.GetComponent<Fire>();
        fireScript.m_RelativeScale = m_RelativeScale;
        Vector3 randOffset = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
        randOffset.Normalize();
        float randRadius = m_SpreadRadius + m_SpreadRadius * Random.Range(-m_RadiusVariation, m_RadiusVariation);
        newFire.transform.position = gameObject.transform.position + randOffset * randRadius;
        m_NextSpreadTime = m_SpreadTime;

        newFire.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public void ScaleParticles(float scale)
    {
        m_RelativeScale *= scale;

        foreach (var system in m_Systems)
        {
            var emission = system.emission;
            system.startSize *= scale;
            system.startLifetime *= scale;
        }
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Fire") && m_TimeAlive > other.gameObject.GetComponent<Fire>().m_TimeAlive)
        {
            Destroy(other.gameObject);
            g_FireCount--;
        }
    }
}
