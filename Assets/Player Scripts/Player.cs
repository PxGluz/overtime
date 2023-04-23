using CameraShake;
using System;
using System.Collections;
using System.Collections.Generic;
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
    public AudioManager audioManager;
    public SettingsManager settingsManager;
    public Interact interact;
    public ScoringSystem scoringSystem;
    public ParticleManager particleManager;
    public CrossHairLogic crossHairLogic;
    public PlayerHealthBar playerHealthBar;
    public TeleportCooldown teleportCooldown;

    [Header("Other References:")]
    public Camera MainCamera;
    public Rigidbody playerRigidBody;
    public Animator playerUIAnimator;
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
    public LayerMask playerLayer;
    public float smoothTime;
    public bool canPressEscape;
    public BounceShake.Params takeDamageShakeParams;
    public BounceShake.Params killEnemyShakeParams;

    [Header("PostProcessing")] 
    private float vigTarget = 0;
    public float vigIntensity;
    private float bloomTarget = 0;
    public float bloomIntensity;
    private float lensTarget = 0;
    public float lensIntensity;
    
    [Header("Stats:")]
    public float MaxHealth;
    public float currentHealth;

    private float ref1, ref2, ref3, ref4;

    private void OnApplicationQuit()
    {
        SnapEffects(true);
    }

    void Awake()
    {
        m = this;

        crouchLogic = GetComponent<CrouchLogic>();
        playerThrow = GetComponent<PlayerThrow>();
        playerMovement = GetComponent<PlayerMovement>();
        playerShooting = GetComponent<PlayerShooting>();
        interact = GetComponent<Interact>();

        playerRigidBody = GetComponent<Rigidbody>();

        canPressEscape = !SceneManager.GetActiveScene().name.Equals("MainMenuLobby");
    }

    private void Start()
    {
        currentHealth = MaxHealth;
        playerHeight = topPosition.position.y - bottomPos.position.y;
        SnapEffects(true);
    }

    public void TakeDamage(float damage)
    {
        CameraShaker.Shake(new BounceShake(takeDamageShakeParams, transform.position));
        scoringSystem.AddScore(100,"minus");
        print("The player took " + damage + " damage");

        currentHealth -= damage;
        
        playerHealthBar.UpdateHealthBar(currentHealth);


        if (currentHealth <= 0)
        {
            Die();
        }

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
        object data = SerializationManager.Load("levelInfo");
        if (data != null)
        {
            List<Contract.LevelData> levelDatas = data as List<Contract.LevelData>;
            foreach (Contract.LevelData levelData in levelDatas)
                if (levelData.levelName == SceneManager.GetActiveScene().name)
                    levelData.highscore = Mathf.Max(levelData.highscore, scoringSystem.scoreValue);
            SerializationManager.Save("levelInfo", levelDatas);
        }
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

    public void SnapEffects(bool toZero = false, string vigType="good")
    {
        if (vigType == "minus")
        {
            if (volume.TryGet(out Vignette vig))
                vig.color.value = Color.red;
        }
        else
            if (volume.TryGet(out Vignette vig))
                vig.color.value = Color.cyan;
        if (volume)
        {
            if (volume.TryGet(out Vignette vig))
                vig.intensity.value = toZero ? 0 : vigIntensity;
            if (volume.TryGet(out Bloom blm))
                blm.intensity.value = toZero ? 0.5f : bloomIntensity;
            if (volume.TryGet(out LensDistortion lnsD))
                lnsD.intensity.value = toZero ? 0 : lensIntensity;
        }
    }
    
    public void Slowing()
    {
        if (volume.TryGet(out Vignette vig))
            vig.color.value = Color.cyan;
        vigTarget = vigIntensity;
        bloomTarget = bloomIntensity;
        lensTarget = lensIntensity;
    }

    public void Fasting()
    {
        vigTarget = 0;
        bloomTarget = 0.5f;
        lensTarget = 0;
    }
    
    void SlowTimeLogic()
    {
        if (volume)
        {
            if (volume.TryGet(out Vignette vig) && Mathf.Abs(vig.intensity.value - vigTarget) > 0.001f)
                vig.intensity.value = Mathf.SmoothDamp(vig.intensity.value, vigTarget, ref ref2, smoothTime);
            if (volume.TryGet(out Bloom blm) && Mathf.Abs(blm.intensity.value - bloomTarget) > 0.001f)
                blm.intensity.value = Mathf.SmoothDamp(blm.intensity.value, bloomTarget, ref ref3, smoothTime);
            if (volume.TryGet(out LensDistortion lnsD) && Mathf.Abs(lnsD.intensity.value - lensTarget) > 0.001f)
                lnsD.intensity.value = Mathf.SmoothDamp(lnsD.intensity.value, lensTarget, ref ref4, smoothTime);
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

    public LayerMask AnnouceHitLayer;
    public float EnemyAnnoucedByWeaponRange;
    public void AnnounceEnemy(Vector3 sourcePosition, float announceRange)
    {
        // Announce to all other enemies in range.
        Collider[] enemies = Physics.OverlapSphere(sourcePosition, announceRange, enemyLayer);

        foreach (Collider enemy in enemies)
        {
            EnemyMovement enemyMovement = enemy.gameObject.GetComponentInParent<EnemyMovement>();
            if (enemyMovement == null)
                continue;
            if (enemyMovement.enabled == false)
                continue;
            if (enemyMovement.isChasingPlayer)
                continue;


            Vector3 dir = sourcePosition- enemyMovement.transform.position;

            if (Mathf.Abs(Vector3.Angle(enemyMovement.transform.forward, Vector3.Normalize(dir))) > 180f * 0.5f)
                continue;

            Physics.Raycast(sourcePosition, Vector3.Normalize(-dir), out RaycastHit hitInfo, announceRange, AnnouceHitLayer);
            Debug.DrawRay(sourcePosition, Vector3.Normalize(-dir), Color.red);
     
            if (hitInfo.transform != null && LayerMask.LayerToName(hitInfo.transform.gameObject.layer) != "Enemy")
            {
                continue;
            }

            enemyMovement.StartChasePlayer();
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draws a 5 unit long red line in front of the object
        Gizmos.color = Color.red;
        Debug.DrawRay(bottomPos.position, bottomPos.TransformDirection(Vector3.up) * playerHeight, Color.yellow);
    }
}
