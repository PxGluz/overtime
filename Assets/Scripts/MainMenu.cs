using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;

public class MainMenu : MonoBehaviour
{
    // Used to display dictionary elements in inspector.
    [System.Serializable]
    public struct NamedButton
    {
        public string name;
        public Button button;
    }

    public NamedButton[] buttons; // What is modified in inspector
    private Dictionary<string, Button> dictionaryButtons = new Dictionary<string, Button>(); // Actual dictionary

    [System.Serializable]
    public struct NamedSlider
    {
        public string name;
        public Slider slider;
    }

    public NamedSlider[] sliders;
    private Dictionary<string, Slider> dictionarySliders = new Dictionary<string, Slider>();

    [System.Serializable]
    public struct NamedDropdown
    {
        public string name;
        public TMP_Dropdown dropdown;
    }

    public NamedDropdown[] dropdowns;
    private Dictionary<string, TMP_Dropdown> dictionaryDropdowns = new Dictionary<string, TMP_Dropdown>();

    [System.Serializable]
    public struct NamedToggle
    {
        public string name;
        public Toggle toggle;
    }

    public NamedToggle[] toggles;
    public Dictionary<string, Toggle> dictionaryToggles = new Dictionary<string, Toggle>();

    // public Button startGame, options, quitGame;
    public List<MaskableGraphic> menuItems;
    public List<MaskableGraphic> playerUI;
    public Transform playerFist;
    public float menuFadeSpeed;
    public float rotationSpeed;
    public Animator weaponAnimator;
    public Player pl;

    private bool startedTransition = false, firstIteration = true, appearing;
    private Transform playerCam;
    private int optionLevel = 0;
    
    private void StartGame()
    {
        foreach (KeyValuePair<string, Button> button in dictionaryButtons)
        {
            button.Value.interactable = false;
        }

        Time.timeScale = 1;
        startedTransition = true;
        Invoke(nameof(ResetEscape), 1);
    }

    private void SetSettings(bool active)
    {
        dictionaryButtons["soundSettings"].gameObject.SetActive(active);
        dictionaryButtons["generalSettings"].gameObject.SetActive(active);
        dictionaryButtons["graphicsSettings"].gameObject.SetActive(active);
    }
    
    private void Options()
    {
        SetSettings(true);
        dictionaryButtons["back"].gameObject.SetActive(true);
        if (firstIteration)
            dictionaryButtons["startGame"].gameObject.SetActive(false);
        else
            dictionaryButtons["resume"].gameObject.SetActive(false);
        dictionaryButtons["options"].gameObject.SetActive(false);
        dictionaryButtons["quitGame"].gameObject.SetActive(false);
        optionLevel++;
    }
    
    private void QuitGame()
    {
        // And saving should also be done here
        Application.Quit();
    }

    private void Back()
    {
        if (optionLevel == 1)
        {
            if (firstIteration)
                dictionaryButtons["startGame"].gameObject.SetActive(true);
            else
                dictionaryButtons["resume"].gameObject.SetActive(true);
            dictionaryButtons["options"].gameObject.SetActive(true);
            dictionaryButtons["quitGame"].gameObject.SetActive(true);
            SetSettings(false);
            dictionaryButtons["back"].gameObject.SetActive(false);
            optionLevel--;
        } else if (optionLevel == 2)
        {
            foreach (KeyValuePair<string, Slider> slider in dictionarySliders)
                slider.Value.gameObject.SetActive(false);
            foreach (KeyValuePair<string, TMP_Dropdown> dropdown in dictionaryDropdowns)
                dropdown.Value.gameObject.SetActive(false);
            foreach (KeyValuePair<string, Toggle> toggle in dictionaryToggles)
                toggle.Value.gameObject.SetActive(false);

            SetSettings(true);
            optionLevel--;
        }
    }

    private void SoundSettings()
    {
        optionLevel++;
        SetSettings(false);
        dictionarySliders["masterVolume"].gameObject.SetActive(true);
    }

    private void GeneralSettings()
    {
        optionLevel++;
        SetSettings(false);
        dictionaryDropdowns["fullscreen"].gameObject.SetActive(true);
        dictionaryDropdowns["resolution"].gameObject.SetActive(true);
        dictionarySliders["sensitivity"].gameObject.SetActive(true);
        dictionaryToggles["subtitles"].gameObject.SetActive(true);
    }

