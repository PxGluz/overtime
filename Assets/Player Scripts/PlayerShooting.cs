using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{

    public float shootingCooldown;
    public float shootingDistance = 10f;
    private Transform playerCameraTransform;
    private bool canShoot = true;
    void Start()
    {
        playerCameraTransform = Player.m.MainCamera.transform;
    }

    void Update()
    {

        // stop the shooting script if the player isn't in the shoot attack type
        if (Player.m.AttackType != "shoot")
            return;

        if (Input.GetMouseButton(0))
        {
            TryShooting();
        }
    }

    // Function called on hit. Change it for something to happen.
    void HitEffects(RaycastHit hitInfo)
    {
        Debug.DrawRay(playerCameraTransform.position, hitInfo.point - playerCameraTransform.position, new Color(0, 0, 1), 2);
        Debug.Log(hitInfo.collider.gameObject.name + " " + hitInfo.point);
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
