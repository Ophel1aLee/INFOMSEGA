using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    private AudioSource m_bgmSource;

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        // Add AudioSource component if not exists
        m_bgmSource = GetComponent<AudioSource>();
        if (m_bgmSource == null)
        {
            m_bgmSource = gameObject.AddComponent<AudioSource>();
        }

        // Configure AudioSource for BGM
        m_bgmSource.loop = true;
        m_bgmSource.playOnAwake = true;
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip != null && m_bgmSource.clip != clip)
        {
            m_bgmSource.clip = clip;
            m_bgmSource.Play();
        }
    }

    public void StopBGM()
    {
        m_bgmSource.Stop();
    }

    public void SetBGMVolume(float volume)
    {
        m_bgmSource.volume = Mathf.Clamp01(volume);
    }
} 