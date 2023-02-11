using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEditorInternal;
using UnityEngine;

public class Player : MonoBehaviour
{

    public static Player m;
    public string MoveType = "stop";
    public LayerMask groundLayer;

    [Header("Script References:")][Space]
    public PlayerCam playerCam;
    public CrouchLogic crouchLogic;

    [Header("Other References:")]
    public Camera MainCamera;

    void Awake()
    {
        m = this;

        crouchLogic = GetComponent<CrouchLogic>();
    }

}
