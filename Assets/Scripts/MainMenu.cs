using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public Button startGame, options, quitGame;
    public List<MaskableGraphic> menuItems;
    public List<MaskableGraphic> playerUI;
    public Transform playerFist;
    public float menuFadeSpeed;
    public float rotationSpeed;
    public Animator weaponAnimator;
    public Player pl;

    private bool started = false;
    private Transform playerCam;
    
    private void StartGame()
    {
        startGame.interactable = false;
        options.interactable = false;
        quitGame.interactable = false;
        Time.timeScale = 1;
        started = true;
    }
    
    private void Options()
    {
        // TODO: options menu
    }
    
    private void QuitGame()
    {
        // And saving should also be done here
        Application.Quit();
    }
    
    private void Awake()
    {
        playerCam = pl.playerCam.transform;
        pl.playerCam.enabled = false;
        pl.playerMelee.enabled = false;
        pl.enabled = false;
        weaponAnimator.enabled = false;
        foreach (MaskableGraphic playerUIComponent in playerUI)
            playerUIComponent.color = new Color(playerUIComponent.color.r, playerUIComponent.color.g, playerUIComponent.color.b, 0);
        playerFist.position -= Vector3.up * 0.3f;
        startGame.onClick.AddListener(StartGame);
        options.onClick.AddListener(Options);
        quitGame.onClick.AddListener(QuitGame);
        Time.timeScale = 0;
    }

    private void Update()
    {
        if (started)
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
            gameObject.SetActive(false);
        }
        else
            playerCam.Rotate(Vector3.up * rotationSpeed);
    }
}
