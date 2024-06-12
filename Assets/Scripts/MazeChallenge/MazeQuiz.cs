using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // Make sure to have TextMeshPro if you use TextMeshProUGUI

[System.Serializable]
public class QuizQuestion
{
    public string questionText;
    public List<string> options; // Ensure this is included
    public string correctAnswer; // Optional if you use correctAnswerIndex
    public int correctAnswerIndex;// This identifies the correct answer's index in the options list
  
    public QuizQuestion(string questionText, List<string> options, int correctAnswerIndex)
    {
        this.questionText = questionText;
        this.options = options;
        this.correctAnswerIndex = correctAnswerIndex;
        if (options.Count > correctAnswerIndex) // Safety check
        {
            this.correctAnswer = options[correctAnswerIndex]; 
        }
    }
}

public class MazeQuiz : MonoBehaviour
{
    public static MazeQuiz Instance;
    public static bool IsQuizActive { get; private set; }


    public GameObject quizUI; // The whole quiz panel
    public TextMeshProUGUI questionText; // Where the question is displayed
    public Button optionAButton, optionBButton; // The two answer buttons
    private CanvasGroup quizUIGroup;

    public List<QuizQuestion> questions = new List<QuizQuestion>(); // List of all questions
    private int currentQuestionIndex = 0; // Tracking which question is currently being asked
    private bool quizCompleted = false;
    private int attemptsLeft = 4;
    public GameObject gameOverPanel; 
    public TextMeshProUGUI scoreText; 
    private int correctAnswersCount = 0;
    public AchievementID achievementToUnlock;
    private bool achievementUnlocked = false;
    private bool isRestarting = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // This ensures the whole MazeQuiz GameObject isn't destroyed
        }
        else if (Instance != this)
        {
            if (Instance.isRestarting)
            {
                
                Instance.ResetQuiz();
            }

            Destroy(gameObject);
            return;
        }

        // Initialize CanvasGroup for fading
        quizUIGroup = quizUI.GetComponent<CanvasGroup>();
        if (quizUIGroup == null)
        {
            quizUIGroup = quizUI.AddComponent<CanvasGroup>();
        }
        InitializeQuestions();

       
        if (quizUI != null)
        {
            DontDestroyOnLoad(quizUI);
        }
        else
        {
            Debug.LogError("Quiz UI GameObject is not assigned in the inspector");
        }

        SetQuizActive(false);
    }




    void InitializeQuestions()
    {
        // Example questions
        questions.Add(new QuizQuestion(
    "A social engineering technique whereby attackers under disguise of a legitimate request attempt to gain access to confidential information is commonly referred to as:",
    new List<string> { "Phishing", "Privilege Escalation" },
    0)); // Correct answer is "Phishing"
        questions.Add(new QuizQuestion(
     "Which of the following answers refer to smishing?",
        new List<string> { "Spam over Internet Messaging(SPIM)", "Text messaging" },
     1));
        questions.Add(new QuizQuestion(
     "What type of spam relies on text-based communication?",
        new List<string> { "Spam over Internet Messaging(SPIM)", "SPIT" },
     0));
        questions.Add(new QuizQuestion(
     "Which of the following terms is commonly used to describe an unsolicited advertising message?\r\n",
        new List<string> { "Spam", "Adware" },
     0));
        questions.Add(new QuizQuestion(
    "Phishing scams targeting a specific group of people are referred to as:",
       new List<string> { "Whaling", "Spear phishing" },
   1));
        questions.Add(new QuizQuestion(
     "The practice of using a telephone system to manipulate user into disclosing confidential information is known as:",
        new List<string> { "Spear phishing", "Vishing" },
     1));
        questions.Add(new QuizQuestion(
    " A situation in which an unauthorized person can view another user's display or keyboard to learn their password or other confidential information is referred to as:",
       new List<string> { " Shoulder surfing", "Tailgating" },
    0));
        questions.Add(new QuizQuestion(
    "Which of the following answers refer to the characteristic features of pharming?",
       new List<string> { "Password attack", "Fraudulent website" },
    1));
    }

    public void ShowNextQuestion()
    {
        if (quizCompleted)
        {
            Debug.Log("Quiz completed. No more questions.");
            return; // Exit if the quiz has been fully completed
        }

        if (currentQuestionIndex >= questions.Count)
        {
            quizCompleted = true;
            Debug.Log($"Quiz completed with {correctAnswersCount} correct answers out of {questions.Count}");

            // Check if the quiz has just been completed and the achievement hasn't been unlocked yet
            if (!achievementUnlocked)
            {
                UnlockAchievement();
            }
            return;
        }

        // Otherwise, display the next question
        QuizQuestion question = questions[currentQuestionIndex];
        DisplayQuestion(question);
    }

    //void DisplayQuestion(QuizQuestion question)
    //{
    //    questionText.text = question.questionText;

    //    optionAButton.GetComponentInChildren<TextMeshProUGUI>().text = question.options[0];

    //    optionBButton.GetComponentInChildren<TextMeshProUGUI>().text = question.options[1];


    //    optionAButton.onClick.RemoveAllListeners();
    //    optionAButton.onClick.AddListener(() => CheckAnswer(0));

    //    optionBButton.onClick.RemoveAllListeners();
    //    optionBButton.onClick.AddListener(() => CheckAnswer(1));

    //    ShowQuiz();
    //}
    void DisplayQuestion(QuizQuestion question)
    {
        if (questionText != null)
        {
            questionText.text = question.questionText;
        }

        // Check if optionAButton is not null before accessing its components
        if (optionAButton != null)
        {
            TextMeshProUGUI optionAText = optionAButton.GetComponentInChildren<TextMeshProUGUI>();
            if (optionAText != null)
            {
                optionAText.text = question.options[0];
            }

            optionAButton.onClick.RemoveAllListeners();
            optionAButton.onClick.AddListener(() => CheckAnswer(0));
        }

        // Check if optionBButton is not null before accessing its components
        if (optionBButton != null)
        {
            TextMeshProUGUI optionBText = optionBButton.GetComponentInChildren<TextMeshProUGUI>();
            if (optionBText != null)
            {
                optionBText.text = question.options[1];
            }

            optionBButton.onClick.RemoveAllListeners();
            optionBButton.onClick.AddListener(() => CheckAnswer(1));
        }

        // Assuming ShowQuiz() shows the UI for the quiz
        ShowQuiz();
    }


    //public void CheckAnswer(int selectedAnswerIndex)
    //{
    //    if (quizCompleted) return; // Exit if the quiz is completed.

    //    QuizQuestion question = questions[currentQuestionIndex];
    //    if (selectedAnswerIndex == question.correctAnswerIndex)
    //    {
    //        Debug.Log("Correct!");
    //        StartCoroutine(FadeOutQuizUI()); // Start fading out the UI.

    //        // Assuming each OccupiedZone triggers a specific question and MazeQuiz knows which one is active.
    //        OccupiedZone currentOccupiedZone = OccupiedZone.activeInstance; // Example: Using a static reference set by OccupiedZone when it triggers a quiz.
    //        if (currentOccupiedZone != null)
    //        {
    //            StartCoroutine(currentOccupiedZone.AnimateDoorOut()); // Ensure you start the coroutine properly.
    //        }

    //        // Increment the question index and check for completion.
    //        currentQuestionIndex++;
    //        if (currentQuestionIndex >= questions.Count)
    //        {
    //            quizCompleted = true;
    //            Debug.Log("Quiz completed.");
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("Incorrect. Try again.");
    //        // Remain the incorrect logic unchanged.
    //    }
    //}


    //public void CheckAnswer(int selectedAnswerIndex)
    //{
    //    if (quizCompleted) return; // Exit if the quiz is completed.

    //    QuizQuestion question = questions[currentQuestionIndex];
    //    if (selectedAnswerIndex == question.correctAnswerIndex)
    //    {
    //        Debug.Log("Correct!");
    //        StartCoroutine(FadeOutQuizUI()); // Start fading out the UI.

    //        // Assuming each OccupiedZone triggers a specific question and MazeQuiz knows which one is active.
    //        OccupiedZone currentOccupiedZone = OccupiedZone.activeInstance; // This finds an active OccupiedZone instance. Adjust as necessary for your project setup.
    //        if (currentOccupiedZone != null)
    //        {
    //            StartCoroutine(currentOccupiedZone.AnimateDoorOut()); // Trigger the door animation
    //        }

    //        currentQuestionIndex++; // Move to the next question or reset if at the end
    //        if (currentQuestionIndex >= questions.Count)
    //        {
    //            quizCompleted = true;
    //            Debug.Log("Quiz completed.");

    //        }
    //        else
    //        {

    //            ShowNextQuestion(); // Proceed to next question
    //        }
    //    }
    //    else
    //    {
    //        attemptsLeft--;
    //        Debug.Log($"Incorrect. {attemptsLeft} attempts left.");
    //        if (attemptsLeft <= 0)
    //        {
    //            Debug.Log("Out of attempts. Restarting...");
    //            //StartCoroutine(RestartGameWithDelay()); // Restart the game with a delay
    //        }
    //    }
    //}
    //    public void CheckAnswer(int selectedAnswerIndex)
    //    {
    //        if (quizCompleted) return; // Exit if the quiz is completed.

    //        QuizQuestion question = questions[currentQuestionIndex];
    //        if (selectedAnswerIndex == question.correctAnswerIndex)
    //        {
    //            Debug.Log("Correct!");
    //            StartCoroutine(FadeOutQuizUI()); // Start fading out the UI.

    //            // Trigger the door animation for the correct answer.
    //            OccupiedZone currentOccupiedZone = OccupiedZone.activeInstance; // Example: Using a static reference set by OccupiedZone when it triggers a quiz.
    //            if (currentOccupiedZone != null)
    //            {
    //                StartCoroutine(currentOccupiedZone.AnimateDoorOut()); // Ensure you start the coroutine properly.
    //            }

    //            // Increment the question index
    //            currentQuestionIndex++;

    //            // Check if the quiz is completed
    //             if (currentQuestionIndex >= questions.Count)
    //        {
    //            quizCompleted = true;
    //            Debug.Log($"Quiz completed with {correctAnswersCount} correct answers.");

    //                if (!achievementUnlocked)
    //                {
    //                    UnlockAchievement();
    //                }

    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("Incorrect answer.");

    //    }
    //}
    //public void CheckAnswer(int selectedAnswerIndex)
    //{
    //    if (quizCompleted) return; // Exit if the quiz is completed.

    //    QuizQuestion question = questions[currentQuestionIndex];
    //    if (selectedAnswerIndex == question.correctAnswerIndex)
    //    {
    //        Debug.Log("Correct!");

    //        // Trigger the door animation for the correct answer.
    //        OccupiedZone currentOccupiedZone = OccupiedZone.activeInstance;
    //        if (currentOccupiedZone != null)
    //        {
    //            StartCoroutine(currentOccupiedZone.AnimateDoorOut());
    //        }

    //        // Increment the question index
    //        currentQuestionIndex++;

    //        // Increment the count of correct answers
    //        correctAnswersCount++;

    //        // Directly close the quiz UI
    //        quizUI.SetActive(false);

    //        // Check if the quiz is completed
    //        if (currentQuestionIndex >= questions.Count)
    //        {
    //            quizCompleted = true;
    //            Debug.Log($"Quiz completed with {correctAnswersCount} correct answers out of {questions.Count}.");

    //            if (!achievementUnlocked && correctAnswersCount == questions.Count)
    //            {
    //                UnlockAchievement();
    //            }
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("Incorrect answer.");
    //    }
    //}
    public void CheckAnswer(int selectedAnswerIndex)
    {
        if (quizCompleted) return; // Exit if the quiz is completed.

        QuizQuestion question = questions[currentQuestionIndex];
        if (selectedAnswerIndex == question.correctAnswerIndex)
        {
            Debug.Log("Correct!");
            correctAnswersCount++; // Increment the count of correct answers

            // Optionally trigger animations or other feedback
            OccupiedZone currentOccupiedZone = OccupiedZone.activeInstance;
            if (currentOccupiedZone != null)
            {
                StartCoroutine(currentOccupiedZone.AnimateDoorOut());
            }

            quizUI.SetActive(false); // Close the quiz UI immediately after the correct answer

            currentQuestionIndex++; // Move to the next question index

            // Check if the quiz is completed
            if (currentQuestionIndex >= questions.Count)
            {
                quizCompleted = true;
                Debug.Log($"Quiz completed with {correctAnswersCount} correct answers out of {questions.Count}.");
                if (!achievementUnlocked && correctAnswersCount == questions.Count)
                {
                    UnlockAchievement();
                }
            }
        }
        else
        {
            Debug.Log("Incorrect answer. Please try again.");
            // Optionally, you can refresh the UI here if you want to give feedback or hints
        }
    }


    public void RestartQuiz()
    {
        // Reset the state as necessary
        ResetQuiz();

        // Now reload the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ResetQuiz()
    {
        // Reset the state of the quiz
        currentQuestionIndex = 0;
        quizCompleted = false;
        correctAnswersCount = 0;
        achievementUnlocked = false;
        // Reset other state variables as needed

        // You may want to reset the timer as well if it's a component of this GameObject
        Timer timer = GetComponent<Timer>();
        if (timer != null)
        {
            // Reset the remaining time on the timer if needed
            //timer.ResetTimer();
        }

        // Additional reset logic if necessary
    }
    private void UnlockAchievement()
    {
       
        if (AchievementManager.Instance != null && !achievementUnlocked)
        {
            AchievementManager.Instance.UnlockAchievement(achievementToUnlock);
            Debug.Log("Achievement unlocked!");
            achievementUnlocked = true;
        }
        else
        {
            Debug.LogError("AchievementManager instance not found.");
        }
    }

    public static bool QuizCompleted
    {
        get { return Instance != null ? Instance.quizCompleted : false; }
    }

  
    public void ShowQuiz()
    {
     
            quizUI.SetActive(true);
            quizUIGroup.alpha = 1f;
            SetQuizActive(true);

       
    }
    private void SetQuizActive(bool isActive)
    {
        quizUIGroup.alpha = isActive ? 1f : 0f; // Fully visible if active, fully transparent if not
        quizUIGroup.blocksRaycasts = isActive; // Block raycasts when active, allowing interaction
        quizUIGroup.interactable = isActive; // Ensure the UI elements within the CanvasGroup are interactable
        quizUI.SetActive(isActive); // Toggle the active state of the quiz UI GameObject
    }
}
