using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [Tooltip("Whether or not the fire spreads across surfaces.")]
    public bool m_Spreads = true;
    [Tooltip("The maximum distance a new fire may spawn from an existing one.")]
    public float m_MaxSpreadDistance = 1f;
    [Tooltip("The amount of time (in seconds) the fire waits before making a random attempt to spread.")]
    public float m_SpreadTime = 5f;
    [Tooltip("How much flame's spread time may vary (in seconds).")]
    public float m_SpreadTimeVariation = 2f;
    [Tooltip("How quickly the fire (particles emitted) scales up to full size.")]
    public float m_ScaleSpeed = 0.2f;
    [Tooltip("How quickly the fire goes out when sprayed by an extinguisher.")]
    public float m_ExtinguishSpeed = 0.2f;
    [Tooltip("The maximum amount of fires that may be created from fire spreading. Values less" +
        "than 1 are ignored (default cap at 100). Any value > 0 will change the cap. Changing the cap" +
        "in another fire will overwrite the previous cap.")]
    public int m_FireCap = 0;
    [Tooltip("How significantly the max size of the flame varies.")]
    public float m_MaxSizeVariation = 0.2f;
    [Tooltip("How much flame's growth speed may vary.")]
    public float m_GrowthVariation = 0.5f;

    private float m_TimeAlive = 0;

    public static int g_FireCap = 100;
    public static int g_FireCount = 0;

    // Variables saved for convenience (they're used repeatedly so we save them)
    private Vector3 m_Normal;
    private float m_CastOffset;
    private BoxCollider m_Collider;
    private ParticleSystem[] m_Systems;

    // Variables related to the fire spreading and growing
    private float m_TimeSinceSpread = 0;
    private float m_NextSpreadTime;
    private float m_RelativeScale = 1f;
    private float m_MaxSize = 1f;
    private float m_TrueScaleSpeed = 0.2f; // The actual speed used, after random variation

    private void Start()
    {
        // Initialize the fire
        m_Systems = GetComponentsInChildren<ParticleSystem>();
        m_NextSpreadTime = m_SpreadTime; // Set the next time for the fire to spread to be 
        m_TimeAlive = 0;
        ScaleParticles(0.2f); // Make the flame start smaller and grow into full size
        // Assume the surface normal is "up" for this fire
        m_Normal = gameObject.transform.up;

        // Get the collider and the distance from the collider that we will perform raycasts
        m_Collider = gameObject.GetComponent<BoxCollider>();
        m_CastOffset = (gameObject.transform.position - 
            m_Collider.transform.TransformPoint(m_Collider.center)).magnitude;

        // Change the global fire cap if it was set
        if (m_FireCap > 0)
            g_FireCap = m_FireCap;

        // Randomize the max size
        m_MaxSize = Random.Range(1f - m_MaxSizeVariation, 1f + m_MaxSizeVariation);
        // Randomize the growth speed
        m_TrueScaleSpeed = m_ScaleSpeed * Random.Range(1f - m_GrowthVariation, 1f + m_GrowthVariation);
    }

    private void Update()
    {
        // Update the time alive
        m_TimeAlive += Time.deltaTime;

        // Scale up the fire till it reaches full size
        if (m_RelativeScale < m_MaxSize)
        {
            ScaleParticles(m_RelativeScale * (1f + m_TrueScaleSpeed * Time.deltaTime));
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
        // Make the fire scale down logarithmically
        ScaleParticles(m_RelativeScale * (1f - m_ExtinguishSpeed));

        // If the fire is small enough, destroy it
        if (m_RelativeScale < 0.1f)
            Destroy(gameObject);
    }

    private void Spread()
    {
        // Set the next spread time
        m_NextSpreadTime = Random.Range(m_SpreadTime - m_SpreadTimeVariation, m_SpreadTime + m_SpreadTimeVariation);

        // Don't spread if fire cap was reached
        if (g_FireCount >= g_FireCap)
            return;

        // Don't spread if not yet max size
        if (m_RelativeScale < m_MaxSize)
            return;

        // Cast a random ray to attempt to find a location to spread to
        Vector3 randDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        randDirection.Normalize();

        // If negative, we cast a ray in towards the direction opposite the normal. This allows the flame
        // to spread around outside corners.
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

        // If the raycast hit nothing or another fire, return
        if (!hit || raycastHit.collider.gameObject.CompareTag("Fire"))
            return;

        // Create a new fire pointed with its "up" direction in the same direction as the normal
        g_FireCount++;
        GameObject newFire = Instantiate(gameObject, raycastHit.point + raycastHit.normal * m_CastOffset, 
            Quaternion.LookRotation(Vector3.Cross(randDirection, raycastHit.normal), raycastHit.normal));

        // Ensure the fire does not move
        newFire.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public void ScaleParticles(float scale)
    {
        // Scale each particle system (the starting size and lifetime) to be scale * their initial values
        float scaleFactor = scale / m_RelativeScale;
        m_RelativeScale *= scaleFactor;

        foreach (var system in m_Systems)
        {
            var emission = system.emission;
            system.startSize *= scaleFactor;
            system.startLifetime *= scaleFactor;
        }
    }

    public void OnCollisionEnter(Collision other)
    {
        // If two fires overlap, destroy one
        if (other.gameObject.CompareTag("Fire") && m_TimeAlive >= other.gameObject.GetComponent<Fire>().m_TimeAlive)
        {
            Destroy(other.gameObject);
            g_FireCount--;
        }
    }
}
