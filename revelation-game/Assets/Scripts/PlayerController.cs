using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Public Variables")]
    [SerializeField] public float playerMaxSpeed;
    [SerializeField] public float playerMinSpeed;
    [SerializeField] public float acceleration;
    [SerializeField] public float deceleration;
    [SerializeField] public float gravityScale;
    [SerializeField] public float jumpForce;
    [SerializeField] public float cameraRotationSpeed;
    [SerializeField] public ParticleSystem ps_blood;
    [SerializeField] public TextMeshProUGUI healthText; // Reference to the UI Text for displaying health
    public float groundRayLength;

    private bool isJumping = false; // Flag to indicate jump input
    private float _playerSpeed;
    private float _jumpForce = 0; // The current jump force
    private float _health = 100; // The current jump force
    private float _maxHealth = 100; // The current jump force
    private float _speedModifier = 1;

    // Private Variables
    private Rigidbody _rb;
    private bool isGrounded = true;
    private bool isCooldown = false; // Flag for hit cooldown
    [SerializeField] private float cooldownTime = 2f; // Cooldown time in seconds


    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
    }

    public void Update()
    {
        if (isCooldown) { Debug.Log("cooldown"); }
        healthText.text = _health.ToString() + "/" + _maxHealth.ToString();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isJumping = true;
            _jumpForce = jumpForce; // Initialize jump force
            Debug.Log("Space key pressed");
        }

        // Get mouse X input for rotation
        float mouseX = Input.GetAxis("Mouse X");

        // Rotate the player based on the mouse X input
        RotatePlayer(mouseX);
    }

    void FixedUpdate()
    {


        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        // Calculate acceleration based on input
        float targetSpeed = _playerSpeed;

        if (verticalInput != 0.0 || horizontalInput != 0.0)
            targetSpeed += acceleration * Time.fixedDeltaTime * _speedModifier;
        else
            targetSpeed -= deceleration * Time.fixedDeltaTime * _speedModifier;

        // Clamp the speed to the specified range
        targetSpeed = Mathf.Clamp(targetSpeed, playerMinSpeed, playerMaxSpeed * _speedModifier);

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
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, groundRayLength))
            isGrounded = true;
        else
            isGrounded = false;

        if (!isGrounded) 
            _rb.AddForce(new Vector3(0,-gravityScale,0), ForceMode.Acceleration);
    }

    void RotatePlayer(float mouseX)
    {
        mouseX = (mouseX + 180.0f) % 360.0f - 180.0f;
        // Smoothly interpolate between the current rotation and the target rotation
        Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + mouseX, 0);
        transform.rotation = targetRotation;
            //Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }

    private void PlayParticleSystem(ParticleSystem ps)
    {
        Debug.Log("played");
        if (!ps.isPlaying) ps.Play();

        // Calculate duration and stop the particle system after one cycle
        float duration = ps.main.duration;
        Invoke(nameof(StopParticleSystem), duration);
    }

    private void StopParticleSystem()
    {
        ps_blood.Stop(); // Stop the particle system
    }

    public void SetSpeed(float speed)
    {
        _speedModifier *= speed;
        Debug.Log("speed: " + _speedModifier);
    }

    private IEnumerator HitCooldown()
    {
        isCooldown = true; // Set cooldown flag
        yield return new WaitForSeconds(cooldownTime); // Wait for cooldown duration
        isCooldown = false; // Reset cooldown flag after cooldown is over
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!isCooldown && collision.gameObject.CompareTag("Enemy"))
        {
            StartCoroutine(HitCooldown()); // Start the cooldown
            PlayParticleSystem(ps_blood);
            _health -= 10;
            Debug.Log(_health);
        }
    }

}
