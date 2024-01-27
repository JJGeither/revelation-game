using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldingHandler : MonoBehaviour
{

    [SerializeField] public float throwForce;
    [SerializeField] private Canvas canvas; // Reference to the Canvas


    // Update is called once per frame
    void FixedUpdate()
    {
        ThrowObject();
    }

    private Vector3 originalScale;  // Store the original scale

    public void PickupObject(GameObject liftObj)
    {
        // Check if the object is not null
        if (liftObj != null && !liftObj.gameObject.CompareTag("Wall"))
        {
            Transform heldObjectsTransform = transform.Find("HeldObjects");

            if (heldObjectsTransform != null)
            {
                liftObj.transform.SetParent(heldObjectsTransform.transform, true);

                liftObj.transform.localRotation = Quaternion.identity;

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
                float sphereRadius = 1f;
                float scaleFactor = sphereRadius / boundsOfStoredObject.size.magnitude;
                liftObj.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

                // Set the position to the left side of the player
                Vector3 playerLeftPosition = transform.position - transform.right * 0.5f; // Adjust the value based on your preference
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

                // Makes Object throw in the middle
                Vector3 playerLeftPosition = transform.position + transform.right * 0.5f; // Adjust the value based on your preference
                firstHeldObject.transform.position = playerLeftPosition;

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

                // Get the direction the player is looking (from camera's forward vector)
                Vector3 throwDirection = Camera.main.transform.forward;

                // Set the object's rotation to face the throw direction
                firstHeldObject.transform.rotation = Quaternion.LookRotation(throwDirection);

                // Rotate the object slightly to adjust its orientation
                Vector3 adjustedRotation = firstHeldObject.transform.rotation.eulerAngles;
                adjustedRotation.x += 90f; // Adjust this value as needed
                firstHeldObject.transform.rotation = Quaternion.Euler(adjustedRotation);

                // Apply force to the thrown object in the direction the player is looking
                // You may adjust the force magnitude based on your requirements
                thrownObjRigidbody.AddForce(throwDirection * throwForce, ForceMode.Impulse);
            }
        }
    }




}
