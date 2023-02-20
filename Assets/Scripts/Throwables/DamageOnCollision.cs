using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class DamageOnCollision : MonoBehaviour
{

    ThrownProjectile thrownProjectile;

    void Start()
    {
        thrownProjectile = GetComponent<ThrownProjectile>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (thrownProjectile.isInPickUpState || collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            return;

        EnemyStats enemy = collision.gameObject.GetComponent<EnemyStats>();
        if (enemy != null)
        {
            enemy.ReceiveHit(thrownProjectile.damage);
        }

        thrownProjectile.PickUpSetActive(true);

    }
}
