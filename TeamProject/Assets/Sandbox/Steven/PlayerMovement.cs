using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    [Header("Movement")] [SerializeField] public float speed = 12f;
    [SerializeField] public float gravity = -9.81f;
    [SerializeField] public float jumpHeight = 3f;

    [Header("Ground Detection")] [SerializeField]
    private Transform groundCheck;

    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    [Header("Head bob")] [SerializeField] public float bobAmount;
    [SerializeField] public float walkBobSpeed;
    
    private Camera playerCam;
    
    private float timer;
    private float defaultYPosition;
    
    private Vector3 velocity;
    private Vector3 movementDirection;

    private bool isGrounded;
    
    private void Start()
    {
        playerCam = GetComponentInChildren<Camera>();
        defaultYPosition = playerCam.transform.localPosition.y;
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        movementDirection = transform.right * x + transform.forward * z;

        controller.Move(movementDirection * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        HeadBob();
    }

    private void HeadBob()
    {
        if (!isGrounded) return;

        if (Mathf.Abs(movementDirection.x) > 0.1f || Mathf.Abs(movementDirection.z) > 0.1f)
        {
            timer += Time.deltaTime * walkBobSpeed;
            playerCam.transform.localPosition = new Vector3(
                playerCam.transform.localPosition.x,
                defaultYPosition + Mathf.Sin(timer) * bobAmount,
                playerCam.transform.localPosition.z
            );
        }
    }
}