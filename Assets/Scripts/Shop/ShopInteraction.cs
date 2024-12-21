using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopInteraction : MonoBehaviour
{
    public Canvas Canvas; // Reference to the shop canvas
    private bool isPlayerInRange = false;

    void Start()
    {
        if (Canvas != null)
        {
            Canvas.enabled = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Entered: " + other.gameObject.name);
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player detected in trigger!");
            isPlayerInRange = true;
            if (Canvas != null)
                Canvas.enabled = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger Exited: " + other.gameObject.name);
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited trigger!");
            isPlayerInRange = false;
            if (Canvas != null)
                Canvas.enabled = false;
        }
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Player interacting with the shop...");

        }
    }
}

