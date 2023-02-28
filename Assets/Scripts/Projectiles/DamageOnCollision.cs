using UnityEngine;


public class DamageOnCollision : MonoBehaviour
{

    ThrownProjectile thrownProjectile;
    Interactable interact;

    FixedJoint fixedJoint;
    Rigidbody rb;

    void Start()
    {
        thrownProjectile = GetComponent<ThrownProjectile>();
        interact = GetComponent<Interactable>();
        rb = GetComponent<Rigidbody>();
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

        DestroyWhenShot destroyWhenShot = collision.gameObject.GetComponent<DestroyWhenShot>();
        if (destroyWhenShot != null)
            destroyWhenShot.ReceiveHit();

        if ( Player.m.weaponManager.GetWeaponType(interact.itemName) == "melee" )
        {
            Rigidbody collisionRigidBody = collision.gameObject.GetComponent<Rigidbody>();

            if (collisionRigidBody != null)
            {
                fixedJoint = gameObject.AddComponent(typeof(FixedJoint)) as FixedJoint;
                fixedJoint.connectedBody = collisionRigidBody;
            }
            else
                rb.isKinematic = true;

        }

        thrownProjectile.PickUpSetActive(true);

    }
}
