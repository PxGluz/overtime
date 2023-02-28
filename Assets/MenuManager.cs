using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    
    public GameObject LoseScreen;
    public GameObject WinScreen;
    
    // WinMenu|LoseMenu
    public void OpenMenu(string name)
    {
        CloseAllMenus();
        Player.m.playerCam.UnLockCursor();

        switch(name)
        {
            case "WinMenu":
                WinScreen.SetActive(true);
                break;

            case "LoseMenu":
                LoseScreen.SetActive(true);
                break;

            default:
                print("No menu was opened...");
                break;
        }
    }

    public void CloseAllMenus() 
    {
        Player.m.playerCam.LockCursor();

        LoseScreen.SetActive(false);
        WinScreen.SetActive(false);
    }

    public void RestartScene()
    {
        print("restart game");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        print("Quit game");
        Application.Quit();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            OpenMenu("WinMenu");
        }
        if (Input.GetKeyDown(KeyCode.M)) {
            OpenMenu("LoseMenu");
        }

        if (Input.GetKeyDown(KeyCode.B)) {
            CloseAllMenus();
        }
    }


}
