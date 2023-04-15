using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [System.Serializable]
    public class SettingsData
    {
        public float masterVolume, musicVolume, sfxVolume, dialogueVolume;
        public int fullScreenMode;
        public int resolutionX, resolutionY;
        public bool subtitles;
        public float sensitivity;

        public SettingsData(float masterVolume, float musicVolume, float sfxVolume, float dialogueVolume, FullScreenMode fullScreenMode, Vector2 resolution, bool subtitles, float sensitivity)
        {
            this.masterVolume = masterVolume;
            this.musicVolume = musicVolume;
            this.sfxVolume = sfxVolume;
            this.dialogueVolume = dialogueVolume;
            this.fullScreenMode = (int)fullScreenMode;
            resolutionX = (int)resolution.x;
            resolutionY = (int)resolution.y;
            this.subtitles = subtitles;
            this.sensitivity = sensitivity;
        }
    }

    [Header("Settings")]
    public float masterVolume, musicVolume, sfxVolume, dialogueVolume;
    public FullScreenMode fullScreenMode;
    public Vector2 resolution;
    public bool subtitles;
    public float sensitivity;
    [Header("Other")]
    public TMP_Dropdown resolutionDropdown;
    public GameObject menu;
    public GameObject togglePrefab;
    [Tooltip("For toggles")]
    public GameObject startPosition;
    public MainMenu mainMenu;

    private void Start()
    {
        // Get all possible resolutions for current screen and add them to options.
        resolution = new Vector2(Screen.width, Screen.height);
        Resolution[] possibleResolutions = Screen.resolutions;
        print(possibleResolutions.ToString());
        resolutionDropdown.options = new List<TMP_Dropdown.OptionData>();
        int currentResolution = 0;
        for (int i = 0; i < possibleResolutions.Length; i++)
        {
            if (possibleResolutions[i].height == resolution.y && possibleResolutions[i].width == resolution.x)
                currentResolution = i;

            resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(possibleResolutions[i].width.ToString() + " x " + possibleResolutions[i].height.ToString()));
        }
        resolutionDropdown.value = currentResolution;

        object data = SerializationManager.Load("settingsFile");
        if (data == null)
        {
            SetMasterVolume(20f);
            mainMenu.dictionarySliders["masterVolume"].value = 20f;
            SetMusicVolume(20f);
            mainMenu.dictionarySliders["musicVolume"].value = 20f;
            SetSFXVolume(20f);
            mainMenu.dictionarySliders["sfxVolume"].value = 20f;
            SetDialogueVolume(20f);
            mainMenu.dictionarySliders["dialogueVolume"].value = 20f;
            SetFullScreenMode(Screen.fullScreenMode);
            SetResolution(Screen.width, Screen.height);
            SetSubtitles(true);
            mainMenu.dictionaryToggles["subtitles"].isOn = true;
            SetSensitivity(1f);
            mainMenu.dictionarySliders["sensitivity"].value = 1f;
        }
        else
        {
            SettingsData settingsData = data as SettingsData;
            UpdateSettings(settingsData);
        }

        int offset = 0;
        foreach (VolumeComponent component in Player.m.volume.components)
        {
            GameObject toggle = Instantiate(togglePrefab, startPosition.transform.position + new Vector3(/*300*/0, /*280 */- 40 * offset, 0), new Quaternion(), menu.transform);
            toggle.GetComponentInChildren<TextMeshProUGUI>().text = component.name;
            toggle.SetActive(false);
            Toggle currentToggle = toggle.GetComponent<Toggle>();
            currentToggle.isOn = component.active;
            currentToggle.onValueChanged.AddListener(delegate { mainMenu.VolumeEffect(component.name); });
            mainMenu.dictionaryToggles.Add(component.name, currentToggle);
            offset++;
        }
    }

    public void SetFullScreenMode(FullScreenMode fullScreenMode)
    {
        this.fullScreenMode = fullScreenMode;
        Screen.fullScreenMode = fullScreenMode;
    }

    public void SetResolution(int width, int height)
    {
        resolution = new Vector2(width, height);
        Screen.SetResolution(width, height, fullScreenMode);
    }

    public void SetSubtitles (bool subtitles)
    {
        this.subtitles = subtitles;
    }

    public void SetSensitivity(float sensitivity)
    {
        this.sensitivity = sensitivity;
        Player.m.playerCam.sensitivity = sensitivity;
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        AudioManager.AM.audioMixer.SetFloat("master", volume);
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        AudioManager.AM.audioMixer.SetFloat("music", volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        AudioManager.AM.audioMixer.SetFloat("soundEffects", volume);
    }

    public void SetDialogueVolume(float volume)
    {
        dialogueVolume = volume;
        AudioManager.AM.audioMixer.SetFloat("dialogue", volume);
    }

    public SettingsData SettingsToData()
    {
        return new SettingsData(masterVolume, musicVolume, sfxVolume, dialogueVolume, fullScreenMode, resolution, subtitles, sensitivity);
    }

    public void UpdateSettings(SettingsData data)
    {
        SetMasterVolume(data.masterVolume);
        mainMenu.dictionarySliders["masterVolume"].value = data.masterVolume;
        SetMusicVolume(data.musicVolume);
        mainMenu.dictionarySliders["musicVolume"].value = data.musicVolume;
        SetSFXVolume(data.sfxVolume);
        mainMenu.dictionarySliders["sfxVolume"].value = data.sfxVolume;
        SetDialogueVolume(data.dialogueVolume);
        mainMenu.dictionarySliders["dialogueVolume"].value = data.dialogueVolume;
        SetFullScreenMode((FullScreenMode)data.fullScreenMode);
        mainMenu.dictionaryDropdowns["fullscreen"].value = FullscreenModeToDropdownValue((FullScreenMode)data.fullScreenMode);
        SetResolution(data.resolutionX, data.resolutionY);
        mainMenu.dictionaryDropdowns["resolution"].value = ResolutionToDropdownValue(data.resolutionX, data.resolutionY);
        SetSubtitles(data.subtitles);
        mainMenu.dictionaryToggles["subtitles"].isOn = data.subtitles;
        SetSensitivity(data.sensitivity);
        mainMenu.dictionarySliders["sensitivity"].value = data.sensitivity;
    }

    private int FullscreenModeToDropdownValue(FullScreenMode fullScreenMode)
    {
        switch (fullScreenMode)
        {
            case FullScreenMode.ExclusiveFullScreen:
                return 0;
            case FullScreenMode.FullScreenWindow:
                return 1;
            case FullScreenMode.Windowed:
                return 2;
            default:
                return 1;
        }
    }

    private int ResolutionToDropdownValue(int x, int y)
    {
        int currentIndex = 0;
        for (int i = 0; i < mainMenu.dictionaryDropdowns["resolution"].options.Count; i++)
        {
            TMP_Dropdown.OptionData currentOption = mainMenu.dictionaryDropdowns["resolution"].options[i];
            string[] resString = currentOption.text.Split(' ');
            if (int.Parse(resString[0]) == x && int.Parse(resString[2]) == y)
            {
                currentIndex = i;
                break;
            }
        }

        return currentIndex;
    }
}
