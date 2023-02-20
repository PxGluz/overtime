using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEditorInternal;
using UnityEngine;

public class Player : MonoBehaviour
{

    public static Player m;

    [Header("Script References:")]
    public PlayerCam playerCam;
    public CrouchLogic crouchLogic;
    public WeaponManager weaponManager;
    public PlayerThrow playerThrow;
    public PlayerMelee playerMelee;

    [Header("Other References:")]
    public Camera MainCamera;

    [Header("Important variables:")]
    public string MoveType = "stop"; // current move types: stop, walk, run, crouch, slide
    public string AttackType = "none"; // current attack types: none, shoot, melee   | future attack types: throw
    public LayerMask groundLayer;
    public LayerMask enemyLayer;
    public LayerMask objectsAffectedByExplosions;


    void Awake()
    {
        m = this;

        crouchLogic = GetComponent<CrouchLogic>();
        //weaponManager = GetComponent<WeaponManager>();
        playerThrow = GetComponent<PlayerThrow>();
        //playerMelee = GetComponent<PlayerMelee>();
    }

}
