using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteractable : MonoBehaviour { 
    public GameObject useButton; // Assign in Inspector
    public EmailDisplayManager emailDisplayManager; // Assign in Inspector
    public EmailManager emailManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Adjust if necessary
        {
            useButton.SetActive(true); // Show the "Use" button
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            useButton.SetActive(false); // Hide the "Use" button
        }
    }

   
    public void OnUsePressed()
    {
    }

}


