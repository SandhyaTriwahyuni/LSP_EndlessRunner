using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioMixer AudioMixer;
    public Slider MusicSlider;
    public Slider SfxSlider;

    private void Start()
    {
        LoadVolume();
        Time.timeScale = 1;

        // Memainkan lagu MainMenu
        MusicManager.Instance.PlayMusic("MainMenu");
    }

    public void PlayGame()
    {
        MusicManager.Instance.PlayMusic("GamePlay"); 
        SceneManager.LoadScene("GamePlay");
    }

    public void UpdateMusicVolume(float volume)
    {
        AudioMixer.SetFloat("MusicVol", volume);
    }

    public void UpdateSoundVolume(float volume)
    {
        AudioMixer.SetFloat("SFXVol", volume);
    }

    public void SaveVolume()
    {
        AudioMixer.GetFloat("MusicVol", out float musicVolume);
        PlayerPrefs.SetFloat("MusicVol", musicVolume);

        AudioMixer.GetFloat("SFXVol", out float sfxVolume);
        PlayerPrefs.SetFloat("SFXVol", sfxVolume);
    }

    public void LoadVolume()
    {
        MusicSlider.value = PlayerPrefs.GetFloat("MusicVol");
        SfxSlider.value = PlayerPrefs.GetFloat("SFXVol");
    }

    public void Quit()
    {
        Application.Quit();
    }

}
