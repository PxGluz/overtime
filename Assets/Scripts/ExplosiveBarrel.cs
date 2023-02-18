using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{
    public float damage;
    public float radius;
    public LayerMask isAffected;
    public float pushForce;
    public GameObject explosionPrefab;

    // This activates the explosion.
    public void ReceiveHit()
    {
        GameObject explosionObject = Instantiate(explosionPrefab, gameObject.transform.position, new Quaternion());
        explosionObject.GetComponent<Explosion>().ActivateExplosion(gameObject, radius, damage, pushForce, isAffected);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(gameObject.transform.position, radius);
    }
}
