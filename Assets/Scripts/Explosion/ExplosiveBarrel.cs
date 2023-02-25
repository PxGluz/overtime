using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{
    public float damage;
    public float radius;
    public float pushForce;

    // This activates the explosion.
    public void ReceiveHit()
    {
        // Collider is disabled so that no explosion (this or another) can pick the collider up.
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
        ExplosionManager.instance.Explode(gameObject.transform.position, radius, damage, pushForce);
        // After the explosion is completed the barrel is destroyed.
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(gameObject.transform.position, radius);
    }
}
