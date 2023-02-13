using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using TMPro;
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
    public bool isGrounded;


    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    [Header("Slide")]
    public float SlideDuration = .5f;
    public float SlideSpeed = 18;
    public float slideCooldwon = 2;
    public bool CanSlide = true;
    private IEnumerator SlideCoroutine;

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
        isGrounded = Physics.CheckSphere(GroundCheckSource.position, GroundCheckRadius, Player.m.groundLayer);

        MovementInputs();
        SpeedControl();
        Player.m.crouchLogic.hasSpaceAboveHead = !Physics.CheckSphere(Player.m.crouchLogic.CeilingCheck.position, Player.m.crouchLogic.CeilingCheckRadius, Player.m.groundLayer);

        // handle drag
        if (isGrounded || Player.m.MoveType == "slide")
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

    private void MovementInputs()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Player.m.MoveType == "slide" )
        {
            print("slide");
            return;
        }

        //(horizontalInput != 0 || verticalInput != 0) && Input.GetKey(KeyCode.LeftShift)
        if (CanSlide && Player.m.MoveType == "run" && Input.GetKey(KeyCode.LeftControl))
        {
            CanSlide = false;
            SlideCoroutine = SlideAction(SlideSpeed, SlideDuration);
            StartCoroutine(SlideCoroutine);

            return;
        }

        if (Input.GetKey(KeyCode.LeftControl) && isGrounded)
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
        if (Input.GetKey(jumpKey) && readyToJump && isGrounded)
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

        if (Player.m.MoveType == "slide")
            return;

        //calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
    
        // on ground
        if (isGrounded)
            rb.AddForce(moveDirection.normalized * currentSpeed * 10f, ForceMode.Force);
        
        // in air
        else if (!isGrounded)
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

    private void ResetJump() { readyToJump = true; }

    private void ResetSlideCoolDown() { CanSlide = true; }

    private void HandleGravity()
    {
        if (!isGrounded)
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

    private IEnumerator SlideAction(float speed, float duration)
    {
        
        float time = 0.0f;

        Player.m.crouchLogic.enterCrouch();

        Player.m.MoveType = "slide";

        //reset player velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        do
        {
            time += Time.deltaTime;

            rb.AddForce(moveDirection.normalized * speed * 10f, ForceMode.Force);

            yield return 0;

        } while (time < duration);

        if (Player.m.crouchLogic.hasSpaceAboveHead)
            Player.m.crouchLogic.exitCrouch();
        else 
            Player.m.MoveType = "crouch";

        Invoke(nameof(ResetSlideCoolDown), slideCooldwon);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(GroundCheckSource.position, GroundCheckRadius);
    }
}
