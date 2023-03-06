using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMaster : MonoBehaviour
{

    [Header("Stats")]
    public float maxHealth = 100f;
    public float currentHealth;
    public string myWeapon;
    [HideInInspector]
    public WeaponManager.Weapon WeaponClass;

    [Header("State:")]
    public EnemyType enemyType;
    public bool isStunned;

    [Header("References:")]
    [HideInInspector]
    public EnemyMovement enemyMovement;
    public Animator animator;
    public VisionCone visionCone;


    [Header("Ragdoll Death Related:")] 
    [HideInInspector]
    public GameObject animatedRig;
    public GameObject ragdollRig;
    
    
    //Other necessary variables to make other scripts work
    [HideInInspector]
    public int lastMeleeIndex = -1;

    [Header("Other: ")]
    public GameObject blood;
    public GameObject ragdoll;


    private void Start()
    {
        currentHealth = maxHealth;

        enemyMovement = GetComponent<EnemyMovement>();

        WeaponClass = Player.m.weaponManager.GetWeaponByName(myWeapon);
    }


    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0) 
        {
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
        /*
        EnemyShooting enemyShooting = gameObject.GetComponent<EnemyShooting>();
        if (enemyShooting != null)
        {
            GameObject drop = Instantiate(enemyShooting.weaponDrop, gameObject.transform.position, new Quaternion());
            Interactable interactable = drop.GetComponent<Interactable>();
            interactable.quantity = Player.m.weaponManager.GetWeaponByName(interactable.itemName).gunMagazineSize;
        }

        GameObject spawned = Instantiate(ragdoll, gameObject.transform.position, gameObject.transform.rotation);

        Destroy(gameObject);
        */
        visionCone.enabled = false;

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
        enemyMovement.StopAllCoroutines();
        enemyMovement.agent.enabled = false;
        enemyMovement.enabled = false;

        //animator.enabled = false;

    }



    public enum EnemyType
    {
        Melee, Ranged
    }

}
