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

    private void Update()
    {
        Player.m.AnnounceEnemy(transform.position, Player.m.EnemyAnnoucedByWeaponRange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (thrownProjectile == null)
        {
            this.enabled = false;
            return;
        }

        if (thrownProjectile.isInPickUpState || collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Explosive"))
        {
            collision.gameObject.GetComponent<ExplosiveBarrel>().ReceiveHit();
        }

        EnemyMaster enemy = collision.gameObject.GetComponentInParent<EnemyMaster>();
        if (enemy != null)
        {
            if (collision.gameObject.name == "spine.006")
                enemy.TakeDamage(thrownProjectile.damage, collision.gameObject, transform.forward * 30f, contactPoint: collision.contacts[0].point, true);
            else
                enemy.TakeDamage(thrownProjectile.damage, collision.gameObject, transform.forward * 30f, contactPoint: collision.contacts[0].point);

            rb.velocity = Vector3.zero;
        }
        else
        {
            Player.m.particleManager.CreateParticle(collision.contacts[0].point, -transform.forward);
        }

        DestroyWhenShot destroyWhenShot = collision.gameObject.GetComponent<DestroyWhenShot>();
        if (destroyWhenShot != null)
            destroyWhenShot.ReceiveHit();

        if (Player.m.weaponManager.GetWeaponType(interact.itemName) == "melee")
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
