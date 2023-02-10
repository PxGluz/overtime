using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEditorInternal;
using UnityEngine;

public class Player : MonoBehaviour
{

    public static Player m;


    [Header("Script References")][Space]
    public PlayerCam playerCam;

    void Awake()
    {
        m = this;
    }

}
