using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using static UnityEngine.UI.Image;

public class TeleportProjectile : MonoBehaviour
{
    public LayerMask teleportProjectileCollisionLayer;
    public float colliderRadius;
    public float playerHeight;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    bool hasHit = false;
    RaycastHit downHitRaycast;
    RaycastHit upHitRaycast;

    void FixedUpdate()
    {


        //detect enemy
        Collider[] colList= Physics.OverlapSphere(transform.position, colliderRadius, teleportProjectileCollisionLayer);
        bool hasHitSomething = colList.Length > 0 ? true : false;


        if (hasHitSomething && !hasHit)
        {
            hasHit = true;

            Physics.Raycast(transform.position, Vector3.down, out downHitRaycast, Mathf.Infinity, teleportProjectileCollisionLayer);

            Physics.Raycast(transform.position, Vector3.up, out upHitRaycast, Mathf.Infinity, teleportProjectileCollisionLayer);

            float teleportPositionY = transform.position.y;

            if (Vector3.Distance(upHitRaycast.point, downHitRaycast.point) < Player.m.playerHeight)
            {
                teleportPositionY = (upHitRaycast.point.y+downHitRaycast.point.y)/2 + Player.m.playerHeight / 4;
                Player.m.crouchLogic.enterCrouchInstantly();
                print("tight spot");
            }

            else if (Vector3.Distance(transform.position,downHitRaycast.point) < Player.m.playerHeight / 2)
            {
                teleportPositionY = downHitRaycast.point.y + Player.m.playerHeight / 2;
                print("hit ground");
            }

            else if (Vector3.Distance(transform.position, upHitRaycast.point) < Player.m.playerHeight / 2)
            {
                teleportPositionY = upHitRaycast.point.y - Player.m.playerHeight / 2;
                print("hit tavan");
            }


            Player.m.transform.position = new Vector3(transform.position.x, teleportPositionY ,transform.position.z);

            //Destroy(gameObject);
            this.enabled = false;
            rb.isKinematic = true;
        }

        
       
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, colliderRadius);

        if (hasHit)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(downHitRaycast.point, .1f);
        }

    }
}
