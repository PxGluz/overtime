using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionManager : MonoBehaviour
{
    public static ExplosionManager instance = null;
    public LayerMask objectsAffectedByExplosions;

    void Awake()
    {
        // Singleton: A single explosion manager can exist.
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void Explode(Vector3 position, float radius, float damage, float pushForce)
    {
        Collider[] hits = Physics.OverlapSphere(position, radius, objectsAffectedByExplosions);

        foreach (Collider hit in hits)
        {
            switch (LayerMask.LayerToName(hit.gameObject.layer))
            {
                case "Enemy":
                    Rigidbody enemyRB = hit.gameObject.GetComponentInParent<Rigidbody>();
                    enemyRB.AddExplosionForce(pushForce, position, radius, 0f, ForceMode.Impulse);
                    // Damage decreases proportional to the distance squared.
                    float trueDamage = 1f / Mathf.Pow(Vector3.Distance(position, hit.transform.position), 2) * damage;
                    hit.gameObject.GetComponentInParent<EnemyStats>().ReceiveHit(trueDamage);
                    break;
                case "Player":
                    Debug.Log("Player hit by explosion. For now nothing happens.");
                    break;
                case "Pushable":
                    Rigidbody pushableRB = hit.gameObject.GetComponent<Rigidbody>();
                    pushableRB.AddExplosionForce(pushForce, position, radius, 0f, ForceMode.Impulse);
                    break;
                case "Explosive":
                    hit.gameObject.GetComponent<ExplosiveBarrel>().ReceiveHit();
                    break;
            }
        }
    }
}
