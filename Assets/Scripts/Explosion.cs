using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    /** TO USE THIS SCRIPT USE THE EXPLOSION PREFAB **/

    private bool explode = false;
    public float radius;
    public LayerMask isAffected;
    public float damage;
    public float pushForce;
    private GameObject parent; // The object that instantiated this explosion. Needed so that you don't get infinite explosions out of a single barrel.

    void Update()
    {
        if (explode)
        {
            Collider[] hits = Physics.OverlapSphere(gameObject.transform.position, radius, isAffected);

            foreach (Collider hit in hits)
            {
                if (hit.gameObject.Equals(parent))
                    continue;

                switch (LayerMask.LayerToName(hit.gameObject.layer))
                {
                    case "Enemy":
                        Rigidbody enemyRB = hit.gameObject.GetComponentInParent<Rigidbody>();
                        enemyRB.AddExplosionForce(pushForce, gameObject.transform.position, radius, 0f, ForceMode.Impulse);
                        float trueDamage = 1f / Mathf.Pow(Vector3.Distance(gameObject.transform.position, hit.transform.position), 2) * damage;
                        hit.gameObject.GetComponentInParent<EnemyStats>().ReceiveHit(trueDamage);
                        break;
                    case "Player":
                        Debug.Log("Player hit by explosion. For now nothing happens.");
                        break;
                    case "Pushable":
                        Rigidbody pushableRB = hit.gameObject.GetComponent<Rigidbody>();
                        pushableRB.AddExplosionForce(pushForce, gameObject.transform.position, radius, 0f, ForceMode.Impulse);
                        break;
                    case "Explosive":
                        hit.gameObject.GetComponent<ExplosiveBarrel>().ReceiveHit();
                        break;
                }
            }

            Destroy(gameObject);
        }
    }

    // This function starts the explosion.
    public void ActivateExplosion(GameObject parent, float radius, float damage, float pushForce, LayerMask isAffected)
    {
        this.parent = parent;
        this.radius = radius;
        this.damage = damage;
        this.isAffected = isAffected;
        this.pushForce = pushForce;
        explode = true;
    }
}