    private void GraphicsSettings()
    {
        optionLevel++;
        SetSettings(false);
        foreach (KeyValuePair<string, Toggle> toggle in dictionaryToggles)
            if (!toggle.Key.Equals("subtitles"))
                toggle.Value.gameObject.SetActive(true);
    }

    private void Fullscreen()
    {
        FullScreenMode fullScreenMode;
        switch (dictionaryDropdowns["fullscreen"].value)
        {
            case 0: fullScreenMode = FullScreenMode.ExclusiveFullScreen; break;
            case 1: fullScreenMode = FullScreenMode.FullScreenWindow; break;
            case 2: fullScreenMode = FullScreenMode.Windowed; break;
            default: fullScreenMode = FullScreenMode.FullScreenWindow; break;
        }
        Player.m.settingsManager.SetFullScreenMode(fullScreenMode);
    }

    private void Resolution()
    {
        string[] resolutionString = dictionaryDropdowns["resolution"].options[dictionaryDropdowns["resolution"].value].text.Split(' ');
        Player.m.settingsManager.SetResolution(int.Parse(resolutionString[0]), int.Parse(resolutionString[2]));
    }

    private void MasterVolume()
    {
        Player.m.settingsManager.SetMasterVolume(dictionarySliders["masterVolume"].value);
    }

    private void Sensitivity()
    {
        Player.m.settingsManager.SetSensitivity(dictionarySliders["sensitivity"].value);
    }

    private void Subtitles()
    {
        Player.m.settingsManager.SetSubtitles(dictionaryToggles["subtitles"].isOn);
    }

    public void VolumeEffect(string component)
    {
        foreach (VolumeComponent volumeComponent in Player.m.volume.components)
            if (volumeComponent.name.Equals(component))
                volumeComponent.active = dictionaryToggles[component].isOn;
    }

    private void Resume()
    {
        foreach (KeyValuePair<string, Button> button in dictionaryButtons)
        {
            button.Value.interactable = false;
        }

        Time.timeScale = 1;
        Player.m.playerCam.LockCursor();
        appearing = false;
        startedTransition = true;
        Invoke(nameof(ResetEscape), 1);
    }
    
    private void Awake()
    {
        foreach (NamedButton button in buttons)
            dictionaryButtons.Add(button.name, button.button);
        foreach (NamedSlider slider in sliders)
            dictionarySliders.Add(slider.name, slider.slider);
        foreach (NamedDropdown dropdown in dropdowns)
            dictionaryDropdowns.Add(dropdown.name, dropdown.dropdown);
        foreach (NamedToggle toggle in toggles)
            dictionaryToggles.Add(toggle.name, toggle.toggle);

        playerCam = pl.playerCam.transform;
        pl.playerCam.enabled = false;
        pl.playerMelee.enabled = false;
        pl.enabled = false;
        weaponAnimator.enabled = false;
        foreach (MaskableGraphic playerUIComponent in playerUI)
            playerUIComponent.color = new Color(playerUIComponent.color.r, playerUIComponent.color.g, playerUIComponent.color.b, 0);
        playerFist.position -= Vector3.up * 0.3f;
        dictionaryButtons["startGame"].onClick.AddListener(StartGame);
        dictionaryButtons["options"].onClick.AddListener(Options);
        dictionaryButtons["quitGame"].onClick.AddListener(QuitGame);
        dictionaryButtons["soundSettings"].onClick.AddListener(SoundSettings);
        dictionaryButtons["generalSettings"].onClick.AddListener(GeneralSettings);
        dictionaryButtons["graphicsSettings"].onClick.AddListener(GraphicsSettings);
        dictionaryButtons["back"].onClick.AddListener(Back);
        dictionaryDropdowns["fullscreen"].onValueChanged.AddListener(delegate { Fullscreen(); });
        dictionaryDropdowns["resolution"].onValueChanged.AddListener(delegate { Resolution(); });
        dictionarySliders["masterVolume"].onValueChanged.AddListener(delegate { MasterVolume(); });
        dictionarySliders["sensitivity"].onValueChanged.AddListener(delegate { Sensitivity(); });
        dictionaryToggles["subtitles"].onValueChanged.AddListener(delegate { Subtitles(); });
        dictionaryButtons["resume"].onClick.AddListener(Resume);
        Time.timeScale = 0;
    }

