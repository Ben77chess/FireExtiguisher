using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAudioSource : MonoBehaviour
{
    public List<AudioClip> m_SmallFireClips;
    public List<AudioClip> m_MediumFireClips;
    public List<AudioClip> m_LargeFireClips;
    public List<AudioClip> m_RagingFireClips;

    public int m_SmallThreshold = 1;
    public int m_MediumThreshold = 4;
    public int m_LargeThreshold = 10;
    public int m_RagingThreshold = 50;

    // There should only be one audio source for the fire
    private static FireAudioSource g_LevelAudioSource;

    private int m_FireCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        g_LevelAudioSource = this;
        transform.position = Vector3.zero;
        AudioSource[] AudioSources = gameObject.GetComponents<AudioSource>();
        foreach (AudioSource source in AudioSources)
        {
            source.loop = true;
        }
        ChangeAudio();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PlaySounds(List<AudioClip> listClips)
    {
        AudioSource[] AudioSources = gameObject.GetComponents<AudioSource>();
        int sourceCount = 0;
        foreach (AudioSource source in AudioSources)
        {
            if (sourceCount >= listClips.Count)
            {
                source.Pause();
                sourceCount++;
                continue;
            }

            source.clip = listClips[sourceCount];
            if (!source.isPlaying)
            {
                source.Play();
            }
            sourceCount++;
        }

        while (sourceCount < listClips.Count)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = listClips[sourceCount];
            source.loop = true;
            source.Play();
            sourceCount++;
        }
    }

    private void ChangeAudio()
    {
        if (m_FireCount >= m_RagingThreshold)
        {
            PlaySounds(m_RagingFireClips);
        }
        else if (m_FireCount >= m_LargeThreshold)
        {
            PlaySounds(m_LargeFireClips);
        }
        else if (m_FireCount >= m_MediumThreshold)
        {
            PlaySounds(m_MediumFireClips);
        }
        else if (m_FireCount >= m_SmallThreshold)
        {
            PlaySounds(m_SmallFireClips);
        }
        else
        {
            AudioSource[] AudioSources = gameObject.GetComponents<AudioSource>();
            foreach (AudioSource source in AudioSources)
            {
                source.Stop();
            }
        }
    }

    public static FireAudioSource GetAudioSource()
    {
        return g_LevelAudioSource;
    }

    public void AddFire(Vector3 position)
    {
        lock (this)
        {
            transform.position = transform.position * m_FireCount / (m_FireCount + 1) + position / (m_FireCount + 1);
            m_FireCount++;

            if (m_FireCount == m_RagingThreshold || m_FireCount == m_LargeThreshold || m_FireCount == m_MediumThreshold || m_FireCount == m_SmallThreshold)
                ChangeAudio();
        }
    }

    public static void AddFireToLevelSource(Vector3 position)
    {
        g_LevelAudioSource.AddFire(position);
    }

    public void RemoveFire(Vector3 position)
    {
        lock (this)
        {
            if (m_FireCount <= 0)
            {
                return;
            }

            transform.position -= position / m_FireCount;
            float factor = 1f / ((float)(m_FireCount - 1) / m_FireCount);
            transform.position *= factor;
            int copy = m_FireCount;
            m_FireCount--;

            if (copy == m_RagingThreshold || copy == m_LargeThreshold || copy == m_MediumThreshold || copy == m_SmallThreshold)
                ChangeAudio();
        }
    }

    public static void RemoveFireFromLevelSource(Vector3 position)
    {
        g_LevelAudioSource.RemoveFire(position);
    }
}
