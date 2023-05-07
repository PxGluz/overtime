using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityMine : MonoBehaviour
{

    private ThrownProjectile thrownProjectile;
    private Rigidbody rb;
    private CapsuleCollider coll;

    public Light lightSource;

    public bool isArmed = false;
    public float mineRadius = 5f;
    public float minePushForce = 10;
    public float ArmDelay = 2f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<CapsuleCollider>();
        thrownProjectile = GetComponent<ThrownProjectile>();

        lightSource.enabled= false;
    }


    void Update()
    {
        if (isArmed)
        {
            //detect enemy
            Collider[] hitObjects = Physics.OverlapSphere(transform.position, mineRadius);

            foreach (Collider obj in hitObjects)
            {
                switch (LayerMask.LayerToName(obj.gameObject.layer))
                {
                    case "Enemy":
                        EnemyMaster enemy = obj.gameObject.GetComponentInParent<EnemyMaster>();
                        if (enemy != null)
                        {
                            ExplosionManager.instance.Explode(transform.position, mineRadius, thrownProjectile.damage, minePushForce);
                            Destroy(gameObject);
                            return;
                        }
                    break;
                }
               

            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (thrownProjectile.isInPickUpState)
            return;

        Invoke(nameof(ArmBomb), ArmDelay);
    }

    public void ArmBomb()
    {
        rb.isKinematic = true;
        coll.enabled = false;
        isArmed = true;
        lightSource.enabled = true;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(gameObject.transform.position, mineRadius);
    }
}
