using UnityEngine;

public class InteractableObject : MonoBehaviour
{
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

    // Call this from the "Use" button's OnClick event in the Inspector
    public void OnUsePressed()
    {
            if (emailDisplayManager.incorrectlyIdentifiedEmails.Count > 0 && !emailDisplayManager.IsEmailsDisplayActive())
            {
                emailDisplayManager.EnterReviewMode();
            }
            else
            {
                emailDisplayManager.DisplayRandomEmail(); 
            }
        }
    }


