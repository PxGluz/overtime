using CameraShake;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class PlayerThrow : MonoBehaviour
{
    [Header("References")]
    public Transform attackPoint;
    public Transform dropPoint;
    public ThrowChargerSlider throwChargerSlider;

    [Header("Settings")]
    public float ThrowCooldown;
    [Header("Throw force")]
    public float minThrowForce;
    public float maxThrowForce;
    public float throwMultiplier;
    public float throwForce;

    [Header("Throwing")]
    public KeyCode throwKey = KeyCode.Mouse0;

    bool readyToThrow;

    [Header("Drop:")]
    public KeyCode dropKey = KeyCode.Q;
    public float dropForce;

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

        if (!Player.m.weaponManager.weaponIsInPlace)
            return;

        if (Input.GetKeyDown(throwKey) && readyToThrow)
        {
            throwForce = minThrowForce;
            throwChargerSlider.ActivateSliders(true);
        }
        if (Input.GetKey(throwKey) && readyToThrow)
        {
            throwForce += Time.deltaTime * throwMultiplier;
            if (throwForce > maxThrowForce)
                throwForce = maxThrowForce;

            throwChargerSlider.ChargeSliders(minThrowForce,maxThrowForce,throwForce);
        }
        if (Input.GetKeyUp(throwKey) && readyToThrow)
        {
            Throw(throwForce);
            Player.m.weaponManager.ChangeWeapon("Fists");
            throwChargerSlider.ActivateSliders(false);
            throwForce = minThrowForce;
        }

        if (Input.GetKeyDown(dropKey))
        {
            DropWeapon();
            Player.m.weaponManager.ChangeWeapon("Fists");
        }
    }

    private void Throw(float throwForce)
    {
        readyToThrow = false;
        
        // Find the exact hit position using raycast
        Ray ray = Player.m.MainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Just a ray through the middle of your camera
        RaycastHit hit;

        // check if ray hits something
        Vector3 forceDirection = Player.m.MainCamera.transform.forward;

        if (Physics.Raycast(ray, out hit))
        {
            forceDirection = (hit.point - attackPoint.position).normalized;
        }
        else
        {
            forceDirection = (ray.GetPoint(75) - attackPoint.position).normalized;
        }

        // instantiate object to throw
        GameObject projectile = Instantiate(Player.m.weaponManager.currentWeapon.WeaponPrefab, attackPoint.position, attackPoint.rotation);

        StartCoroutine(PlaySoundAfterDelay(projectile));

        // get rigidbody component
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        // add force                          Player.m.weaponManager.currentWeapon.throwForce
        Vector3 forceToAdd = forceDirection * throwForce + transform.up * Player.m.weaponManager.currentWeapon.throwUpwardForce;

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

        // Set damage and pickup
        ThrownProjectile thrownProjectile = projectile.GetComponent<ThrownProjectile>();
        if (thrownProjectile != null)
        {
            thrownProjectile.damage = Player.m.weaponManager.currentWeapon.throwDamage;
            thrownProjectile.myDamageType = "Ranged";
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
    private IEnumerator PlaySoundAfterDelay(GameObject projectile) {yield return new WaitForEndOfFrame(); projectile.GetComponent<NeedSounds>().Play("throw"); }
    public void DropWeapon()
    {
        throwChargerSlider.ActivateSliders(false);
        throwForce = minThrowForce;

        // instantiate object to throw
        GameObject projectile = Instantiate(Player.m.weaponManager.currentWeapon.WeaponPrefab, dropPoint.position, dropPoint.rotation);//Player.m.MainCamera.transform.rotation

        // get rigidbody component
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        // add force
        Vector3 forceToAdd = Player.m.MainCamera.transform.forward * dropForce + transform.up ;

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);
        projectileRb.velocity += Player.m.playerMovement.rb.velocity;

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
