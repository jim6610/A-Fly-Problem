using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Handles all functionality related to player movement behavior
 */
public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    private bool ShouldCrouch => Input.GetKeyDown(KeyCode.LeftControl) && !duringCrouchAnimation && isGrounded;
    public bool IsSprinting => canSprint && Input.GetKey(KeyCode.LeftShift) && isGrounded;

    [Header("Movement")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float walkSpeed = 12f;
    [SerializeField] private float sprintSpeed = 14f;
    [SerializeField] private float crouchSpeed = 6f;
    [SerializeField] private bool canSprint = true;
    [Header("Crouch Parameters")]
    [SerializeField] private float crouchingHeight = 0.5f;
    [SerializeField] private float standingHeight = 3f;
    [SerializeField] private float timeToCrouch = 0.25f;
    [SerializeField] private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 standingCenter = new Vector3(0, 0f, 0);

    [Header("Ground Detection")] 
    [SerializeField] private Transform groundCheck;

    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    [Header("Head Bob")] [SerializeField] public float bobAmount;
    [SerializeField] private float crouchBobSpeed = 10f;
    [SerializeField] private float walkBobSpeed = 14f;
    [SerializeField] private float sprintBobSpeed = 16f;
    
    private Camera playerCam;
    
    private float timer;
    private float defaultYPosition;
    
    private Vector3 velocity;
    private Vector3 movementDirection;

    private bool isGrounded;
    
    private bool isCrouching;

    public bool IsCrouching => isCrouching;
    
    private bool duringCrouchAnimation;

    private void Start()
    {
        playerCam = GetComponentInChildren<Camera>();
        defaultYPosition = playerCam.transform.localPosition.y;
    }

    void Update()
    {
        GroundCheckHandler();

        MovementHandler();
        
        HandleCrouch();

        HeadBobHandler();
    }

    void MovementHandler()
    {
        Transform _transform = transform;
        
        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical");

        movementDirection = _transform.right * x + _transform.forward * z;

        var _speed = IsSprinting ? sprintSpeed : (isCrouching ? crouchSpeed : walkSpeed);

        controller.Move(movementDirection * _speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        
        controller.Move(velocity * Time.deltaTime);
    }
    
    void GroundCheckHandler()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        // Keep the y velocity from infinitely decreasing while we are actually grounded
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    private void HandleCrouch()
    {
        if (ShouldCrouch)
            StartCoroutine(CrouchStand());
    }

    private IEnumerator CrouchStand()
    {
        // If the player crouches under something that should stop them from getting back up
        //if (isCrouching && Physics.Raycast(playerCam.transform.position, Vector3.up, 1f))
        //    yield break;
        
        duringCrouchAnimation = true;

        float timeElapsed = 0;
        float targetHeight = isCrouching ? standingHeight : crouchingHeight;
        float currentHeight = controller.height;

        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = controller.center;

        while (timeElapsed < timeToCrouch)
        {
            controller.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
            controller.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        controller.height = targetHeight;
        controller.center = targetCenter;

        isCrouching = !isCrouching;

        duringCrouchAnimation = false;
    }

    /**
     * Tilt the player camera based on the players movement speed
     */
    private void HeadBobHandler()
    {
        // Don't bob if the player is jumping/falling
        if (!isGrounded) return;

        if (Mathf.Abs(movementDirection.x) > 0.1f || Mathf.Abs(movementDirection.z) > 0.1f)
        {
            Vector3 localPosition = playerCam.transform.localPosition;

            float bobSpeed = IsSprinting ? sprintBobSpeed : (isCrouching ? crouchBobSpeed : walkBobSpeed);
            
            timer += Time.deltaTime * bobSpeed;
            
            playerCam.transform.localPosition = new Vector3(
                localPosition.x,
                defaultYPosition + Mathf.Sin(timer) * bobAmount,
                localPosition.z
            );
        }
    }
}