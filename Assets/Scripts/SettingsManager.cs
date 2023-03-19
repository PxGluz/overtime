using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("Settings")]
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

    private void Awake()
    {
        // Get all possible resolutions for current screen and add them to options.
        Resolution[] possibleResolutions = Screen.resolutions;
        resolutionDropdown.options = new List<TMP_Dropdown.OptionData>();
        int currentResolution = 0;
        for (int i = 0; i < possibleResolutions.Length; i++)
        {
            if (possibleResolutions[i].height == Screen.height && possibleResolutions[i].width == Screen.width)
                currentResolution = i;

            resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(possibleResolutions[i].width.ToString() + " x " + possibleResolutions[i].height.ToString()));
        }
        resolutionDropdown.value = currentResolution;

        // Setting default values.
        fullScreenMode = Screen.fullScreenMode;
        resolution = new Vector2(Screen.width, Screen.height);
        subtitles = true;
        sensitivity = 1f;
    }

    private void Start()
    {
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
        Player.m.playerCam.sensitivity = 1f;
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
}
