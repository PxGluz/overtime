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
    public PlayerMovement playerMovement;
    public PlayerShooting playerShooting;

    [Header("Other References:")]
    public Camera MainCamera;

    [Header("Important variables:")]
    public string MoveType = "stop"; // current move types: stop, walk, run, crouch, slide
    public string AttackType = "none"; // current attack types: none, shoot, melee   | future attack types: throw
    public LayerMask groundLayer;
    public LayerMask enemyLayer;
    //public LayerMask objectsAffectedByExplosions;
    public GameObject PointDebug;

    [Header("Stats:")]
    public float MaxHealth;
    public float currentHealth;


    void Awake()
    {
        m = this;

        crouchLogic = GetComponent<CrouchLogic>();
        //weaponManager = GetComponent<WeaponManager>();
        playerThrow = GetComponent<PlayerThrow>();
        playerMovement = GetComponent<PlayerMovement>();
        playerShooting = GetComponent<PlayerShooting>();
        //playerMelee = GetComponent<PlayerMelee>();
    }

    private void Start()
    {
        currentHealth = MaxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth < 0)
        {
            Die();
        }

    }

    public void Die()
    {
        print("The player has died");
    }

}
