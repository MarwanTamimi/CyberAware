
//using UnityEngine;
//using TMPro;
//using UnityEngine.UI;
//using UnityEngine.SceneManagement;
//using System.Linq;
//using System.Collections.Generic;

//public class FeedbackManager : MonoBehaviour
//{
//    public TextMeshProUGUI rankText;
//    public Button socialEngineeringButton;
//    public Button strongPasswordCreationButton;
//    public Button identifyingSuspiciousURLButton;
//    public Button identifyingPhishingEmailButton;

//    void Start()
//    {
//        // Directly access the QuizResultsManager instance to get the last result for the current user
//        var lastResult = QuizResultsManager.Instance.GetLastResultForCurrentUser();
//        if (lastResult != null)
//        {
//            string rank = AssignRank(lastResult.scorePercent);
//            SetupTooltips(lastResult.categoryScores);
//            // Update the last result with the rank and potentially other feedback, then save
//            QuizResultsManager.Instance.UpdateQuizResultWithRank(lastResult.username, rank);
//        }
//        SetupSceneChangeButtons();
//    }

//    string AssignRank(float score)
//    {
//        // Determine and assign rank based on score
//        string rank;
//        if (score >= 80)
//        {
//            rankText.text = "Advanced";
//            rank = "Advanced";
//        }
//        else
//        {
//            rankText.text = "Basic";
//            rank = "Basic";
//        }
//        Debug.Log($"[RankAndFeedbackManager] Rank assigned: {rank}");
//        return rank;
//    }

//    void SetupTooltips(List<CategoryScore> categoryScoresList)
//    {
//        var categoryScores = categoryScoresList.ToDictionary(cs => cs.category, cs => cs.score);
//        // Setup tooltips for each category
//        SetupTooltipForButton(socialEngineeringButton, "Social Engineering", categoryScores);
//        SetupTooltipForButton(strongPasswordCreationButton, "Strong Password Creation", categoryScores);
//        SetupTooltipForButton(identifyingSuspiciousURLButton, "Identifying Suspicious URLs", categoryScores);
//        SetupTooltipForButton(identifyingPhishingEmailButton, "Identifying Phishing Emails", categoryScores);
//    }

//    void SetupTooltipForButton(Button button, string category, Dictionary<string, int> categoryScores)
//    {
//        // Example implementation. Adapt according to your actual tooltip setup
//        int score = categoryScores.ContainsKey(category) ? categoryScores[category] : 0;
//        string tooltipMessage = $"Your proficiency in {category} is {score}.";
//        // Assuming TooltipTrigger is a component you've defined that handles displaying tooltips
//        var tooltipTrigger = button.gameObject.AddComponent<TooltipTrigger>();
//        tooltipTrigger.header = category;
//        tooltipTrigger.content = tooltipMessage;
//    }

//    void SetupSceneChangeButtons()
//    {
//        // Setup listeners for your buttons
//        socialEngineeringButton.onClick.AddListener(() => LoadScene("FirstChallenge"));
//        strongPasswordCreationButton.onClick.AddListener(() => LoadScene("FirstChallenge"));
//        identifyingSuspiciousURLButton.onClick.AddListener(() => LoadScene("FirstChallenge"));
//        identifyingPhishingEmailButton.onClick.AddListener(() => LoadScene("FirstChallenge"));
//    }

//    void LoadScene(string sceneName)
//    {
//        SceneManager.LoadScene(sceneName);
//    }
//}
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections.Generic;

public class FeedbackManager : MonoBehaviour
{
    public TextMeshProUGUI rankText;
    public Button socialEngineeringButton;
    public Button strongPasswordCreationButton;
    public Button identifyingSuspiciousURLButton;
    public Button identifyingPhishingEmailButton;

    void Start()
    {
        // Directly access the QuizResultsManager instance to get the last result for the current user
        var lastResult = QuizResultsManager.Instance.GetLastResultForCurrentUser();
        if (lastResult != null)
        {
            UpdateFeedback(lastResult.score, lastResult.totalScore);
            SetupTooltips(lastResult.categoryScores);
            // Update the last result with feedback and potentially other data, then save
            QuizResultsManager.Instance.UpdateQuizResultWithFeedback(lastResult.username, rankText.text);
        }
        SetupSceneChangeButtons();
    }

    void UpdateFeedback(int score, int totalScore)
    {
        // Check if the score is 6 or less
        if (score <= 6)
        {
            rankText.text = $"Don't worry, you will get better. YOU GOT {score} OUT OF {totalScore}";
        }
        // Check if the score is more than 6
        else if (score > 6)
        {
            rankText.text = $"WOOW!! YOU GOT {score} OUT OF {totalScore}";
        }
        Debug.Log($"[FeedbackManager] Feedback updated: {rankText.text}");
    }


    void SetupTooltips(List<CategoryScore> categoryScoresList)
    {
        var categoryScores = categoryScoresList.ToDictionary(cs => cs.category, cs => cs.score);
        // Setup tooltips for each category
        SetupTooltipForButton(socialEngineeringButton, "Social Engineering", categoryScores);
        SetupTooltipForButton(strongPasswordCreationButton, "Strong Password Creation", categoryScores);
        SetupTooltipForButton(identifyingSuspiciousURLButton, "Identifying Suspicious URLs", categoryScores);
        SetupTooltipForButton(identifyingPhishingEmailButton, "Identifying Phishing Emails", categoryScores);
    }

    void SetupTooltipForButton(Button button, string category, Dictionary<string, int> categoryScores)
    {
        int score = categoryScores.ContainsKey(category) ? categoryScores[category] : 0;
        string tooltipMessage = $"Your proficiency in {category} is {score}.";
        var tooltipTrigger = button.gameObject.AddComponent<TooltipTrigger>();
        tooltipTrigger.header = category;
        tooltipTrigger.content = tooltipMessage;
    }

    void SetupSceneChangeButtons()
    {
        socialEngineeringButton.onClick.AddListener(() => LoadScene("FirstChallenge"));
        strongPasswordCreationButton.onClick.AddListener(() => LoadScene("FirstChallenge"));
        identifyingSuspiciousURLButton.onClick.AddListener(() => LoadScene("FirstChallenge"));
        identifyingPhishingEmailButton.onClick.AddListener(() => LoadScene("FirstChallenge"));
    }

    void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
