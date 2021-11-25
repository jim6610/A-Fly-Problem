using System.Collections;
using UnityEngine;

/// Handles all functionality related to player movement behavior
public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public AudioSource walkSoundEffect;
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

    [SerializeField] private GameObject poisonOverlay;
    
    private Camera playerCam;
    
    private float timer;
    private float defaultYPosition;
    private bool reverseMovment;
    
    private Vector3 velocity;
    private Vector3 movementDirection;

    private bool isGrounded;
    private bool duringCrouchAnimation;
    
    public bool IsCrouching { get; private set; }

    private float startingWalkSpeed;
    private float startingCrouchSpeed;

    private void Start()
    {
        playerCam = GetComponentInChildren<Camera>();
        defaultYPosition = playerCam.transform.localPosition.y;
        reverseMovment = false;
        startingWalkSpeed = walkSpeed;
        startingCrouchSpeed = crouchSpeed;
    }

    void Update()
    {
        GroundCheckHandler();

        MovementHandler();
        
        HandleCrouch();

        HeadBobHandler();

        //WalkSound();
    }

    /// Handle moving the player based on the directional keys pressed
    void MovementHandler()
    {
        Transform _transform = transform;
        
        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical");

        if(reverseMovment)
        {
            x = -x;
            z = -z;
        }

        movementDirection = _transform.right * x + _transform.forward * z;

        var _speed = IsSprinting ? sprintSpeed : (IsCrouching ? crouchSpeed : walkSpeed);

        controller.Move(movementDirection * _speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        
        controller.Move(velocity * Time.deltaTime);
    }
    
    /// Check that the player is touching the ground to keep the y velocity from infinitely growing
    void GroundCheckHandler()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        // Keep the y velocity from infinitely decreasing while we are actually grounded
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }
    
    /// Fires the coroutine if the player is able to crouch
    private void HandleCrouch()
    {
        if (ShouldCrouch)
            StartCoroutine(CrouchStand());
    }
    
    /// Handles the transition "animation" of going from crouching => standing and vice-versa
    private IEnumerator CrouchStand()
    {
        // If the player crouches under something that should stop them from getting back up
        // if (isCrouching && Physics.Raycast(playerCam.transform.position, Vector3.up, 1f))
        //    yield break;
        
        duringCrouchAnimation = true;

        float timeElapsed = 0;
        float targetHeight = IsCrouching ? standingHeight : crouchingHeight;
        float currentHeight = controller.height;

        Vector3 targetCenter = IsCrouching ? standingCenter : crouchingCenter;
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

        IsCrouching = !IsCrouching;

        duringCrouchAnimation = false;
    }

    /// Tilt the player camera based on the players movement speed
    private void HeadBobHandler()
    {
        // Don't bob if the player is jumping/falling
        if (!isGrounded) return;

        if (Mathf.Abs(movementDirection.x) > 0.1f || Mathf.Abs(movementDirection.z) > 0.1f)
        {
            Vector3 localPosition = playerCam.transform.localPosition;

            float bobSpeed = IsSprinting ? sprintBobSpeed : (IsCrouching ? crouchBobSpeed : walkBobSpeed);
            
            timer += Time.deltaTime * bobSpeed;
            
            playerCam.transform.localPosition = new Vector3(
                localPosition.x,
                defaultYPosition + Mathf.Sin(timer) * bobAmount,
                localPosition.z
            );
        }
    }

    private void WalkSound()
    {
        if (controller.isGrounded == true && controller.velocity.magnitude > 0 && walkSoundEffect.isPlaying == false)
        {
            walkSoundEffect.Play();
        }
    }

    public void StartReverseMovement(float time)
    {
        StartCoroutine(ApplyMovementReverse(time));
    }

    private IEnumerator ApplyMovementReverse(float time)
    {
        reverseMovment = true;
        poisonOverlay.SetActive(true);
        yield return new WaitForSeconds(time);
        reverseMovment = false;
        poisonOverlay.SetActive(false);
    }

    public void StartMovementSlowdown(float time)
    {
        StartCoroutine(ApplyMovementSlowdown(time));
    }

    private IEnumerator ApplyMovementSlowdown(float time)
    {
        walkSpeed = startingWalkSpeed/3.0f;
        crouchSpeed = startingCrouchSpeed/3.0f;
        canSprint = false;

        MouseLook ml = this.transform.GetComponentInChildren<MouseLook>();
        if(ml)
            ml.SetMouseSensitivity(50f);

        yield return new WaitForSeconds(time);

        walkSpeed = startingWalkSpeed;
        crouchSpeed = startingCrouchSpeed;
        canSprint = true;

        if (ml)
            ml.SetMouseSensitivity(50f);


    }
}