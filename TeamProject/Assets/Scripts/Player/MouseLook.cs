using UnityEngine;

/// Handles the directional looking behavior of the player
public class MouseLook : MonoBehaviour
{
    [Header("Mouse Parameters")]
    [SerializeField] private float mouseSensitivity = 300f;
    [Header("Game Objects")]
    [SerializeField] private Transform playerBody;

    private float xRotation;
    private float initialMouseSensitivity;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        initialMouseSensitivity = mouseSensitivity;
    }

    void Update()
    {
        MouseHandler();
    }
    
    /// Moves where the player is looking based on the movement of the mouse
    private void MouseHandler()
    {
        var mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        var mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        // Clamp to prevent rotation around the player
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    public void SetMouseSensitivity(float sensitivity)
    {
        this.mouseSensitivity = sensitivity;
    }

    public float GetInitialMouseSensitivity()
    {
        return initialMouseSensitivity;
    }
}
