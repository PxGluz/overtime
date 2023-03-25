using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WeaponManager;

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

    [Header("Stats")]
    public float maxHealth = 100f;
    public float currentHealth;
    public string myWeapon;
    //[HideInInspector]
    public WeaponManager.Weapon myWeaponClass;

    [Header("State:")]
    public bool isDead = false;
    public EnemyType enemyType;
    public DamageType damageType;
    public bool isStunned;
    private float stunTime = 0f;

    [Header("Other: ")]
    public GameObject blood;
    public GameObject ragdoll;

    private void Start()
    {
        if (currentHealth == 0f)
            currentHealth = maxHealth;

        enemyMovement = GetComponent<EnemyMovement>();
        enemyRanged = GetComponent<EnemyRanged>();
        soundManager = GetComponent<NeedSounds>();

        if (myWeapon == "" && enemyType.ToString() == "Ranged")
            myWeapon = "Gun";
        else if (myWeapon == "" && enemyType.ToString() == "Melee")
            myWeapon = "Knife";

        myWeaponClass = Player.m.weaponManager.GetWeaponByName(myWeapon);

        enemyHealthBar.activateHealthSliders(false);

        PutWeaponInHand();
        SetMyDamageType();
        NerfEnemyWeapons();
    }

    private void Update()
    {
        if (stunTime > 0)
        {
            stunTime -= Time.deltaTime;
        }
        else
        {
            isStunned = false;
        }
    }

    public void StunEnemy(float stunDuration = 1f)
    {
        stunTime = stunDuration;
        isStunned = true;
    }

    public void TakeDamage(float damage, GameObject bodyPart=null, Vector3 direction=new Vector3())
    {

        if (isDead)
        {
            if (bodyPart && bodyPart.TryGetComponent(out Rigidbody rbBodyPart))
                rbBodyPart.velocity = direction;
            return;
        }

        enemyHealthBar.UpdateHealthBar( Mathf.Max(0, currentHealth - damage));
        enemyHealthBar.activateHealthSliders(true);

        currentHealth = Mathf.Max(0, currentHealth - damage);

        print(this.gameObject.name + " - " + damage + " damage");

        if (currentHealth <= 0) 
        {
            Die(bodyPart, direction);
        }
        else
        {
            if (enemyMovement != null)
                enemyMovement.Announce(false);

            if (enemyMovement != null && !enemyMovement.isChasingPlayer)
                StartCoroutine(enemyMovement.ChasePlayer(enemyMovement.chaseDuration));
        }
    }

    public void Die(GameObject bodyPart=null, Vector3 direction=new Vector3())
    {
        isDead = true;

        // Drop enemy weapon
        weaponInHand.SetActive(false);
        GameObject drop = Instantiate(myWeaponClass.WeaponPrefab, weaponInHand.transform.position, weaponInHand.transform.rotation);
        if (Player.m.weaponManager.GetWeaponType(myWeaponClass.name) == "ranged")
        {
            Interactable interactable = drop.GetComponent<Interactable>();
            //interactable.quantity = WeaponClass.gunMagazineSize;
            BulletPickUp bulletPick = Instantiate(Player.m.prefabHolder.bulletPickup,transform.position,Quaternion.identity).GetComponent<BulletPickUp>();
            bulletPick.nrOfBullets = 2;
        }


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

        //Destroy Enemy Scripts:
        IncapacitateEnemy();
        Destroy(animator);
    }

    private void NerfEnemyWeapons()
    {
        myWeaponClass.gunTimeBetweenShooting *= 150 / 100;
        myWeaponClass.gunMagazineSize = Mathf.FloorToInt((float)myWeaponClass.gunMagazineSize * 60 / 100);
        myWeaponClass.gunSpread += 0.05f;
    }

    public void IncapacitateEnemy()
    {
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
    }

    void PutWeaponInHand()
    {
        if (weaponInHand != null)
            return;

        Transform location;
        switch (enemyType.ToString())
        {
            case "Melee":
                location = enemyMelee.KnifePosition;
                break;
            case "Ranged":
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
                if (enemyType.ToString() == "Ranged")
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

    public SkinnedMeshRenderer EnemyMesh;
    public void SetMyDamageType()
    { 
        Material[] mats = EnemyMesh.materials;
        mats[4].color = Player.m.colorManager.GetDamageTypeMaterialByName(damageType.ToString());
        mats[5].color = Player.m.colorManager.GetDamageTypeMaterialByName(damageType.ToString());
        EnemyMesh.materials = mats;
    }

    private void SetWeaponInHandToPosition()
    {
        weaponInHand.transform.localPosition = new Vector3(0, 0, 0);
        weaponInHand.transform.localRotation = Quaternion.identity;
    }


    public enum EnemyType
    {
        Melee, Ranged
    }
    public enum DamageType
    {
        Pierce, Slash, Blunt, Explosion
    }


}
