using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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


    public Image reticleImage;
    public TextMeshProUGUI interactText;

    private Transform playerTransform;
    private float currentRotationAngleX = 0.0f;
    private float currentRotationAngleY = 0.0f;

    void Start()
    {
        playerTransform = target.transform;
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
        Cursor.visible = false; // Hide the cursor
    }

    private void Update()
    {
        transform.position = playerTransform.position + offset;
        // Adjust the camera's y rotation based on mouse y input
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;
        currentRotationAngleY -= mouseY;

        // Clamp the y rotation angle to limit between -45 and 45 degrees
        currentRotationAngleY = Mathf.Clamp(currentRotationAngleY, -80f, 60f);

        // Calculate the desired camera rotation
        Quaternion newRotation = Quaternion.Euler(currentRotationAngleY, playerTransform.eulerAngles.y, 0f);

        transform.rotation = newRotation;

        ReticleInteraction();
    }

    void ReticleInteraction()
    {
        int wallLayerMask = 1 << LayerMask.NameToLayer("Interactable");

        Ray ray = Camera.main.ScreenPointToRay(reticleImage.transform.position);
        RaycastHit hit;

        // Use the layer mask to check for collisions with walls in the raycast
        if (Physics.Raycast(ray, out hit, 5, wallLayerMask))
        {
            //Debug.Log("HIT: " + hit.collider.name);
            interactText.enabled = true;
            PlayerInteraction playerInteraction = hit.collider.GetComponent<PlayerInteraction>();
            // Check for player input to trigger interaction
            if (Input.GetKeyDown(KeyCode.E))
            {
                playerInteraction.Interact();
            }

        }
        else
        {
            interactText.enabled = false;
        }
    }

}
