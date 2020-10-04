using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsWindow : MonoBehaviour
{
    [SerializeField] private GameObject _container = null;
    [SerializeField] private Slider _volumeSlider = null;
    [SerializeField] private AudioMixer _audioMixer = null;
    [SerializeField] private Toggle _vsyncToggle = null;

    private void Awake()
    {
        _volumeSlider.value = PlayerPrefs.GetFloat("Volume");
        _vsyncToggle.isOn = PlayerPrefs.GetInt("VSync") == 1;

        _volumeSlider.onValueChanged.AddListener(SetVolume);
        _vsyncToggle.onValueChanged.AddListener(SetVSync);

        SetVolume(_volumeSlider.value);
        SetVSync(_vsyncToggle.isOn);
    }

    public void Toggle()
    {
        if (_container.activeSelf)
        {
            _container.SetActive(false);
        }
        else
        {
            _container.SetActive(true);
        }
    }

    public void SetVolume(float fraction)
    {
        PlayerPrefs.SetFloat("Volume", fraction);
        _audioMixer.SetFloat("Volume", Mathf.Log10(fraction) * 20);
    }

    public void SetVSync(bool value)
    {
        PlayerPrefs.SetInt("VSync", value ? 1 : 0);
        QualitySettings.vSyncCount = value ? 1 : 0;
    }
}
