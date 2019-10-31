using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAudioSource : MonoBehaviour
{
    public List<AudioClip> m_SmallFireClips;
    public List<AudioClip> m_MediumFireClips;
    public List<AudioClip> m_LargeFireClips;
    public List<AudioClip> m_RagingFireClips;

    private List<AudioClip> m_FireClips;
    private List<bool> m_FireSourcesPlaying;

    public int m_SmallThreshold = 1;
    public int m_MediumThreshold = 4;
    public int m_LargeThreshold = 10;
    public int m_RagingThreshold = 50;

    public float m_VolumeChangeSpeed = 1f;

    // There should only be one audio source for the fire
    private static FireAudioSource g_LevelAudioSource;

    private int m_FireCount = 0;

    private AudioSource[] m_AudioSources;

    // Start is called before the first frame update
    void Start()
    {
        g_LevelAudioSource = this;
        transform.position = Vector3.zero;

        m_FireClips = new List<AudioClip>();
        m_FireClips.AddRange(m_SmallFireClips);
        m_FireClips.AddRange(m_MediumFireClips);
        m_FireClips.AddRange(m_LargeFireClips);
        m_FireClips.AddRange(m_RagingFireClips);

        m_FireSourcesPlaying = new List<bool>(m_FireClips.Count);
        foreach (AudioClip clip in m_FireClips)
        {
            m_FireSourcesPlaying.Add(false);
        }

        m_AudioSources = gameObject.GetComponents<AudioSource>();
        int sourceCount = 0;
        foreach (AudioSource source in m_AudioSources)
        {
            source.loop = true;
            source.clip = m_FireClips[sourceCount];
            source.Play();
        }

        ChangeAudio();
    }

    private void OnDestroy()
    {
        g_LevelAudioSource = null;
    }

    // Update is called once per frame
    void Update()
    {
        int sourceCount = 0;
        foreach (AudioSource source in m_AudioSources)
        {
            if (source.volume < 1f && m_FireSourcesPlaying[sourceCount])
            {
                float volume = source.volume + m_VolumeChangeSpeed * Time.deltaTime;
                if (volume > 1f)
                    volume = 1f;

                source.volume = volume;
            }
            else if (source.isPlaying && !m_FireSourcesPlaying[sourceCount])
            {
                float volume = source.volume - m_VolumeChangeSpeed * Time.deltaTime;
                if (volume <= 0f)
                {
                    source.Pause();
                }
                else
                {
                    source.volume = volume;
                }
            }
        }
    }

    private void PlaySounds(int clipsToPlay)
    {
        int sourceCount = 0;
        foreach (AudioSource source in m_AudioSources)
        {
            if (sourceCount >= clipsToPlay)
            {
                m_FireSourcesPlaying[sourceCount] = false;
                sourceCount++;
                continue;
            }

            if (!source.isPlaying)
            {
                source.UnPause();
                source.volume = 0f;
                m_FireSourcesPlaying[sourceCount] = true;
            }
            sourceCount++;
        }

        while (sourceCount < clipsToPlay)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = m_FireClips[sourceCount];
            source.loop = true;
            source.volume = 0f;
            source.Play();
            m_FireSourcesPlaying[sourceCount] = true;
            sourceCount++;
        }

        m_AudioSources = gameObject.GetComponents<AudioSource>();
    }

    private void ChangeAudio()
    {
        if (m_FireCount >= m_RagingThreshold)
        {
            PlaySounds(m_RagingFireClips.Count + m_LargeFireClips.Count + m_MediumFireClips.Count + m_SmallFireClips.Count);
        }
        else if (m_FireCount >= m_LargeThreshold)
        {
            PlaySounds(m_LargeFireClips.Count + m_MediumFireClips.Count + m_SmallFireClips.Count);
        }
        else if (m_FireCount >= m_MediumThreshold)
        {
            PlaySounds(m_MediumFireClips.Count + m_SmallFireClips.Count);
        }
        else if (m_FireCount >= m_SmallThreshold)
        {
            PlaySounds(m_SmallFireClips.Count);
        }
        else
        {
            AudioSource[] AudioSources = gameObject.GetComponents<AudioSource>();
            foreach (AudioSource source in AudioSources)
            {
                source.Pause();
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
            if (m_FireCount <= 0 || g_LevelAudioSource == null)
            {
                return;
            }

            transform.position -= position / m_FireCount;
            float temp = ((float)(m_FireCount - 1) / m_FireCount);

            if (temp == 0f)
            {
                transform.position = Vector3.zero;
                m_FireCount = 0;
                ChangeAudio();
                return;
            }

            float factor = 1f / temp;
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
