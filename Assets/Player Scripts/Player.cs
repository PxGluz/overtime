using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

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
    public MainMenu mainMenu;
    public ColorManager colorManager;
    public AudioManager audioManager;
    public SettingsManager settingsManager;
    public Interact interact;
    public PrefabHolder prefabHolder;
    public ScoringSystem scoringSystem;

    [Header("Other References:")]
    public Camera MainCamera;
    public Rigidbody playerRigidBody;
    public GameObject PointDebug;
    public GameObject playerObject;
    public VolumeProfile volume;
    public Transform bottomPos;
    public Transform topPosition;


    [Header("Important variables:")]
    public string MoveType = "stop"; // current move types: stop, walk, run, crouch, slide
    public string AttackType = "melee"; // current attack types: none, shoot, melee   | future attack types: throw
    public float playerHeight;
    public LayerMask groundLayer;
    public LayerMask enemyLayer;
    public float smoothTime;
    public bool canPressEscape;

    [Header("PostProcessing")] 
    private float vigTarget = 0;
    public float vigIntensity;
    private float bloomTarget = 0;
    public float bloomIntensity;
    private float lensTarget;
    public float lensIntensity;
    
    [Header("Stats:")]
    public float MaxHealth;
    public float currentHealth;

    private float ref1, ref2, ref3, ref4;

    void Awake()
    {
        m = this;

        crouchLogic = GetComponent<CrouchLogic>();
        //weaponManager = GetComponent<WeaponManager>();
        playerThrow = GetComponent<PlayerThrow>();
        playerMovement = GetComponent<PlayerMovement>();
        playerShooting = GetComponent<PlayerShooting>();
        interact = GetComponent<Interact>();
        //playerMelee = GetComponent<PlayerMelee>();

        playerRigidBody = GetComponent<Rigidbody>();

        canPressEscape = !SceneManager.GetActiveScene().name.Equals("MainMenuLobby");
    }

    private void Start()
    {
        currentHealth = MaxHealth;
        playerHeight = topPosition.position.y - bottomPos.position.y;
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
        scoringSystem.combo.transform.parent.gameObject.SetActive(false);
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

    public void Slowing()
    {
        vigTarget = vigIntensity;
        bloomTarget = bloomIntensity;
        lensTarget = lensIntensity;
    }

    public void Fasting()
    {
        vigTarget = 0;
        bloomTarget = 0;
        lensTarget = 0;
    }
    
    void SlowTimeLogic()
    {
        if (volume)
        {
            if (volume.TryGet(out Vignette vig))
                vig.intensity.value = Mathf.SmoothDamp(vig.intensity.value, vigTarget, ref ref2, smoothTime);
            if (volume.TryGet(out Bloom blm))
                blm.intensity.value = Mathf.SmoothDamp(blm.intensity.value, bloomTarget, ref ref3, smoothTime);
            if (volume.TryGet(out LensDistortion lnsD))
                lnsD.intensity.value = Mathf.SmoothDamp(blm.intensity.value, bloomTarget, ref ref4, smoothTime);
            print(lnsD.intensity.value);
        }
    }
    

    private void Update()
    {
        SlowTimeLogic();
        if (Input.GetKeyDown(KeyCode.Escape) && canPressEscape)
        {
            canPressEscape = false;
            mainMenu.PressedEscape();
        }
        
    }

    void OnDrawGizmosSelected()
    {
        // Draws a 5 unit long red line in front of the object
        Gizmos.color = Color.red;
        Debug.DrawRay(bottomPos.position, bottomPos.TransformDirection(Vector3.up) * playerHeight, Color.yellow);
    }
}
