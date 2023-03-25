using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class RotateWhenThrown : MonoBehaviour
{
   // Rigidbody rb;
    public float rotationsPerMinute;

    void Start()
    {
        //rb = GetComponent<Rigidbody>();
        //enabled = false;
    }


    void FixedUpdate()
    {
        transform.Rotate( 6.0f * rotationsPerMinute * Time.deltaTime, 0 , 0);
        //rb.AddTorque(transform.right * rotationForce, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision collision)
    {
        this.enabled = false;
    }

}
