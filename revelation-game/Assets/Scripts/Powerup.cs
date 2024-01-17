using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    private Animator anim;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetTrigger("powerupSpawn");
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ApplyPowerup()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Check if the colliding object has a script
            PlayerController playerScript = other.GetComponent<PlayerController>();

            if (playerScript != null)
            {
                // Call a function from the PlayerScript
                playerScript.SetSpeed(1.1f);

                Debug.Log("Successfully called function from PlayerScript");
            }

            // Destroy this GameObject
            Destroy(gameObject);
        }
    }


}