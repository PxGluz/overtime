using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleport : MonoBehaviour
{
    public Transform shootOriginPoint;
    
    public GameObject teleportProjectilePrefab;

    public KeyCode teleportKey;

    public float teleportCooldown = 2f;
    public bool readyToTeleport = true;

    public float throwForce, throwUpwardForce;

    void Update()
    {
        if (Input.GetKeyDown(teleportKey) && readyToTeleport)
        {
            ShootTeleportProjectile();
        }
    }

    private void ShootTeleportProjectile()
    {
        readyToTeleport = false;

        // Find the exact hit position using raycast
        Ray ray = Player.m.MainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Just a ray through the middle of your camera
        RaycastHit hit;

        // check if ray hits something
        Vector3 forceDirection = Player.m.MainCamera.transform.forward;

        if (Physics.Raycast(ray, out hit))
        {
            forceDirection = (hit.point - shootOriginPoint.position).normalized;
        }
        else
        {
            forceDirection = (ray.GetPoint(75) - shootOriginPoint.position).normalized;
        }

        // instantiate object to throw
        GameObject projectile = Instantiate(teleportProjectilePrefab, shootOriginPoint.position, shootOriginPoint.rotation);


        // get rigidbody component
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        // add force                
        Vector3 forceToAdd = forceDirection * throwForce + transform.up * throwUpwardForce;

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);
        
        Invoke(nameof(ResetThrow), teleportCooldown);
    }

    private void ResetThrow()
    {
        readyToTeleport = true;
    }

}
