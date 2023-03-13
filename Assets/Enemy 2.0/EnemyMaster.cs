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
    public bool isStunned;
    private float stunTime = 0f;

    [Header("Ragdoll Death Related:")] 
    public GameObject animatedRig;
    public GameObject ragdollRig;
    
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

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        print("Oh no, I " + this.gameObject.name + " took " + damage + " damage!");

        if (currentHealth <= 0) 
        {
            print("I " + this.gameObject.name + " am dead");
            Die();
        }
        else
        {
            enemyMovement.Announce(false);

            if (!enemyMovement.isChasingPlayer)
                StartCoroutine(enemyMovement.ChasePlayer(enemyMovement.chaseDuration));
        }
    }

    public void Die()
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

        // Enemy ragdoll
        if (animatedRig != null && ragdollRig != null)
        {
            Queue<Transform> animatedChildList = new Queue<Transform>();
            Queue<Transform> ragdollChildList = new Queue<Transform>();
            animatedChildList.Enqueue(animatedRig.transform);
            ragdollChildList.Enqueue(ragdollRig.transform);
            while (animatedChildList.Count > 0)
            {
                Transform currentAnimatedChild = animatedChildList.Dequeue();
                Transform currentRagdollChild = ragdollChildList.Dequeue();
                foreach (Transform child in currentAnimatedChild)
                    if (!child.CompareTag("EnemyWeapon"))
                        animatedChildList.Enqueue(child);
                foreach (Transform child in currentRagdollChild)
                    ragdollChildList.Enqueue(child);
                currentRagdollChild.position = currentAnimatedChild.position;
                currentRagdollChild.rotation = currentAnimatedChild.rotation;
                currentRagdollChild.localScale = currentAnimatedChild.localScale;
            }
            animatedRig.transform.parent.gameObject.SetActive(false);
            ragdollRig.transform.parent.gameObject.SetActive(true);
        }

        // Disable enemy scripts
        visionCone.enabled = false;

        enemyMovement.StopAllCoroutines();
        enemyMovement.isChasingPlayer = false;
        enemyMovement.agent.enabled = false;
        enemyMovement.enabled = false;

        enemyMelee.enabled = false;
        enemyRanged.enabled = false;

        // Create moment in time
        Player.m.rewind.Invoke (nameof(Player.m.rewind.CreateNewMomentInTime),0.1f);
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

    public enum EnemyType
    {
        Melee, Ranged
    }

}
