using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using static UnityEngine.UI.Image;

public class BulletCollision : MonoBehaviour
{

    [HideInInspector]
    public float bulletDamage = 0;
    public float bulletRadius = 0.1f;

    void Update()
    {
        //detect enemy
        Collider[] hitObjects = Physics.OverlapSphere(transform.position, bulletRadius);


        foreach (Collider obj in hitObjects)
        {
            print(obj.gameObject);
            switch (LayerMask.LayerToName(obj.gameObject.layer))
            {
                case "Enemy":
                    EnemyStats enemy = obj.gameObject.GetComponentInParent<EnemyStats>();
                    if (enemy != null)
                    {
                        if (obj.gameObject.name == "Body")
                        {
                            enemy.ReceiveHit(bulletDamage);
                        }
                        else if (obj.gameObject.name == "Head")
                        {
                            enemy.ReceiveHit(bulletDamage * 2);
                        }
                    }
                   
                    break;
                    

                case "Explosive":
                    obj.gameObject.GetComponent<ExplosiveBarrel>().ReceiveHit();
                    break;

            }
            Destroy(gameObject);
            
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, bulletRadius);
    }
}
