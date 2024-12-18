using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopInteraction : MonoBehaviour
{
    public Canvas shopCanvas; // Reference to the shop canvas
    public GameObject interactionPrompt; // Optional: "Press E to interact" UI
    private bool isPlayerInRange = false;

    void Start()
    {
        if (shopCanvas != null)
        {
            shopCanvas.enabled = false; // Ensure the Canvas is hidden at the start
        }

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false); // Ensure the prompt is hidden at the start
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the Player enters the trigger
        {
            isPlayerInRange = true;

            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(true); // Show the prompt
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the Player exits the trigger
        {
            isPlayerInRange = false;

            if (shopCanvas != null)
            {
                shopCanvas.enabled = false; // Hide the Canvas if active
            }

            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false); // Hide the prompt
            }
        }
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E)) // Check for interaction key
        {
            if (shopCanvas != null)
            {
                shopCanvas.enabled = !shopCanvas.enabled; // Toggle the Canvas
            }
        }
    }
}
