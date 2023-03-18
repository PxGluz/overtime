using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
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

    [Header("Stats")]
    public float maxHealth = 100f;
    public float currentHealth;
    public string myWeapon;
    [HideInInspector]
    public WeaponManager.Weapon WeaponClass;

    [Header("State:")]
    public bool isDead = false;
    public EnemyType enemyType;
    public DamageType damageType;
    public bool isStunned;
    private float stunTime = 0f;

    [Header("Other: ")]
    public GameObject blood;
    public GameObject ragdoll;

    //Other necessary variables to make other scripts work
    [HideInInspector]
    public int lastMeleeIndex = -1;

    private void Start()
    {
        if (currentHealth == 0f)
            currentHealth = maxHealth;

        enemyMovement = GetComponent<EnemyMovement>();
        enemyRanged = GetComponent<EnemyRanged>();

        if (myWeapon == "" && enemyType.ToString() == "Ranged")
            myWeapon = "Gun";
        else if (myWeapon == "" && enemyType.ToString() == "Melee")
            myWeapon = "Knife";

        WeaponClass = Player.m.weaponManager.GetWeaponByName(myWeapon);

        PutWeaponInHand();
        SetMyDamageType();
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

        currentHealth -= damage;

        print("Oh no, I " + this.gameObject.name + " took " + damage + " damage!");

        if (currentHealth <= 0) 
        {
            print("I " + this.gameObject.name + " am dead");
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
        GameObject drop = Instantiate(WeaponClass.WeaponPrefab, weaponInHand.transform.position, weaponInHand.transform.rotation);
        if (Player.m.weaponManager.GetWeaponType(WeaponClass.name) == "ranged")
        {
            Interactable interactable = drop.GetComponent<Interactable>();
            interactable.quantity = WeaponClass.gunMagazineSize;
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

        // Create moment in time
        Player.m.rewind.Invoke (nameof(Player.m.rewind.CreateNewMomentInTime),0.1f);
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
        weaponInHand = Instantiate(WeaponClass.WeaponPrefab, location.position, location.rotation);
        weaponInHand.transform.parent = location.transform;
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


    public enum EnemyType
    {
        Melee, Ranged
    }
    public enum DamageType
    {
        Pierce, Slash, Blunt, Explosion
    }


}
