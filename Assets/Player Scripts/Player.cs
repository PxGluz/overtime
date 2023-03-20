using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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
    public ColorManager colorManager;
    public AudioManager audioManager;
    public SettingsManager settingsManager;
    public Interact interact;
    public PrefabHolder prefabHolder;

    [Header("Other References:")]
    public Camera MainCamera;
    public Rigidbody playerRigidBody;
    public GameObject PointDebug;
    public GameObject playerObject;
    public VolumeProfile volume;

    [Header("Important variables:")]
    public string MoveType = "stop"; // current move types: stop, walk, run, crouch, slide
    public string AttackType = "melee"; // current attack types: none, shoot, melee   | future attack types: throw
    public LayerMask groundLayer;
    public LayerMask enemyLayer;
    public float lowTimeScale;
    public float smoothTime;
    private float timeScaleTarget = 1;

    [Header("PostProcessing")] 
    private float vigTarget = 0;
    public float vigIntensity;
    private float bloomTarget = 0;
    public float bloomIntensity;
    
    [Header("Stats:")]
    public float MaxHealth;
    public float currentHealth;

    private float ref1, ref2, ref3;

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

    public void Slowing()
    {
        timeScaleTarget = lowTimeScale;
        vigTarget = vigIntensity;
        bloomTarget = bloomIntensity;
    }

    public void Fasting()
    {
        timeScaleTarget = 1;
        vigTarget = 0;
        bloomTarget = 0;
    }
    
    void SlowTimeLogic()
    {
        Time.timeScale = Mathf.SmoothDamp(Time.timeScale, timeScaleTarget, ref ref1, smoothTime);
        if (volume)
        {
            if (volume.TryGet(out Vignette vig))
                vig.intensity.value = Mathf.SmoothDamp(vig.intensity.value, vigTarget, ref ref2, smoothTime);
            if (volume.TryGet(out Bloom blm))
                blm.intensity.value = Mathf.SmoothDamp(blm.intensity.value, bloomTarget, ref ref3, smoothTime);
        }
    }
    

    private void Update()
    {
        SlowTimeLogic();
    }
}
