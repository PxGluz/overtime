using System.Collections;
using System.Collections.Generic;
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
    public MenuManager menuManager;
    public Rewind rewind;

    [Header("Other References:")]
    public Camera MainCamera;
    public Rigidbody playerRigidBody;
    public GameObject PointDebug;
    public GameObject playerObject;
    public Transform headPosition;

    [Header("Important variables:")]
    public string MoveType = "stop"; // current move types: stop, walk, run, crouch, slide
    public string AttackType = "melee"; // current attack types: none, shoot, melee   | future attack types: throw
    public LayerMask groundLayer;
    public LayerMask enemyLayer;

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

        playerRigidBody = GetComponent<Rigidbody>();    
    }

    private void Start()
    {
        currentHealth = MaxHealth;
    }

    public void TakeDamage(float damage)
    {

        print("The player took " + damage + " damage");
        SetPlayerHealth(currentHealth - damage);

        if (currentHealth <= 0)
        {
            Die();
        }

    }

    public void SetPlayerHealth(float health)
    {
        currentHealth = health;
    }

    public void Die()
    {
        gameObject.SetActive(false);
        playerCam.enabled = false;


        Transform[] cameraChildren = playerCam.gameObject.GetComponentsInChildren<Transform>();
       
        for (int i = 1; i < cameraChildren.Length; i++) 
        {
            cameraChildren[i].gameObject.SetActive(false);
        }

        menuManager.OpenMenu("LoseMenu");
        print("The player has died");
    }
    
    public void YouWin()
    {
        gameObject.SetActive(false);
        playerCam.enabled = false;


        Transform[] cameraChildren = playerCam.gameObject.GetComponentsInChildren<Transform>();
       
        for (int i = 1; i < cameraChildren.Length; i++) 
        {
            cameraChildren[i].gameObject.SetActive(false);
        }

        menuManager.OpenMenu("WinMenu");
        print("Won");
    }

}
