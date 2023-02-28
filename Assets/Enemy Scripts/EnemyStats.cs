using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyStats : MonoBehaviour
{

    [Header("Stats")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("State:")]
    public bool isStunned;
    public bool armored = false;

    [Header("References and others")]
    public EnemyPathfinding pathfinding;

    //Other necessary variables to make other scripts work
    [HideInInspector]
    public int lastMeleeIndex = -1;

    [Header("Other: ")]
    public GameObject blood;
    public GameObject ragdoll;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void ReceiveHit(float damage)
    {
        pathfinding.announcedPosition = Player.m.transform.position;
        pathfinding.gotAnnounced = true;

        Instantiate(blood,transform.position, Quaternion.identity);

        if (armored)
            damage /= 2;

        currentHealth -= damage;

        if (currentHealth <= 0)
            Die();
    }

    public void Die()
    {
        EnemyShooting enemyShooting = gameObject.GetComponent<EnemyShooting>();
        if (enemyShooting != null)
        {
            GameObject drop = Instantiate(enemyShooting.weaponDrop, gameObject.transform.position, new Quaternion());
            Interactable interactable = drop.GetComponent<Interactable>();
            interactable.quantity = Player.m.weaponManager.GetWeaponByName(interactable.itemName).gunMagazineSize;
        }

        GameObject spawned = Instantiate(ragdoll, gameObject.transform.position, gameObject.transform.rotation);

        Destroy(gameObject);
    }
}
