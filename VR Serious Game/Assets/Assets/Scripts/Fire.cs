using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public bool m_Spreads = true;
    public float m_MaxSpreadDistance = 1f;
    public float m_MinSpreadDistance = 0.7f;
    public float m_RadiusVariation = 0.1f;
    public float m_SpreadTime = 5f;
    private float m_TimeAlive = 0;
    public float m_TimeToFullSize = 1f;

    public static int g_FireCap = 100;
    public static int g_FireCount = 0;

    private Vector3 m_Normal;
    private float m_CastOffset;
    private BoxCollider m_Collider;

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
        m_Normal = new Vector3(0f, 1f, 0f);
        m_Collider = gameObject.GetComponent<BoxCollider>();
        m_CastOffset = (gameObject.transform.position - 
            m_Collider.transform.TransformPoint(m_Collider.center)).magnitude;
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
        m_NextSpreadTime = m_SpreadTime;

        if (g_FireCount >= g_FireCap)
            return;

        if (m_RelativeScale < 1f)
            return;

        Vector3 randDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        randDirection.Normalize();

        bool Negative = Random.Range(0, 2) < 1;
        float castOffset = m_CastOffset;
        if (Negative)
            castOffset *= -1f;

        Vector3 castStart = m_Collider.transform.TransformPoint(m_Collider.center) + castOffset * m_Normal;
        if (Negative)
        {
            castStart += randDirection * m_MaxSpreadDistance;
            randDirection *= -1f;
        }

        RaycastHit raycastHit;
        bool hit = Physics.Raycast(castStart, randDirection, out raycastHit, m_MaxSpreadDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal);

        if (!hit || raycastHit.collider.gameObject.CompareTag("Fire"))
            return;

        g_FireCount++;
        GameObject newFire = Instantiate(gameObject, raycastHit.point + raycastHit.normal * m_CastOffset, 
            Quaternion.LookRotation(Vector3.Cross(new Vector3(1f, 0f, 0f), raycastHit.normal), raycastHit.normal) /*new Quaternion(0f, 1f, 0f, Random.Range(-1f, 1f))*/);

        Fire fireScript = newFire.GetComponent<Fire>();
        fireScript.m_RelativeScale = m_RelativeScale;
        fireScript.m_Normal = raycastHit.normal;

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
