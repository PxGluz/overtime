using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PayerThrow : MonoBehaviour
{
    [Header("References")]

    public Transform attackPoint;
    public GameObject objectToThrow;

    [Header("Settings")]
    public int totalThrows;
    public float ThrowCooldown;

    [Header("Throwing")]
    public KeyCode throwKey = KeyCode.Mouse0;
    public float throwForce;
    public float throwUpwardForce;

    bool readyToThrow;

    private void Start()
    {
        readyToThrow = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(throwKey) && readyToThrow && totalThrows > 0){
            Throw();
        }
    }

    private void Throw()
    {
        readyToThrow= false;

        // instantiate object to throw
        GameObject projectile = Instantiate(objectToThrow, attackPoint.position, Player.m.MainCamera.transform.rotation);

        // get rigidbody component
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        // add force
        Vector3 forceToAdd = Player.m.MainCamera.transform.forward * throwForce + transform.up * throwUpwardForce;

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

        totalThrows--;

        // implement throwCooldown
        Invoke(nameof(ResetThrow), ThrowCooldown);
    }

    private void ResetThrow()
    {
        
    }


}
