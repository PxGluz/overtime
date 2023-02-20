using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject explosionPrefab;
    public float radius = 10;
    public float damage = 10;
    public float pushForce = 10;
    public float explosionDelay = 2;

  

    private void OnCollisionEnter(Collision collision)
    {
        Invoke(nameof(Explode), explosionDelay);
    }

    private void Explode()
    {
        GameObject explosionObject = Instantiate(explosionPrefab, gameObject.transform.position, new Quaternion());
        explosionObject.GetComponent<Explosion>().ActivateExplosion(gameObject, radius, damage, pushForce, Player.m.objectsAffectedByExplosions);
        Destroy(gameObject);
    }
}
