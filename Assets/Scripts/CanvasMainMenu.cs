using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class CanvasMainMenu : MonoBehaviour
{
    [SerializeField] private GameManagerSO gameManagerSO;

    [SerializeField] private GameObject panelMainMenu;
    [SerializeField] private GameObject panelSettingsMenu;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Slider sliderVolumeMusic;
    [SerializeField] private Slider sliderVolumeSFX;

    private void Start()
    {
        panelMainMenu.SetActive(true);
        panelSettingsMenu.SetActive(false);

        // set sliders values
        audioMixer.GetFloat("MusicVolume", out float volumeMusic);
        sliderVolumeMusic.value = volumeMusic;        
        audioMixer.GetFloat("SFXvolume", out float volumeSFX);
        sliderVolumeSFX.value = volumeSFX;
    }

    public void ToSettingsMenu()
    {
        panelMainMenu.SetActive(false);
        panelSettingsMenu.SetActive(true);
    }

    public void ToMainMenu()
    {
        panelMainMenu.SetActive(true);
        panelSettingsMenu.SetActive(false);
    }

    public void SetNewVolumeMusic(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
    }

    public void SetNewVolumeSFX(float volume)
    {
        audioSource.Stop();
        audioMixer.SetFloat("SFXvolume", volume);
        audioSource.Play();
    }

    public void SetQualityLevel(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        Debug.Log("Quality=>" + QualitySettings.GetQualityLevel());
    }
}
