
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;

public class PasswordManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public GameObject[] ruleFeedbackObjects;
    public float fadeDuration = 2f;
    private bool[] rulesAnimated;

    public AchievementID achievementToUnlock;
    private bool achievementUnlocked = false;
    public Button submitButton;
    public GameObject panel; // Assuming the panel is a GameObject that can be enabled/disabled

    private HashSet<string> commonPasswords = new HashSet<string>();

    private List<System.Func<string, bool>> passwordRules = new List<System.Func<string, bool>>();

    void Start()
    {
        StartCoroutine(LoadCommonPasswords());
        InitializePasswordRules();
        inputField.onValueChanged.AddListener(delegate { CheckPasswordAndUpdateFeedback(); });
        rulesAnimated = new bool[passwordRules.Count];
        submitButton.onClick.AddListener(OnSubmitButtonPressed);
    }


    void InitializePasswordRules()
    {
        passwordRules.Add(password => !commonPasswords.Contains(password));
        passwordRules.Add(password => password.Length >= 10);
        passwordRules.Add(password => Regex.IsMatch(password, "[A-Z]"));
        passwordRules.Add(password => Regex.IsMatch(password, "[a-z]"));
        passwordRules.Add(password => Regex.IsMatch(password, "[0-9]"));
        passwordRules.Add(password => Regex.IsMatch(password, "[!@#$%^&*()_+=-]"));
        passwordRules.Add(password => !Regex.IsMatch(password, "(.)\\1\\1"));
        passwordRules.Add(password => !Regex.IsMatch(password, "012|123|234|345|456|567|678|789|890|abc|bcd|cde|def|efg|fgh|ghi|hij|ijk|jkl|klm|lmn|mn|nop|opq|pqr|qrs|rst|stu|tuv|uvw|vwx|wxy|xyz", RegexOptions.IgnoreCase)); // Avoids common sequences.

    }

    IEnumerator LoadCommonPasswords()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "CommonPasswords.txt");
        Debug.Log("Trying to load common passwords from: " + path);

        using (UnityWebRequest www = UnityWebRequest.Get(path))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError("Failed to load common passwords. Error: " + www.error);
            }
            else
            {
                // Successfully loaded the data
                string dataAsText = www.downloadHandler.text;
                if (string.IsNullOrEmpty(dataAsText))
                {
                    Debug.LogError("Loaded common passwords file is empty.");
                }
                else
                {
                    Debug.Log("Received data: " + dataAsText.Substring(0, Mathf.Min(200, dataAsText.Length))); // Log first 200 characters
                    ProcessPasswords(dataAsText);
                }
            }
        }
    }

    void ProcessPasswords(string dataAsText)
    {
        string[] lines = dataAsText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        int count = 0;
        foreach (var line in lines)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                commonPasswords.Add(line.Trim());
                count++;
            }
        }
        Debug.Log($"Loaded {count} common passwords successfully.");
    }

    void CheckPasswordAndUpdateFeedback()
    {
        string password = inputField.text;
        bool shouldEnableSubmit = true; // Assume the button should be enabled, but look for reasons to set this to false.

        // Check if password is a common password (rule 1).
        bool isCommonPasswordUsed = commonPasswords.Contains(password);
        if (isCommonPasswordUsed)
        {
            HandleFeedbackForCommonAndSequenceRules(0, true);
            shouldEnableSubmit = false; // Disable button if it's a common password.
        }
        else
        {
            HandleFeedbackForCommonAndSequenceRules(0, false);
        }

        // Check for repetitive characters (rule 7)
        bool isRepetitiveCharacters = Regex.IsMatch(password, "(.)\\1\\1");
        // Check for sequential characters (rule 8)
        bool isSequentialCharacters = Regex.IsMatch(password, "012|123|234|345|456|567|678|789|890|abc|bcd|cde|def|efg|fgh|ghi|hij|ijk|jkl|klm|lmn|mn|nop|opq|pqr|qrs|rst|stu|tuv|uvw|vwx|wxy|xyz", RegexOptions.IgnoreCase);

        // Provide feedback and disable button if rule 7 or 8 is violated
        if (isRepetitiveCharacters || isSequentialCharacters)
        {
            if (isRepetitiveCharacters)
                HandleFeedbackForCommonAndSequenceRules(passwordRules.Count - 2, true); // Assuming second last rule for repetitive characters
            if (isSequentialCharacters)
                HandleFeedbackForCommonAndSequenceRules(passwordRules.Count - 1, true); // Assuming last rule for sequential characters

            shouldEnableSubmit = false;
        }
        else // Reset feedback if the rules are not violated
        {
            HandleFeedbackForCommonAndSequenceRules(passwordRules.Count - 2, false); // Repetitive characters feedback reset
            HandleFeedbackForCommonAndSequenceRules(passwordRules.Count - 1, false); // Sequential characters feedback reset
        }

        // Check rules 2 through 6.
        for (int i = 1; i <= 5; i++) // Correctly check rules 2 through 6.
        {
            var rulePassed = passwordRules[i](password);
            Debug.Log($"Rule {i + 1} passed: {rulePassed}");

            var feedbackObject = ruleFeedbackObjects[i]; // Adjust index if needed to align with your UI elements
            if (rulePassed)
            {
                AnimateFeedbackObjectIfNeeded(feedbackObject, i);
            }
            else
            {
                ResetOrFadeFeedback(feedbackObject, i);
                shouldEnableSubmit = false; // Correctly set to false if any rule is not passed.
            }
        }

        Debug.Log($"Submit Button should be enabled: {shouldEnableSubmit}");

        // Finally, set the submit button's interactable state based on the overall evaluation.
        submitButton.interactable = shouldEnableSubmit;
    }






    void HandleFeedbackForCommonAndSequenceRules(int ruleIndex, bool ruleViolated)
    {
        var feedbackObject = ruleFeedbackObjects[ruleIndex];
        if (ruleViolated)
        {
            feedbackObject.SetActive(true);
            CanvasGroup canvasGroup = feedbackObject.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 1f; // Ensure it's fully visible

            // Assuming the first rule feedback object contains a TextMeshProUGUI component to display text
            if (ruleIndex == 0) // Common password rule
            {
                TextMeshProUGUI feedbackText = feedbackObject.GetComponentInChildren<TextMeshProUGUI>();
                feedbackText.text = "You are using a common password. Please choose a stronger password.";
            }
        }
        else
        {
            ResetOrFadeFeedback(feedbackObject, ruleIndex);
        }
    }


    void AnimateFeedbackObjectIfNeeded(GameObject feedbackObject, int ruleIndex)
    {
        var canvasGroup = feedbackObject.GetComponent<CanvasGroup>();
        if (!rulesAnimated[ruleIndex])
        {
            AnimateFeedbackObject(feedbackObject, ruleIndex);
        }
        else
        {
            // If already animated, ensure it's visible
            canvasGroup.alpha = 1f;
            feedbackObject.SetActive(true);
        }
    }

    void ResetOrFadeFeedback(GameObject feedbackObject, int ruleIndex)
    {
        var canvasGroup = feedbackObject.GetComponent<CanvasGroup>();
        if (feedbackObject.activeSelf)
        {
            FadeOutFeedbackObject(canvasGroup);
        }
        else
        {
            // If not active, ensure it's ready for next time
            rulesAnimated[ruleIndex] = false;
            feedbackObject.transform.localScale = Vector3.one; // Reset scale
        }
    }
    void FadeOutFeedbackObject(CanvasGroup canvasGroup)
    {
        // Fade to 0 alpha over 0.5 seconds
        LeanTween.alphaCanvas(canvasGroup, 0f, 0.5f).setOnComplete(() =>
        {
            canvasGroup.gameObject.SetActive(false);
            canvasGroup.alpha = 1f; // Reset alpha for next time it's shown
        });
    }

    void AnimateFeedbackObject(GameObject feedbackObject, int ruleIndex)
    {
        if (!rulesAnimated[ruleIndex])
        {
            // Mark this rule as animated
            rulesAnimated[ruleIndex] = true;

            // Start from a very small scale
            feedbackObject.transform.localScale = Vector3.one * 0.1f;
            feedbackObject.SetActive(true);

            // Scale up to full size
            LeanTween.scale(feedbackObject, Vector3.one, 0.5f)
                .setEaseOutElastic()
                .setOnComplete(() =>
                {
                    // Optionally, do something after the animation is complete
                });
        }
    }


    public void OnSubmitButtonPressed()
    {
        if (!achievementUnlocked)
        {
            UnlockAchievement();
            panel.SetActive(false); // Close the panel
        }
    }

    void UnlockAchievement()
    {
        if (AchievementManager.Instance != null && !achievementUnlocked) // Ensuring achievementUnlocked is checked here for safety
        {
            AchievementManager.Instance.UnlockAchievement(achievementToUnlock);
            Debug.Log("Achievement unlocked!");
            achievementUnlocked = true; // Ensure we don't unlock it multiple times
        }
        else if (!achievementUnlocked) // If the criteria is not met or AchievementManager instance not found, and we haven't unlocked it yet
        {
            Debug.LogError("Achievement criteria not met or AchievementManager instance not found.");
        }
    }
}