    private void Update()
    {
        if (firstIteration)
        {
            if (startedTransition)
            {
                foreach (MaskableGraphic playerUIComponent in playerUI)
                    playerUIComponent.color = Color.Lerp(playerUIComponent.color,
                        new Color(playerUIComponent.color.r, playerUIComponent.color.g, playerUIComponent.color.b, 1),
                        menuFadeSpeed);
                foreach (MaskableGraphic menuItem in menuItems)
                    menuItem.color = Color.Lerp(menuItem.color,
                        new Color(menuItem.color.r, menuItem.color.g, menuItem.color.b, 0),
                        menuFadeSpeed);
                playerFist.position = Vector3.Lerp(playerFist.position, playerFist.parent.position, menuFadeSpeed);
                pl.enabled = true;
                pl.playerCam.enabled = true;
                if ((playerFist.position - playerFist.parent.position).magnitude > 0.001f)
                    return;
                foreach (MaskableGraphic playerUIComponent in playerUI)
                    if (1 - playerUIComponent.color.a > 0.001f)
                        return;
                foreach (MaskableGraphic menuItem in menuItems)
                    if (menuItem.color.a > 0.001f)
                        return;
                pl.playerMelee.enabled = true;
                weaponAnimator.enabled = true;
                firstIteration = false;
                startedTransition = false;
                gameObject.SetActive(false);
            }
            else
                playerCam.Rotate(Vector3.up * rotationSpeed);
        } 
        else
        {
            if (startedTransition)
            {
                if (!appearing)
                {
                    foreach (MaskableGraphic playerUIComponent in playerUI)
                        playerUIComponent.color = Color.Lerp(playerUIComponent.color,
                            new Color(playerUIComponent.color.r, playerUIComponent.color.g, playerUIComponent.color.b, 1),
                            menuFadeSpeed);
                    foreach (MaskableGraphic menuItem in menuItems)
                        menuItem.color = Color.Lerp(menuItem.color,
                            new Color(menuItem.color.r, menuItem.color.g, menuItem.color.b, 0),
                            menuFadeSpeed);
                    pl.enabled = true;
                    pl.playerCam.enabled = true;
                    foreach (MaskableGraphic playerUIComponent in playerUI)
                        if (1 - playerUIComponent.color.a > 0.001f)
                            return;
                    foreach (MaskableGraphic menuItem in menuItems)
                        if (menuItem.color.a > 0.001f)
                            return;
                    pl.playerMelee.enabled = true;
                    weaponAnimator.enabled = true;
                    startedTransition = false;
                    gameObject.SetActive(false);
                }
                else
                {
                    foreach (MaskableGraphic playerUIComponent in playerUI)
                        playerUIComponent.color = Color.Lerp(playerUIComponent.color,
                            new Color(playerUIComponent.color.r, playerUIComponent.color.g, playerUIComponent.color.b, 0),
                            menuFadeSpeed);
                    foreach (MaskableGraphic menuItem in menuItems)
                        menuItem.color = Color.Lerp(menuItem.color,
                            new Color(menuItem.color.r, menuItem.color.g, menuItem.color.b, 1),
                            menuFadeSpeed);
                    pl.enabled = false;
                    pl.playerCam.enabled = false;
                    pl.playerMelee.enabled = false;
                    weaponAnimator.enabled = false;
                    foreach (MaskableGraphic playerUIComponent in playerUI)
                        if (playerUIComponent.color.a > 0.001f)
                            return;
                    foreach (MaskableGraphic menuItem in menuItems)
                        if (1 - menuItem.color.a > 0.001f)
                            return;
                    startedTransition = false;
                    Time.timeScale = 0;
                }
            }
        }
    }

    public void PressedEscape()
    {
        gameObject.SetActive(true);
        Player.m.playerCam.UnLockCursor();
        dictionaryButtons["startGame"].gameObject.SetActive(false);
        dictionaryButtons["resume"].gameObject.SetActive(true);
        foreach (KeyValuePair<string, Button> button in dictionaryButtons)
        {
            button.Value.interactable = true;
        }
        startedTransition = true;
        appearing = true;
    }

    private void ResetEscape()
    {
        Player.m.canPressEscape = true;
    }
}
