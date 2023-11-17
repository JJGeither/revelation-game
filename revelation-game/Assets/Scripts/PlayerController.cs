using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Public Variables")]
    [SerializeField] public float playerMaxSpeed;
    [SerializeField] public float playerMinSpeed;
    [SerializeField] public float acceleration;
    [SerializeField] public float deceleration;
    [SerializeField] public float gravityScale;
    [SerializeField] public float jumpForce;
    [SerializeField] public float rotationSpeed;
    public float groundRayLength;

    private bool isJumping = false; // Flag to indicate jump input
    private float _playerSpeed;
    private float _jumpForce = 0; // The current jump force

    // Private Variables
    private Rigidbody _rb;
    private bool isGrounded = true;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isJumping = true;
            _jumpForce = jumpForce; // Initialize jump force
            Debug.Log("Space key pressed");
        }
    }

    void FixedUpdate()
    {
        // Get mouse X input for rotation
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;

        // Rotate the player based on the mouse X input
        RotatePlayer(mouseX);

        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        // Calculate acceleration based on input
        float targetSpeed = _playerSpeed;

        if (verticalInput != 0.0 || horizontalInput != 0.0)
            targetSpeed += acceleration * Time.fixedDeltaTime;
        else
            targetSpeed -= deceleration * Time.fixedDeltaTime;

        // Clamp the speed to the specified range
        targetSpeed = Mathf.Clamp(targetSpeed, playerMinSpeed, playerMaxSpeed);

        // Smoothly adjust the playerSpeed towards the targetSpeed
        _playerSpeed = Mathf.MoveTowards(_playerSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);

        // Clamping input values
        float clampedVerticalInput = Mathf.Clamp(verticalInput, -1f, 1f);
        float clampedHorizontalInput = Mathf.Clamp(horizontalInput, -1f, 1f);

        float verticalMovement = clampedVerticalInput * _playerSpeed * Time.fixedDeltaTime;
        float horizontalMovement = clampedHorizontalInput * _playerSpeed * Time.fixedDeltaTime;

        Vector3 movement = new Vector3(horizontalMovement, 0.0f, verticalMovement);

        movement = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * movement;

        // Apply both jump force and movement velocity
        Vector3 totalVelocity = movement + Vector3.up * _jumpForce;
        _rb.velocity = totalVelocity;

        if (isJumping)
        {
            if (!Input.GetKey(KeyCode.Space))
                _jumpForce = _jumpForce / 2;

            // Gradually decrease jumpForce to make it smoother
            _jumpForce -= Time.fixedDeltaTime * 50;

            if (_jumpForce <= 0)
            {
                isJumping = false;
                _jumpForce = 0;
            }
        }

        int layerMask = 1 << 3;


        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, groundRayLength, layerMask))
            isGrounded = true;
        else
            isGrounded = false;

        _rb.AddForce(Physics.gravity * gravityScale, ForceMode.Acceleration);
    }

    void RotatePlayer(float mouseX)
    {
        mouseX = (mouseX + 180.0f) % 360.0f - 180.0f;
        // Smoothly interpolate between the current rotation and the target rotation
        Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + mouseX, 0);
        transform.rotation = targetRotation;
            //Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }
}
