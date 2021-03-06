using System.Collections;
using UnityEngine;

/// Handles all functionality related to player movement behavior
public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    private bool ShouldCrouch => Input.GetKeyDown(KeyCode.LeftControl) && !duringCrouchAnimation && isGrounded;
    public bool IsSprinting => canSprint && Input.GetKey(KeyCode.LeftShift) && isGrounded;

    [Header("Movement")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float crouchSpeed = 2f;
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

    [Header("Head Bob")] 
    [SerializeField] private float bobAmount;
    [SerializeField] private float crouchBobSpeed = 10f;
    [SerializeField] private float walkBobSpeed = 14f;
    [SerializeField] private float sprintBobSpeed = 16f;

    [Header("Spider Web Debuff Effect")]
    [SerializeField] private float webDebuffDuration = 3.0f;
    [SerializeField] private float reducedWalkSpeed = 3f;
    [SerializeField] private float reducedCrouchSpeed = 1f;
    [SerializeField] private float reducedLookSpeed = 40f;

    [Header("Scorpion Poison Debuff Effect")]
    [SerializeField] private float poisonDebuffDuration = 4.0f;

    [Header("FX")]
    [SerializeField] private GameObject poisonOverlay;
    
    private Camera playerCam;
    private AudioManager audioManager;

    private float timer;
    private float defaultYPosition;
    private bool reverseMovment;
    
    private Vector3 velocity;
    private Vector3 movementDirection;

    private bool isGrounded;
    private bool duringCrouchAnimation;
    
    public bool IsCrouching { get; private set; }

    private float initialWalkSpeed;
    private float initialCrouchSpeed;

    private Light playerLight;

    private void Start()
    {
        playerLight = GetComponentInChildren<Light>();
        playerCam = GetComponentInChildren<Camera>();
        audioManager = FindObjectOfType<AudioManager>();
        defaultYPosition = playerCam.transform.localPosition.y;
        reverseMovment = false;
        initialWalkSpeed = walkSpeed;
        initialCrouchSpeed = crouchSpeed;
    }

    void Update()
    {
        GroundCheckHandler();

        MovementHandler();
        
        HandleCrouch();

        HeadBobHandler();
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


    public void StartReverseMovement()
    {
        audioManager.Play("Hurt");
        StartCoroutine(ApplyMovementReverse(poisonDebuffDuration));
    }

    private IEnumerator ApplyMovementReverse(float time)
    {
        reverseMovment = true;
        poisonOverlay.SetActive(true);
        yield return new WaitForSeconds(time);
        reverseMovment = false;
        poisonOverlay.SetActive(false);
    }

    public void StartMovementSlowdown()
    {
        audioManager.Play("Webbed");
        StartCoroutine(ApplyMovementSlowdown(webDebuffDuration));
    }

    private IEnumerator ApplyMovementSlowdown(float time)
    {
        walkSpeed = reducedWalkSpeed;
        crouchSpeed = reducedCrouchSpeed;
        canSprint = false;

        MouseLook ml = this.transform.GetComponentInChildren<MouseLook>();
        if(ml)
            ml.SetMouseSensitivity(reducedLookSpeed);

        yield return new WaitForSeconds(time);

        walkSpeed = initialWalkSpeed;
        crouchSpeed = initialCrouchSpeed;
        canSprint = true;

        if (ml)
            ml.SetMouseSensitivity(ml.GetInitialMouseSensitivity());
    }

    public void ToggleLight()
    {
        playerLight.enabled = !playerLight.enabled;
    }
}