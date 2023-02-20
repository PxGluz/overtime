using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public float throwForce;
    public float throwUpwardForce;

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
        if (Input.GetKeyDown(throwKey) && readyToThrow && totalThrows > 0){
            Throw();
        }

        if (Input.GetKeyDown(dropKey) && Player.m.weaponManager.currentWeapon.name.ToLower() != "fists")
        {
            DropWeapon();
            Player.m.weaponManager.ChangeWeapon("Fists");
        }
    }

    private void Throw()
    {
        readyToThrow= false;

        // instantiate object to throw
        GameObject projectile;
        if (Player.m.weaponManager.currentWeapon.ThrowablePrefab == null)
            projectile = Instantiate(objectToThrow, attackPoint.position, Player.m.MainCamera.transform.rotation);
        else
        {
            projectile = Instantiate(Player.m.weaponManager.currentWeapon.ThrowablePrefab, Player.m.weaponManager.currentWeapon.WeaponModelOnPlayer.transform.position, attackPoint.rotation);
            attackPoint = Player.m.weaponManager.currentWeapon.WeaponModelOnPlayer.transform;
        }
        //projectile.transform.SetPositionAndRotation(attackPoint.position, Player.m.MainCamera.transform.rotation);

        // get rigidbody component
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        // calculate direction
        Vector3 forceDirection = Player.m.MainCamera.transform.forward;

        RaycastHit hit;

        if (Physics.Raycast(Player.m.MainCamera.transform.position, Player.m.MainCamera.transform.forward, out hit, 500f))
        {
            forceDirection = (hit.point - attackPoint.position).normalized;
            print(hit.transform.gameObject.name);
        }
        Debug.DrawRay(Player.m.MainCamera.transform.position, Player.m.MainCamera.transform.forward);

        // add force
        //Vector3 forceToAdd = Player.m.MainCamera.transform.forward * throwForce + transform.up * throwUpwardForce;
        Vector3 forceToAdd = forceDirection * throwForce + transform.up * throwUpwardForce;

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);
        //projectileRb.velocity += Player.m.playerMovement.rb.velocity ;

        totalThrows--;

        // implement throwCooldown
        Invoke(nameof(ResetThrow), ThrowCooldown);
    }

    public void DropWeapon()
    {
        
        // instantiate object to throw
        GameObject projectile = Instantiate(Player.m.weaponManager.currentWeapon.WeaponPickupPrefab, dropPoint.position, Player.m.MainCamera.transform.rotation);

        // get rigidbody component
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        // add force
        Vector3 forceToAdd = Player.m.MainCamera.transform.forward * dropForce + transform.up ;

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);


            //Player.m.weaponManager.ChangeWeapon("Fists");
        
    }

    private void ResetThrow()
    {
        readyToThrow = true;
    }



}
