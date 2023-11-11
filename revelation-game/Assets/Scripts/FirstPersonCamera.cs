using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public GameObject target;
    public float smoothSpeed = 0.125f;
    public Vector3 locationOffset;
    public float distance; // The fixed distance from the target
    public float rotationSpeed; // Speed of mouse X rotation
    public float maxDistance = 5.0f; // Maximum distance from the target
    public LayerMask obstacleLayer; // Layer mask for obstacles
    public Vector3 offset;

    private Transform playerTransform;
    private float currentRotationAngleX = 0.0f;
    private float currentRotationAngleY = 0.0f;

    void Start()
    {
        playerTransform = target.transform;
    }

    void FixedUpdate()
    {
        playerTransform = target.transform;

        // Adjust the camera's y rotation based on mouse y input
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;
        currentRotationAngleY -= mouseY;

        // Clamp the y rotation angle to limit between -45 and 45 degrees
        currentRotationAngleY = Mathf.Clamp(currentRotationAngleY, -45f, 20f);

        // Calculate the desired camera rotation
        Quaternion newRotation = Quaternion.Euler(currentRotationAngleY, playerTransform.eulerAngles.y, 0f);

        // Smoothly interpolate towards the desired rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, smoothSpeed * Time.fixedDeltaTime);

        // Set the camera position to the player's position
        transform.position = playerTransform.position + offset;
    }
}
