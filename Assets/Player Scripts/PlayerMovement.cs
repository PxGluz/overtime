using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody rb;

    [Header("Movement")]
    public Transform orientation;
    public float currentSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float crouchSpeed;

    public float groundDrag;

    [Header("Jumping")]
    public KeyCode jumpKey = KeyCode.Space;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public float fallMultiplier;
    private bool readyToJump = true;
    public float gravityForce = 10f;
    private bool isAscending = false;
    private bool isDescending = false;

    [Header("Ground Check")]
    public Transform GroundCheckSource;
    public float GroundCheckRadius = 0.2f;
    public bool grounded;


    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    //[Header("Crouch")]
    //public bool hasSpaceToExitCrouch;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        currentSpeed = walkSpeed;
    }



    private void Update()
    {
        // ground check: Raycast approach
        //grounded = Physics.Raycast(transform.position, Vector3.down, Mathf.Abs(GroundCheckSource.localPosition.y) + 0.2f, Player.m.groundLayer);
        //UnityEngine.Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * (Mathf.Abs(GroundCheckSource.localPosition.y) + 0.2f), Color.red);
        
        // ground check: Sphere check approach
        grounded = Physics.CheckSphere(GroundCheckSource.position, GroundCheckRadius, Player.m.groundLayer);

        MyInput();
        SpeedControl();
        Player.m.crouchLogic.hasSpaceAboveHead = !Physics.CheckSphere(Player.m.crouchLogic.CeilingCheck.position, Player.m.crouchLogic.CeilingCheckRadius, Player.m.groundLayer);

        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        // handle jump phases
        if ( isAscending && rb.velocity.y < 0)
        {
            isAscending = false;
            isDescending= true;
        }

        // set the current player speed based on their move type
        currentSpeed = walkSpeed;
        switch (Player.m.MoveType)
        {
            case "walk":
                currentSpeed = walkSpeed;
                break;
            case "run":
                currentSpeed = sprintSpeed;
                break;
            case "crouch":
                currentSpeed = crouchSpeed;
                break;
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

        if (Input.GetKey(KeyCode.LeftControl))
        {
            //Player.m.MoveType = "crouch";

            if (!Player.m.crouchLogic.hasEnteredCrouch)
                Player.m.crouchLogic.enterCrouch();

            return;

        }else if (Player.m.crouchLogic.hasEnteredCrouch)
        {
            //Player.m.MoveType = "crouch";

            if (Player.m.crouchLogic.hasSpaceAboveHead)
                Player.m.crouchLogic.exitCrouch();
            else
                return;
        }

        if ((horizontalInput != 0 || verticalInput != 0))
        {
            Player.m.MoveType = "walk";

            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
            {
                Player.m.MoveType = "run";
            }
        }
        else
        {
            Player.m.MoveType = "stop";
        }

        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            isAscending = true; 
            isDescending = false;
            
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(GroundCheckSource.position, GroundCheckRadius);
    }
}
