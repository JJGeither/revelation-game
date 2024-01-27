using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearMovement : MonoBehaviour
{
    private Rigidbody rb; // Rigidbody component of the spear
    private bool isMoving = true; // Flag to track if the spear is moving
    public GameObject head;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!rb.isKinematic)
        {
            RotateSpear(); // Rotate the spear based on its velocity
        }
    }

    void RotateSpear()
    {
        // Check if the spear has a velocity
        if (rb.velocity.magnitude > 0.1f)
        {
            // Get the velocity direction
            Vector3 targetDirection = rb.velocity.normalized;

            // Ensure no rotation around the y-axis
            targetDirection.y = 0;

            // Check if the direction is not zero (prevents NaN errors)
            if (targetDirection != Vector3.zero)
            {
                // Calculate the rotation towards the target direction using LookRotation
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

                // Rotate the spear by 180 degrees around the forward axis
                targetRotation *= Quaternion.Euler(180f, 0f, 0f);


                // Slerp the current rotation towards the target rotation
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 50f);
            }
        }
    }



    // OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
        {
            // If the collided object has the tag "Player", do not execute further actions
            return;
        }

        string collidedObjectName = collision.gameObject.name;
        Debug.Log("Collided with: " + collidedObjectName);
        rb.isKinematic = true;

        // Stop the spear if it collides with something
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

    }
}
