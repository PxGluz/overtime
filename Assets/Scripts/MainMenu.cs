using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button start, exit;
    [SerializeField] private String sceneName;
    
    
    private void StartScene()
    {
        SceneManager.LoadScene(sceneName);
    }

    private void ExitScene()
    {
        // Maybe we need more things to do before quitting
        Application.Quit();
    }
    
    private void Start()
    {
        start.onClick.AddListener(StartScene);
        exit.onClick.AddListener(ExitScene);
    }
}
