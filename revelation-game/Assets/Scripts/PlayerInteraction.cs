using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private InteractableElement interactableElement;
    public GameObject powerup;
    public enum InteractableType
    {
        Base,
        Chest
    }

    [SerializeField]
    private InteractableType interactableType;

    // Start is called before the first frame update
    void Start()
    {

        CreateInteractable();
    }

    // Method to create an instance based on the selected type
    private void CreateInteractable()
    {
        switch (interactableType)
        {
            case InteractableType.Base:
                interactableElement = new InteractableElement();
                break;
            case InteractableType.Chest:
                interactableElement = new Chest(gameObject, powerup);
                break;
            default:
                interactableElement = new InteractableElement();
                break;
        }
    }

    public void Interact()
    {
        interactableElement.Interact();
    }

    public class InteractableElement
    {
        public virtual void Interact()
        {
            Debug.Log("Interacting with base element");
        }
    }

    public class Chest : InteractableElement
    {
        bool open = false;
        GameObject p;
        GameObject callingObject;

        public Chest(GameObject callingObject, GameObject powerup)
        {
            this.callingObject = callingObject;
            p = powerup;
        }
        // Override the Interact method
        public override void Interact()
        {
            if (!open)
            {
                open = true;
                Debug.Log("Interacting with chest");
                GameObject t = Instantiate(p, callingObject.transform);
            }
        }
    }


}
