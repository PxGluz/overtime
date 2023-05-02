using UnityEngine;

public class PlayerTeleport : MonoBehaviour
{
    public Transform shootOriginPoint;

    public GameObject teleportProjectilePrefab;

    public bool readyToTeleport;

    public float throwForce, throwUpwardForce;

    public float cooldown = 2f;
    public float teleportTimer = 0f;

    private void Start()
    {
        teleportTimer = cooldown + 0.1f;
        readyToTeleport = true;
        Player.m.teleportCooldown.ActivateSliders(false);
    }

    void Update()
    {

        if (Input.GetMouseButtonUp(2) && readyToTeleport)
        {
            ShootTeleportProjectile();
        }

        if (teleportTimer <= cooldown)
        {
            teleportTimer += Time.deltaTime;
            Player.m.teleportCooldown.ChargeSliders(0, cooldown, teleportTimer);
        }
        else if (!readyToTeleport)
        {
            readyToTeleport = true;
            Player.m.teleportCooldown.ActivateSliders(false);
        }
    }

    private void ShootTeleportProjectile()
    {
        readyToTeleport = false;
        teleportTimer = 0f;

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

        //Invoke(nameof(ResetThrow), teleportCooldown);
    }

    /*
    private void ResetThrow()
    {
        readyToTeleport = true;
    }
    */

}
