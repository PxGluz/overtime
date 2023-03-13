using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrow : MonoBehaviour
{
    [Header("References")]
    public Transform attackPoint;
    public Transform dropPoint;
    public GameObject objectToThrow;

    [Header("Settings")]
    public int totalThrows;
    public float ThrowCooldown;

    [Header("Throwing")]
    public KeyCode throwKey = KeyCode.Mouse0;

    bool readyToThrow;

    [Header("Drop:")]
    public KeyCode dropKey = KeyCode.Q;
    public float dropForce;

    public GameObject DebugBox;

    private void Start()
    {
        readyToThrow = true;
    }

    private void Update()
    {
        if (Player.m.weaponManager.currentWeapon.name.ToLower() == "fists")
            return;

        if (!Player.m.playerShooting.readyToShoot)
            return;

        if (Input.GetKeyDown(throwKey) && readyToThrow)//&& totalThrows > 0
        {
            Throw();
            Player.m.weaponManager.ChangeWeapon("Fists");
        }

        if (Input.GetKeyDown(dropKey))
        {
            DropWeapon();
            Player.m.weaponManager.ChangeWeapon("Fists");
        }
    }

    private void Throw()
    {
        readyToThrow= false;
        
        // Find the exact hit position using raycast
        Ray ray = Player.m.MainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Just a ray through the middle of your camera
        RaycastHit hit;

        // check if ray hits something
        Vector3 forceDirection = Player.m.MainCamera.transform.forward;

        if (Physics.Raycast(ray, out hit))
            forceDirection = (hit.point - attackPoint.position).normalized;
        else
            forceDirection = (ray.GetPoint(75) - attackPoint.position).normalized;

        // instantiate object to throw
        GameObject projectile;
        if (Player.m.weaponManager.currentWeapon.WeaponPrefab == null)
            projectile = Instantiate(objectToThrow, attackPoint.position, Player.m.MainCamera.transform.rotation);
        else
        {
            projectile = Instantiate(Player.m.weaponManager.currentWeapon.WeaponPrefab, attackPoint.position, attackPoint.rotation);
        }

        // get rigidbody component
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        // add force
        Vector3 forceToAdd = forceDirection * Player.m.weaponManager.currentWeapon.throwForce + transform.up * Player.m.weaponManager.currentWeapon.throwUpwardForce;

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);
        //projectileRb.velocity += Player.m.playerMovement.rb.velocity ;

        totalThrows--;

        // Set damage and pickup
        ThrownProjectile thrownProjectile = projectile.GetComponent<ThrownProjectile>();
        if (thrownProjectile != null)
        {
            thrownProjectile.damage = Player.m.weaponManager.currentWeapon.throwDamage;
            thrownProjectile.myPickUp = Player.m.weaponManager.currentWeapon.WeaponPrefab;
            thrownProjectile.PickUpSetActive(false);
        }

        RotateWhenThrown rotateWhenThrown = projectile.GetComponent<RotateWhenThrown>();
        if (rotateWhenThrown != null)
        {
            rotateWhenThrown.enabled = true;
        }

        SetQuantityOfInteractable(projectile);

        // implement throwCooldown
        Invoke(nameof(ResetThrow), ThrowCooldown);
    }

    public void DropWeapon()
    {
        
        // instantiate object to throw
        GameObject projectile = Instantiate(Player.m.weaponManager.currentWeapon.WeaponPrefab, dropPoint.position, Player.m.MainCamera.transform.rotation);

        // get rigidbody component
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        // add force
        Vector3 forceToAdd = Player.m.MainCamera.transform.forward * dropForce + transform.up ;

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

        ThrownProjectile thrownProjectile = projectile.GetComponent<ThrownProjectile>();
        if (thrownProjectile != null)
        {
            thrownProjectile.PickUpSetActive(true);
        }

        SetQuantityOfInteractable(projectile);

    }

    private void ResetThrow()
    {
        readyToThrow = true;
    }

    private void SetQuantityOfInteractable(GameObject projectile)
    {
        if (Player.m.weaponManager.GetWeaponType(Player.m.weaponManager.currentWeapon.name) == "ranged")
        {
            Interactable interactable = projectile.GetComponent<Interactable>();
            if (interactable != null)
            {
                interactable.quantity = Player.m.playerShooting.bulletsleft;
            }
        }
    }

}
