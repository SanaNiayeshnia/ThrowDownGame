using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public AudioClip backgroundMusicClip; 

    private AudioSource audioSource;
    private static MusicManager instance; 
    void Awake()
    {
      
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource != null && backgroundMusicClip != null)
        {
            if (!audioSource.isPlaying) 
            {
                audioSource.clip = backgroundMusicClip;
                audioSource.loop = true; 
                audioSource.Play();
            }
        }
        else
        {
            Debug.LogWarning("AudioSource یا backgroundMusicClip در صحنه موجود نیست!");
        }
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("BackgroundMusicVolume", volume);
    }

    public void SetBackgroundMusic(AudioClip newClip)
    {
        if (audioSource != null && newClip != null)
        {
            audioSource.clip = newClip;
            audioSource.Play(); 
        }
    }
}
