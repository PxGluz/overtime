using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using static UnityEditor.Progress;

public class DamageOnCollision : MonoBehaviour
{

    ThrownProjectile thrownProjectile;
    Interactable interact;

    void Start()
    {
        thrownProjectile = GetComponent<ThrownProjectile>();
        interact = GetComponent<Interactable>();
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


        //print(Player.m.weaponManager.GetWeaponType(interact.name));
        if ( Player.m.weaponManager.GetWeaponType(interact.itemName) == "melee")
        {
            if (enemy == null)
                this.gameObject.GetComponent<Rigidbody>().isKinematic = true;

        }

        thrownProjectile.PickUpSetActive(true);

    }
}
