using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayLevel : MonoBehaviour
{
    public Contract contract;

    // TODO: TEMP SCRIPT
    void Update()
    {
        contract.SetPlanning();
        Player.m.playerCam.UnLockCursor();
        SceneManager.LoadScene(contract.selectedLevel[0].levelScene);
        enabled = false;
    }
}
