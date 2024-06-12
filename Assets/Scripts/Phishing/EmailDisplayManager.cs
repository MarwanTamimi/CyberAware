using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class EmailDisplayManager : MonoBehaviour
{
    public TextMeshProUGUI subjectText;
    public TextMeshProUGUI senderText;
    public TextMeshProUGUI bodyText;
    public Button phishingButton;
    public Button notPhishingButton;
    public EmailManager emailManager;

    public GameObject emailPanel; // Reference to the whole email panel
    public GameObject confirmationPanel; // The pop-up panel for confirmation
    public Button yesButton; // The 'Yes' button on the confirmation panel
    public Button noButton; // The 'No' button on the confirmation panel
    public Button reviewMistakesButton;
    public Button resetButton;
    public Button closeButton;
   // public Button resumeButton;

    public TextMeshProUGUI reviewModeIndicator;

    public AchievementID achievementToUnlock;

    private List<Email> emailsToDisplay = new List<Email>();
    public List<Email> incorrectlyIdentifiedEmails = new List<Email>();

    private int correctResponses = 0;
    private int totalResponses = 0;

    public Button forwardButton;
    public Button backButton;
    public int currentEmailIndex = 0;

    public Dictionary<int, bool?> userResponses = new Dictionary<int, bool?>();


    void Start()
    {
       // resumeButton.onClick.AddListener(ResumeChallenge);
        //closeButton.onClick.AddListener(CloseChallenge);
        resetButton.onClick.AddListener(ResetChallenge);
        forwardButton.onClick.AddListener(GoToNextEmail);
        backButton.onClick.AddListener(GoToPreviousEmail);

        // Initially, there may not be a previous email to go back to
        backButton.interactable = false;
        // Initialize buttons and display the first email
        phishingButton.onClick.AddListener(() => RespondToEmail(true));
        notPhishingButton.onClick.AddListener(() => RespondToEmail(false));
        reviewMistakesButton.onClick.AddListener(EnterReviewMode);

        InitializeEmailsToDisplay();
        DisplayRandomEmail();

        // Initially hide review mode UI elements
        reviewMistakesButton.gameObject.SetActive(false);
        reviewModeIndicator.gameObject.SetActive(false);
    }

    void InitializeEmailsToDisplay()
    {
        
        emailsToDisplay.AddRange(emailManager.emailList.emails);
        ShuffleEmails(); // Randomize the order of emails
    }

    void ShuffleEmails()
    {
        for (int i = 0; i < emailsToDisplay.Count; i++)
        {
            int randomIndex = Random.Range(i, emailsToDisplay.Count);
            Email temp = emailsToDisplay[i];
            emailsToDisplay[i] = emailsToDisplay[randomIndex];
            emailsToDisplay[randomIndex] = temp;
        }
    }

    public void DisplayRandomEmail()
    {
        if (emailsToDisplay.Count > 0)
        {
            Email currentEmail = emailsToDisplay[0];
            subjectText.text = currentEmail.subject;
            senderText.text = currentEmail.sender;
            bodyText.text = currentEmail.body;
        }
        else
        {
            // Handle case when there are no emails left to display
            Debug.Log("No more emails to display.");
            emailPanel.SetActive(false); // Optionally hide the panel
            reviewModeIndicator.gameObject.SetActive(false); // Hide review mode indicator
        }
    }


    void RespondToEmail(bool markedAsPhishing)
    {
        if (currentEmailIndex < emailsToDisplay.Count)
        {
            userResponses[currentEmailIndex] = markedAsPhishing;
            Email currentEmail = emailsToDisplay[currentEmailIndex];

            bool isResponseCorrect = currentEmail.isPhishing == markedAsPhishing;

            // Log whether the response is correct or incorrect
            if (isResponseCorrect)
            {
                Debug.Log($"Correct response to email: {currentEmail.subject}");
            }
            else
            {
                Debug.Log($"Incorrect response to email: {currentEmail.subject}");
                if (!incorrectlyIdentifiedEmails.Contains(currentEmail))
                {
                    incorrectlyIdentifiedEmails.Add(currentEmail);
                }
            }

            // Update total responses count and correct responses count based on the user's answer
            totalResponses++;
            if (isResponseCorrect) correctResponses++;

            // Check if it's not the last email before attempting to go to the next one
            if (currentEmailIndex < emailsToDisplay.Count - 1)
            {
                currentEmailIndex++;
                DisplayEmailAtIndex(currentEmailIndex); // Ensure we display the next email
            }
            else
            {
                // If it's the last email, prepare to confirm submission
                ConfirmSubmission();
            }

            UpdateNavigationButtons();
        }
    }



    void ConfirmSubmission()
    {
        confirmationPanel.SetActive(true);
        emailPanel.GetComponent<CanvasGroup>().interactable = false;

        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(SubmitResponses);
        noButton.onClick.AddListener(CancelSubmission);
    }

    void SubmitResponses()
    {
        confirmationPanel.SetActive(false);
        emailPanel.GetComponent<CanvasGroup>().interactable = true;

        // Calculate correct responses based on userResponses
        CalculateCorrectResponses();
        emailPanel.SetActive(false);
        UnlockAchievement();

        //if (CheckAchievementCriteria())
        //{
        //    UnlockAchievement();
        //    emailPanel.SetActive(false);
        //}
        //else
        //{
        //    Debug.Log("Achievement criteria not met.");
        //}

        if (incorrectlyIdentifiedEmails.Count > 0)
        {
            reviewMistakesButton.gameObject.SetActive(true);
        }
    }

    void CancelSubmission()
    {
        confirmationPanel.SetActive(false);
        emailPanel.GetComponent<CanvasGroup>().interactable = true;
    }

    bool CheckAchievementCriteria()
    {
        return ((float)correctResponses / totalResponses) >= 0.8f;
    }

    void UnlockAchievement()
    {
        if (AchievementManager.Instance != null && CheckAchievementCriteria())
        {
            AchievementManager.Instance.UnlockAchievement(achievementToUnlock);
            Debug.Log("Achievement unlocked!");
        }
        else
        {
            Debug.LogError("Achievement criteria not met or AchievementManager instance not found.");
        }
    }

    void CalculateCorrectResponses()
    {
        correctResponses = 0;
        foreach (var entry in userResponses)
        {
            if (emailsToDisplay[entry.Key].isPhishing == entry.Value)
            {
                correctResponses++;
            }
        }
        totalResponses = userResponses.Count;
    }
    public void EnterReviewMode()
    {
        // Clear any previously displayed emails and set the list to only include incorrectly identified ones.
        emailsToDisplay.Clear();
        emailsToDisplay.AddRange(incorrectlyIdentifiedEmails);
        currentEmailIndex = 0; // Reset the index to start from the first email in review mode.

        if (emailsToDisplay.Count > 0)
        {
            
            emailPanel.SetActive(true);
            reviewModeIndicator.gameObject.SetActive(true); // Indicate that it's review mode.
            DisplayEmailAtIndex(currentEmailIndex); // Display the first incorrectly identified email.
        }
        else
        {
            Debug.Log("No incorrect emails to review.");
            emailPanel.SetActive(false); // Optionally, handle the case where there are no incorrect emails.
            reviewModeIndicator.gameObject.SetActive(false);
        }

        // Hide and disable the phishing and not phishing buttons as actions are not required in review mode.
        phishingButton.gameObject.SetActive(false);
        notPhishingButton.gameObject.SetActive(false);
        phishingButton.interactable = false;
        notPhishingButton.interactable = false;

        // Hide the review mistakes button as we are already in review mode.
        reviewMistakesButton.gameObject.SetActive(false);
        resetButton.gameObject.SetActive(true);

        forwardButton.gameObject.SetActive(emailsToDisplay.Count > 1);
        forwardButton.interactable = (emailsToDisplay.Count > 1);

        // Ensure navigation buttons are updated to reflect the reset index.
        UpdateNavigationButtons();
    }

    public bool IsEmailsDisplayActive()
    {
        // This method should return true if there are more emails to display or if the email panel is currently active
        return emailsToDisplay.Count > 0 || emailPanel.activeSelf;
    }


    void GoToNextEmail()
    {
        if (currentEmailIndex < emailsToDisplay.Count - 1)
        {
            currentEmailIndex++;
            DisplayEmailAtIndex(currentEmailIndex);
        }

        // Update button interactability based on the current index
        backButton.interactable = currentEmailIndex > 0;
        forwardButton.interactable = currentEmailIndex < emailsToDisplay.Count - 1;
    }
    void GoToPreviousEmail()
    {
        if (currentEmailIndex > 0)
        {
            // Reset the response for the current email before moving back
            if (userResponses.ContainsKey(currentEmailIndex))
            {
                // Optionally, remove the response or set it to null if you want to allow re-answering
                userResponses.Remove(currentEmailIndex);
                // Or userResponses[currentEmailIndex] = null; if using nullable bools and want to keep the key

                // If the email was incorrectly identified, remove it from the list as well
                Email currentEmail = emailsToDisplay[currentEmailIndex];
                if (incorrectlyIdentifiedEmails.Contains(currentEmail))
                {
                    incorrectlyIdentifiedEmails.Remove(currentEmail);
                }
            }

            // Move to the previous email
            currentEmailIndex--;
            DisplayEmailAtIndex(currentEmailIndex);

            // Update the buttons state as needed
            UpdateNavigationButtons();
        }
    }


    public void DisplayEmailAtIndex(int index)
    {
        // Display the email at the given index
        if (index >= 0 && index < emailsToDisplay.Count)
        {
            Email currentEmail = emailsToDisplay[index];
            subjectText.text = currentEmail.subject;
            senderText.text = currentEmail.sender;
            bodyText.text = currentEmail.body;
        }
    }
    void UpdateNavigationButtons()
    {
        backButton.interactable = currentEmailIndex > 0;
        forwardButton.interactable = currentEmailIndex < emailsToDisplay.Count - 1;
    }
    void ResetChallenge()
    {
        // Clear user responses and incorrect emails list
        userResponses.Clear();
        incorrectlyIdentifiedEmails.Clear();

        // Reset counters
        correctResponses = 0;
        totalResponses = 0;

        // IMPORTANT: Reset the email display list to its initial condition
        // First, clear the current list to ensure it's empty before repopulating
        emailsToDisplay.Clear(); // Make sure this list is cleared here

        // Repopulate the list with the original set of emails
        InitializeEmailsToDisplay(); // Assuming this method correctly adds emails from a source without duplicating them

        // Reset the current index and display the first email
        currentEmailIndex = 0;
        DisplayEmailAtIndex(currentEmailIndex);

        // Hide review mode UI elements
        reviewModeIndicator.gameObject.SetActive(false);
        reviewMistakesButton.gameObject.SetActive(false);
        resetButton.gameObject.SetActive(false); // Hide the reset button itself

        // Show or hide the email panel based on the context
        emailPanel.SetActive(true);

        // Ensure phishing and not phishing buttons are visible and interactive
        phishingButton.gameObject.SetActive(true);
        notPhishingButton.gameObject.SetActive(true);
        phishingButton.interactable = true;
        notPhishingButton.interactable = true;
        forwardButton.gameObject.SetActive(false); 
        backButton.gameObject.SetActive(true);
       

        // Update navigation buttons as needed
        UpdateNavigationButtons();
    }

    //void CloseChallenge()
    //{
    //    // Hide the email challenge panel without resetting the progress
    //    emailPanel.SetActive(false);
    //}
    //public void ResumeChallenge()
    //{
    //    emailManager.LoadChallengeState(); // Make sure to load the saved state first

    //    // Check if there's a specific email index we need to resume from
    //    if (emailManager.HasSavedState())
    //    {
    //        // Load the saved state to resume the challenge properly
    //        LoadSavedState(); // This method should update `currentEmailIndex`, `userResponses`, etc., from the saved state
    //    }

    //    // Determine what to display based on the current state
    //    if (incorrectlyIdentifiedEmails.Count > 0)
    //    {
    //        // There are incorrect emails to review
    //        EnterReviewMode();
    //    }
    //    else if (currentEmailIndex < emailsToDisplay.Count)
    //    {
    //        // Continue the challenge from the current email
    //        DisplayEmailAtIndex(currentEmailIndex);
    //    }
    //    else
    //    {
    //        // No more emails to display, possibly show a completion message or options to restart
    //        Debug.Log("All emails addressed. Consider showing completion message or restart option.");
    //    }

    //    // Update UI elements based on the resumed state
    //    emailPanel.SetActive(true);
    //    UpdateNavigationButtons();
    //    reviewMistakesButton.gameObject.SetActive(incorrectlyIdentifiedEmails.Count > 0);
    //    resetButton.gameObject.SetActive(true); // Optionally, allow resetting even after resuming
    //}

    //// This is a placeholder for a method you'll need to implement based on how you've structured saving/loading state
    //private void LoadSavedState()
    //{
    //    // Load the state from `EmailManager`, updating `currentEmailIndex`, `userResponses`, etc.
    //    // This method's implementation will depend on how you're saving/loading state in `EmailManager`
    //}


    //public void StartChallenge()
    //{
    //    // Reset or initialize all necessary variables and lists
    //    userResponses.Clear();
    //    incorrectlyIdentifiedEmails.Clear();
    //    correctResponses = 0;
    //    totalResponses = 0;
    //    currentEmailIndex = 0;

    //    // Re-initialize emails to display and shuffle them if necessary
    //    InitializeEmailsToDisplay();
    //    ShuffleEmails(); // If you want the emails to be presented in a new random order each time

    //    // Display the first email or set up the UI for the challenge start
    //    DisplayRandomEmail();

    //    // Ensure all UI elements are correctly set for the challenge start
    //    emailPanel.SetActive(true);
    //    reviewModeIndicator.gameObject.SetActive(false);
    //    reviewMistakesButton.gameObject.SetActive(false);
    //    resetButton.gameObject.SetActive(false);
    //    closeButton.gameObject.SetActive(true); // Assuming you want the close button always available

    //    phishingButton.gameObject.SetActive(true);
    //    notPhishingButton.gameObject.SetActive(true);
    //    phishingButton.interactable = true;
    //    notPhishingButton.interactable = true;

    //    // Update navigation buttons as needed
    //    UpdateNavigationButtons();
    //}



}
