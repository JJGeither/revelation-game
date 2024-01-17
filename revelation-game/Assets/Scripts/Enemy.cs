using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private int health; // Initial health of the enemy
    public int maxHealth = 100; // Initial health of the enemy
    public string playerTag = "Player"; // Tag of the player GameObject
    public ParticleSystem ps_blood;
    public ParticleSystem ps_smoke;
    public float movementSpeed = 5f; // Enemy movement speed towards the player
    public float stopDistance = 1f; // Distance at which the enemy stops moving
    public float jumpForce = 5f; // Force applied when jumping
    public float hitForce = 10f; // Force applied when hit
    public TextMeshProUGUI healthText; // Reference to the UI Text for displaying health


    private Rigidbody rb;
    private Transform playerTransform; // Reference to the player's transform
    private bool canMove = false; // Flag to determine if the enemy can move
    private bool hasBeenHit = false; // Flag to track if enemy has been hit

    private void Start()
    {
        health = maxHealth;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // Set Rigidbody as kinematic initially
        ToggleKinematic();

        // Find the player GameObject by tag
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("Player not found!");
        }
    }

    private void Update()
    {
        healthText.text = health.ToString() + "/" + maxHealth.ToString();

        if (playerTransform != null)
        {
            // Rotate enemy to face the player
            Vector3 lookDirection = playerTransform.position - transform.position;
            lookDirection.y = 0f; // Ensure the enemy doesn't tilt upwards or downwards
            Quaternion rotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = rotation;
        }
    }

    private void FixedUpdate()
    {
        if (canMove && playerTransform != null)
        {
            MoveTowardsPlayer();
        }
    }

    private void ToggleKinematic()
    {
        canMove = !canMove;
        rb.isKinematic = !canMove;
    }

    private void MoveTowardsPlayer()
    {
        Vector3 direction = playerTransform.position - transform.position;
        float distance = direction.magnitude;

        if (distance > stopDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, movementSpeed * Time.deltaTime);

        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 4))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                Jump();
            }
        }

    }

    private void Jump()
    {
        rb.velocity = Vector3.zero; // Reset vertical velocity before jumping
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Apply jump force once
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon"))
        {
            PlayParticleSystem(ps_blood);
            health -= 10;
            Debug.Log(health);

            if (health <= 0)
            {
                PlayParticleSystem(ps_smoke);
                Destroy(gameObject);
            }
            else
            {
                // Apply force backward and upward when hit
                Vector3 forceDirection = (transform.position - other.transform.position).normalized + new Vector3(0, .3F, 0);
                rb.AddForce(forceDirection * hitForce, ForceMode.Impulse);
            }
        }
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
        ps_smoke.Stop(); // Stop the particle system
    }

}
