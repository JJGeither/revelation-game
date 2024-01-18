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
    [SerializeField] public float throwForce;
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
        ThrowObject();

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

    private Vector3 originalScale;  // Store the original scale

    public void PickupObject(GameObject liftObj)
    {
        // Check if the object is not null
        if (liftObj != null)
        {
            Transform heldObjectsTransform = transform.Find("HeldObjects");

            if (heldObjectsTransform != null)
            {
                liftObj.transform.SetParent(heldObjectsTransform.transform, true);

                // Store the original scale before modifying it
                originalScale = liftObj.transform.localScale;

                // Get the bounds of the stored object
                var boundsOfStoredObject = new Bounds(Vector3.zero, Vector3.zero);
                Collider liftObjCollider = liftObj.GetComponent<Collider>();

                if (liftObjCollider != null)
                {
                    boundsOfStoredObject = liftObjCollider.bounds;
                }

                // Set the object's scale to fit within the sphere
                float sphereRadius = 10f;
                float scaleFactor = sphereRadius / boundsOfStoredObject.size.magnitude;
                liftObj.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

                // Set the position to the left side of the player
                Vector3 playerLeftPosition = transform.position - transform.right * 1.0f; // Adjust the value based on your preference
                liftObj.transform.position = playerLeftPosition;

                if (liftObjCollider != null)
                {
                    // Disable the collider while the object is held
                    liftObjCollider.enabled = false;
                }

                // Check if the object has a Rigidbody component
                Rigidbody liftObjRigidbody = liftObj.GetComponent<Rigidbody>();
                if (liftObjRigidbody != null)
                {
                    // Set the object to be kinematic
                    liftObjRigidbody.isKinematic = true;
                }
            }
        }
    }

    void ThrowObject()
    {
        // Check if the left mouse button is pressed
        if (Input.GetMouseButtonDown(0))
        {
            // Find the child object named "HeldObjects"
            Transform heldObjectsTransform = transform.Find("HeldObjects");

            // Check if HeldObjects exists and has at least one child
            if (heldObjectsTransform != null && heldObjectsTransform.childCount > 0)
            {
                // Get the first object held
                GameObject firstHeldObject = heldObjectsTransform.GetChild(0).gameObject;

                // Detach the object from HeldObjects
                firstHeldObject.transform.parent = null;

                // Enable the collider of the thrown object
                Collider thrownObjCollider = firstHeldObject.GetComponent<Collider>();
                if (thrownObjCollider != null)
                {
                    thrownObjCollider.enabled = true;
                }

                // Add a Rigidbody component if not already present
                Rigidbody thrownObjRigidbody = firstHeldObject.GetComponent<Rigidbody>();
                if (thrownObjRigidbody == null)
                {
                    thrownObjRigidbody = firstHeldObject.AddComponent<Rigidbody>();
                }

                // Make the object not kinematic
                if (thrownObjRigidbody != null)
                {
                    thrownObjRigidbody.isKinematic = false;
                }

                // Reset the scale to the original scale
                firstHeldObject.transform.localScale = originalScale;

                // Ignore collisions with the player
                Physics.IgnoreCollision(firstHeldObject.GetComponent<Collider>(), GetComponent<Collider>());

                // Get the direction the player is looking
                Vector3 throwDirection = Camera.main.transform.forward;

                // Apply force to throw the object in the direction the player is looking
                thrownObjRigidbody.AddForce(throwDirection * throwForce, ForceMode.Impulse);
            }
        }
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
