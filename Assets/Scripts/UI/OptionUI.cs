using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionUI : UIBase
{
    [Header("Sound Settings")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;
    
    [Header("Resolution Settings")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    
    private Resolution[] resolutions;
    private float currentBGMVolume = 1f;
    private float currentSEVolume = 1f;

    [SerializeField] AudioClip testSound;

    public override void Initialization(UIData data)
    {
        base.data = data;
        InitializeResolutionDropdown();
        InitializeSoundSliders();
    }

    private void InitializeResolutionDropdown()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        var options = new System.Collections.Generic.List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = $"{resolutions[i].width} x {resolutions[i].height}";
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    private void InitializeSoundSliders()
    {
        if (bgmSlider != null)
        {
            bgmSlider.value = currentBGMVolume;
            bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        }

        if (seSlider != null)
        {
            seSlider.value = currentSEVolume;
            seSlider.onValueChanged.AddListener(OnSEVolumeChanged);
        }
    }

    public override void Show(UIData _data)
    {
        base.data = _data;
        contents.SetActive(true);
        isShow = true;
    }

    public override void Hide()
    {
        contents.SetActive(false);
        isShow = false;
        data?.onHide?.Invoke();
    }

    private void OnBGMVolumeChanged(float value)
    {
        currentBGMVolume = value;
        SoundManager.instance.SetBGMVolume(value);
    }

    private void OnSEVolumeChanged(float value)
    {
        currentSEVolume = value;
        SoundManager.instance.SetSEVolume(value);
        SoundManager.instance.PlaySE(testSound, false);
    }

    public void OnResolutionChanged(int index)
    {
        Resolution resolution = resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
} 