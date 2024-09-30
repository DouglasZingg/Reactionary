using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetVolumn : MonoBehaviour
{
    [SerializeField] AudioMixer a_audioMixer;

    [SerializeField] Slider s_musicSlider;
    [SerializeField] Slider s_sfxSlider;
    [SerializeField] Slider s_masterSlider;

    [SerializeField] float f_musicSliderValue;
    [SerializeField] float f_sfxSliderValue;
    [SerializeField] float f_masterSliderValue;

    void Start()
    {
        s_musicSlider.value = PlayerPrefs.GetFloat("music", f_musicSliderValue);
        s_sfxSlider.value = PlayerPrefs.GetFloat("sfx", f_sfxSliderValue);
        s_masterSlider.value = PlayerPrefs.GetFloat("master", f_masterSliderValue);
    }

    void Update()
    {
        PlayerPrefs.SetFloat("music", s_musicSlider.value);
        PlayerPrefs.SetFloat("sfx", s_sfxSlider.value);
        PlayerPrefs.SetFloat("master", s_masterSlider.value);
    }

    public void SetMasterVolumn(float sliderValue)
    {
        a_audioMixer.SetFloat("Master", Mathf.Log10(sliderValue) * 20);
    }

    public void SetMusicVolumn(float sliderValue)
    {
        a_audioMixer.SetFloat("Music", (Mathf.Log10(sliderValue) * 20));
    }

    public void SetSFXVolumn(float sliderValue)
    {
        a_audioMixer.SetFloat("SFX", (Mathf.Log10(sliderValue) * 20));
    }
}
