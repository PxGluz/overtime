using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;


public class EnemyMaster : MonoBehaviour
{
    [Header("References:")]
    public EnemyMelee enemyMelee;
    public EnemyRanged enemyRanged;
    public Animator animator;
    public VisionCone visionCone;
    [HideInInspector]
    public GameObject weaponInHand;
    [HideInInspector]
    public EnemyMovement enemyMovement;
    public NeedSounds soundManager;
    public EnemyHealthBar enemyHealthBar;
    public EnemySpineLookAtPlayer enemySpineLookAtPlayer;
    public Transform EnemyCenter;

    [Header("Important:")]
    public string myWeapon;
    public float maxHealth = 100f;
    public float currentHealth;
    public float MeleePreferedDistanceToPlayer;
    public float RangedPreferedDistanceToPlayer;
    [HideInInspector]
    public WeaponManager.Weapon myWeaponClass;

    [Header("State:")]
    public bool isDead = false;
    public bool isStunned;
    public string enemyType;
    private float stunTime = 0f;

    [Header("Throw weapon towards player:")]
    public float verticalDropForce;
    public float horizontalDropForce;
    public float maxDistanceToThrow;
    public float minDistanceToThrow;

    [Header("Other: ")]
    public GameObject ragdoll;

    private void Start()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        enemyRanged = GetComponent<EnemyRanged>();
        soundManager = GetComponent<NeedSounds>();

        if (currentHealth == 0f)
            currentHealth = maxHealth;

        if (myWeapon == "" )
            myWeapon = "Knife";
       
        myWeaponClass = Player.m.weaponManager.GetWeaponByName(myWeapon);
        NerfEnemyWeapons();
        enemyType = myWeaponClass.attackType.ToString();

        enemyHealthBar.activateHealthSliders(false);

