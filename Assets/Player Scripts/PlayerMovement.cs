using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody rb;

    [Header("Movement")]
    public float currentSpeed;
    public float moveSpeed;
    public float sprintSpeed;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public float fallMultiplier;
    public bool readyToJump = true;
    public KeyCode jumpKey = KeyCode.Space;
    public float gravityForce = 10f;
    public bool isAscending = false;
    public bool isDescending = false;

    [Header("Ground Check")]
    public Transform GroundCheckSource;
    public LayerMask groundLayer;
    public bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }



    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, Mathf.Abs(GroundCheckSource.localPosition.y) + 0.2f, groundLayer);
        UnityEngine.Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * (Mathf.Abs(GroundCheckSource.localPosition.y) + 0.2f), Color.red);

        MyInput();
        SpeedControl();

        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        if ( isAscending && rb.velocity.y < 0)
        {
            isAscending = false;
            isDescending= true;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();

        HandleGravity();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            isAscending = true; 
            isDescending = false;
            
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // Sprint
    }

    private void HandleGravity()
    {
        if (!grounded)
        {   
            // Normal gravity when falling 
            if (!isDescending)
                rb.AddForce(Vector3.down * gravityForce, ForceMode.Acceleration);
            // Stronger gravity when finishing jump
            else
                rb.AddForce(Vector3.down * gravityForce * fallMultiplier, ForceMode.Acceleration);
        }
        else
        {
            isDescending = false;
        }
    }

    private void MovePlayer()
    {
        //calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
    
        // on ground
        if (grounded)
            rb.AddForce(moveDirection.normalized * currentSpeed * 10f, ForceMode.Force);
        
        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * currentSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x,0f,rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > currentSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * currentSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}
