using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class RotateWhenThrown : MonoBehaviour
{
    Rigidbody rb;
    public float rotationForce;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    void FixedUpdate()
    {
        rb.AddTorque(-transform.up * rotationForce, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision collision)
    {
        this.enabled = false;
    }

}