        ActivateAttackScripts();
        PutWeaponInHand();
    }

    private void ActivateAttackScripts()
    {
        if (enemyType != "ranged")
        {
            enemyRanged.enabled = false;
        }
        else
        {
            enemyMovement.PreferedDistanceToPlayer = RangedPreferedDistanceToPlayer;
            enemyRanged.bulletsleft = myWeaponClass.gunMagazineSize;
        }

        if (enemyType != "melee")
        {
            enemyMelee.enabled = false;
        }
        else
        {
            enemyMovement.PreferedDistanceToPlayer = MeleePreferedDistanceToPlayer;
        }
    }

    private void ThrowWeaponTowardsPlayer(GameObject droppedWeapon)
    {
        if (Vector3.Distance(Player.m.transform.position, transform.position) >= maxDistanceToThrow)
            return;

        Vector3 forceDirection = (Player.m.transform.position - droppedWeapon.transform.position).normalized;

        // get rigidbody component
        Rigidbody projectileRb = droppedWeapon.GetComponent<Rigidbody>();

        // add force
        Vector3 forceToAdd;
        if (Vector3.Distance(Player.m.transform.position, transform.position) <= minDistanceToThrow)
        {
            forceToAdd = forceDirection * 2 + transform.up * verticalDropForce;
        }
        else
        {
            forceToAdd = forceDirection * horizontalDropForce + transform.up * verticalDropForce;
        }

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

        /*
        RotateWhenThrown rotateWhenThrown = droppedWeapon.GetComponent<RotateWhenThrown>();
        if (rotateWhenThrown != null)
        {
            rotateWhenThrown.enabled = true;
        }
        */
    }

    private void Update()
    {
        if (stunTime > 0)
            stunTime -= Time.deltaTime;
        else
            isStunned = false;
    }

    public void TakeDamage(float damage, GameObject bodyPart=null, Vector3 direction=new Vector3(), Vector3 contactPoint = new Vector3(), bool isHeadShot = false)
    {
        Player.m.particleManager.CreateParticle(contactPoint, direction, "bulletHit");

        if (isDead)
        {
            if (bodyPart && bodyPart.TryGetComponent(out Rigidbody rbBodyPart))
                rbBodyPart.velocity = direction;
            return;
        }

        soundManager.Play("enemyHurt");
        Player.m.crossHairLogic.ActivateHitXEffect(isHeadShot);

        if (isHeadShot)
        {
            damage *= 2;
            print("HEADSHOT");
        }

        enemyHealthBar.UpdateHealthBar( Mathf.Max(0, currentHealth - damage));
        enemyHealthBar.activateHealthSliders(true);

        currentHealth = Mathf.Max(0, currentHealth - damage);

        print(this.gameObject.name + " - " + damage + " damage");

        if (enemyMovement != null)
            enemyMovement.Announce(false);
        
        if (currentHealth <= 0) 
        {
            Die(bodyPart, direction, isHeadShot);
        }
        else
        {

            if (enemyMovement != null && !enemyMovement.isChasingPlayer)
                StartCoroutine(enemyMovement.ChasePlayer(enemyMovement.chaseDuration));
        }
    }

    public void Die(GameObject bodyPart=null, Vector3 direction=new Vector3(),bool isHeadShot = false)
    {
        if (!isDead)
        {
            Player.m.scoringSystem.AddScore(isHeadShot ? 100: 50, "good");
        }
        isDead = true;


        soundManager.Stop("enemyFootSteps");

        // Drop enemy weapon
        weaponInHand.SetActive(false);
        GameObject drop = Instantiate(myWeaponClass.WeaponPrefab, weaponInHand.transform.position, weaponInHand.transform.rotation);
        if (Player.m.weaponManager.GetWeaponType(myWeaponClass.name) == "ranged")
        {
            Interactable interactable = drop.GetComponent<Interactable>();
            interactable.quantity = Player.m.weaponManager.GetWeaponByName(myWeaponClass.name).gunMagazineSize;
            //BulletPickUp bulletPick = Instantiate(Player.m.prefabHolder.bulletPickup,transform.position,Quaternion.identity).GetComponent<BulletPickUp>();
            //bulletPick.nrOfBullets = 2;
        }

        ThrowWeaponTowardsPlayer(drop);

        animator.enabled = false;
        // Enemy ragdoll
        if (ragdoll != null)
        {
            Queue<Transform> animatedChildList = new Queue<Transform>();
            animatedChildList.Enqueue(ragdoll.transform);
            while (animatedChildList.Count > 0)
            {
                Transform currentAnimatedChild = animatedChildList.Dequeue();
                if (currentAnimatedChild.TryGetComponent(out Rigidbody rb))
                    rb.isKinematic = false;
                foreach (Transform child in currentAnimatedChild)
                    animatedChildList.Enqueue(child);
            }
        }

        if (bodyPart && bodyPart.TryGetComponent(out Rigidbody rbBodyPart))
            rbBodyPart.velocity = direction;

        enemyRanged.enemyAmmoReloadDisplay.SliderSetActive(false);

        //Destroy Enemy Scripts:
        IncapacitateEnemy();
        Destroy(animator);
    }

    private void NerfEnemyWeapons()
    {
        myWeaponClass.bulletDamage = 10;
        myWeaponClass.gunTimeBetweenShooting *= 4;
        //myWeaponClass.gunMagazineSize = Mathf.FloorToInt((float)myWeaponClass.gunMagazineSize * 60 / 100);
        myWeaponClass.gunSpread += 0.05f;
        myWeaponClass.gunShootForce *= 0.7f;
    }

    public void IncapacitateEnemy()
    {
        soundManager.Stop("enemyFootSteps");
        
        //Destroy Enemy Scripts:
        if (enemyMovement != null)
        {
            enemyMovement.StopAllCoroutines();
            Destroy(enemyMovement.agent);
            Destroy(enemyMovement);
        }
        if (TryGetComponent(out Rigidbody rbBody))
            Destroy(rbBody);
        if (enemyMelee != null)
            Destroy(enemyMelee);
        if (enemyRanged != null)
            Destroy(enemyRanged);
        if (visionCone != null)
            Destroy(visionCone);
        if (enemySpineLookAtPlayer != null)
            Destroy(enemySpineLookAtPlayer);
    }
    public void StunEnemy(float stunDuration = 1f)
    {
        stunTime = stunDuration;
        isStunned = true;
    }

    void PutWeaponInHand()
    {
        if (weaponInHand != null)
            return;

        Transform location;
        switch (enemyType)
        {
            case "melee":
                location = enemyMelee.KnifePosition;
                break;
            case "ranged":
                location = enemyRanged.gunPosition;
                break;
            default:
                location = enemyMelee.KnifePosition;
                break;
        }
        
        weaponInHand = Instantiate(myWeaponClass.WeaponPrefab, location.position, location.rotation);
        weaponInHand.transform.parent = location.transform;

        Invoke(nameof(SetWeaponInHandToPosition),0.1f);
        
        // Delete all scripts on the prefab
        foreach (var comp in weaponInHand.GetComponents<Component>())
        {
            if (!(comp is Transform))
            {
                if (enemyType == "ranged")
                    if (comp is Interactable)
                    {
                        Interactable interact = weaponInHand.GetComponent<Interactable>();
                        enemyRanged.shootPoint = interact.myAttackPoint;
                    }
                Destroy(comp);
            }
        }

        // Change the prefab and the children's layer to "Enemy"
        var children = weaponInHand.GetComponentsInChildren<Transform>(includeInactive: true);
        foreach (var child in children)
            child.gameObject.layer = LayerMask.NameToLayer("Enemy");

        Collider[] weaponColliders = weaponInHand.GetComponentsInChildren<Collider>(includeInactive: true);
        foreach(Collider coll in weaponColliders)
        {
            Destroy(coll);
        }
    }

    private void SetWeaponInHandToPosition()
    {
        weaponInHand.transform.localPosition = new Vector3(0, 0, 0);
        weaponInHand.transform.localRotation = Quaternion.identity;
    }

}
