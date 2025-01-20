using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider musicVolumeSlider;
    private MusicManager musicManager; 

    void Start()
    {
        musicManager = FindObjectOfType<MusicManager>();

        if (musicManager != null && musicVolumeSlider != null)
        {
            float currentVolume = 0f;
            musicManager.audioMixer.GetFloat("BackgroundMusicVolume", out currentVolume);
            musicVolumeSlider.value = currentVolume;
        }

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }
    }

    public void SetMusicVolume(float volume)
    {
        if (musicManager != null)
        {
            musicManager.SetMusicVolume(volume);
        }
    }
}
