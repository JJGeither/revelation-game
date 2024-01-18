using UnityEngine;

public class GravityController : MonoBehaviour
{
    // Set your desired gravity vector here
    public Vector3 customGravity = new Vector3(0, -9.81f, 0);

    void Start()
    {
        // Set the custom gravity when the script starts
        Physics.gravity = customGravity;
    }

    // You can change gravity dynamically during runtime if needed
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            // Example: Invert gravity on key press
            Physics.gravity = -Physics.gravity;
        }
    }
}
