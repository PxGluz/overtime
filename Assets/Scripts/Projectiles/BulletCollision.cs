using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    [HideInInspector]
    public float bulletDamage = 0;
    private void Start()
    {
        Invoke(nameof(Expire), 10f);
    }

    private void FixedUpdate()
    {
        Debug.DrawRay(transform.position, transform.forward * 2f, Color.yellow);

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, 2f))
        {
            if (Player.m.PointDebug != null)
                Player.m.PointDebug.transform.position =  hit.point;

            HandleLayerLogic(hit);
        }
    }

    public void HandleLayerLogic(RaycastHit hit)
    {

        DestroyWhenShot destroyWhenShot = hit.collider.gameObject.GetComponent<DestroyWhenShot>();
        if (destroyWhenShot != null)
            destroyWhenShot.ReceiveHit();

        switch (LayerMask.LayerToName(hit.collider.gameObject.layer))
        {
            case "Interactable":
                return;

            case "Enemy":
                EnemyMaster enemy = hit.collider.gameObject.GetComponentInParent<EnemyMaster>();
                if (enemy != null)
                {
                    print(hit.collider.gameObject.name);
                    if (hit.collider.gameObject.name == "Head")
                    {
                        enemy.TakeDamage(bulletDamage * 2);
                    }
                    else 
                    {
                        enemy.TakeDamage(bulletDamage, hit.collider.gameObject, transform.forward * 25f);
                    }
                }
                break;

            case "Explosive":
                hit.collider.gameObject.GetComponent<ExplosiveBarrel>().ReceiveHit();
            break;

        }

        if (LayerMask.LayerToName(hit.collider.gameObject.gameObject.layer) != "Player")
        {
            Destroy(gameObject);
            //coll.enabled = false;
            //rb.isKinematic = true;
        }
    }

    private void Expire()
    {
        Destroy(gameObject);
    }

}
