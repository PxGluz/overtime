using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public float shootingCooldown;
    public float shootingDistance = 10f;
    private Transform playerCameraTransform;
    private bool canShoot = true;
    public float baseDamage = 50f;
    void Start()
    {
        playerCameraTransform = Player.m.MainCamera.transform;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            TryShooting();
        }
    }

    // Function called on hit. Change it for something to happen.
    void HitEffects(RaycastHit hitInfo)
    {
        //Debug.DrawRay(playerCameraTransform.position, hitInfo.point - playerCameraTransform.position, new Color(0, 0, 1), 2);
        //Debug.Log(hitInfo.collider.gameObject.name + " " + hitInfo.point);

        if (hitInfo.collider.gameObject.layer == 8) // object hit is in enemy layer
        {
            if (hitInfo.collider.gameObject.name == "Body")
            {
                hitInfo.collider.gameObject.GetComponentInParent<EnemyStats>().ReceiveHit(baseDamage);
            }
            else if (hitInfo.collider.gameObject.name == "Head")
            {
                hitInfo.collider.gameObject.GetComponentInParent<EnemyStats>().ReceiveHit(baseDamage * 2);
            }
        }
    }

    void TryShooting()
    {
        if (canShoot)
        {
            if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit hitInfo, shootingDistance))
            {
                HitEffects(hitInfo);
            }
            canShoot = false;
            Invoke("ResetShooting", shootingCooldown);
        }
    }

    void ResetShooting()
    {
        canShoot = true;
    }
}
