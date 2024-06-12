
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;

public class QuizManager : MonoBehaviour
{
    public TextMeshProUGUI questionText; // TextMeshPro for question text
    public Toggle[] answerToggles; // Array of Toggles for answers
    public TextMeshProUGUI questionCountText; // TextMeshPro for question count
    public Button nextButton; // Button to go to the next question
    public Button previousButton; // Button to go back to the previous question
    public Button submitButton;
    public GameObject alertPanel; // Reference to the alert panel GameObject
    public Button alertYesButton; // 'Yes' button on the alert panel
    public Button alertNoButton; // 'No' button on the alert panel

    private List<Question> questions = new List<Question>(); // List to store questions
    private int currentQuestionIndex = 0;
    private List<int> userAnswers;
    private bool isUserInteraction = true;
    private Dictionary<string, int> categoryScores = new Dictionary<string, int>();
    private int categoriesToLoad = 4; // Total number of question categories
    private int categoriesLoaded = 0; // Counter for loaded categories

    void Start()
    {
        UserSession currentUserSession = LoadUserSession();
        if (currentUserSession != null)
        {
            Debug.Log("Quiz started by user: " + currentUserSession.username);
           
        }
        else
        {
            Debug.LogError("No user session found. Make sure to log in first.");
            // Handle the case where no user is logged in
        }

        StartCoroutine(LoadQuestionsFromJSON("socialEngineeringQuestions.json"));
        StartCoroutine(LoadQuestionsFromJSON("identifyingSuspiciousURLs.json"));
        StartCoroutine(LoadQuestionsFromJSON("identifyingPhishingEmails.json"));
        StartCoroutine(LoadQuestionsFromJSON("strongPasswordCreation.json"));

        userAnswers = new List<int>(new int[20]); 

        for (int i = 0; i < 12; i++)
        {
            userAnswers[i] = -1; 
        }

        LoadQuestion(currentQuestionIndex);

        nextButton.onClick.AddListener(GoToNextQuestion);
        previousButton.onClick.AddListener(GoToPreviousQuestion);

        UpdateNavigationButtons();

        for (int i = 0; i < answerToggles.Length; i++)
        {
            int index = i; // Local copy of the loop counter for closure
            answerToggles[i].onValueChanged.AddListener(delegate { OnToggleValueChanged(index); });
        }
        foreach (var question in questions)
        {
            if (!categoryScores.ContainsKey(question.category))
            {
                categoryScores[question.category] = 0;
            }
        }
    }

    private UserSession LoadUserSession()
    {
        string sessionPath = Path.Combine(Application.dataPath, "Data/userSession.json");
        if (File.Exists(sessionPath))
        {
            string json = File.ReadAllText(sessionPath);
            UserSession session = JsonUtility.FromJson<UserSession>(json);
            return session;
        }
        return null; // Or handle this case as needed
    }


    //void LoadQuestions()
    //{
    //    // Temporary lists for each category
    //    List<Question> socialEngineeringQuestions = LoadQuestionsFromJSON("socialEngineeringQuestions.json");
    //    List<Question> suspiciousURLsQuestions = LoadQuestionsFromJSON("identifyingSuspiciousURLs.json");
    //    List<Question> phishingEmailsQuestions = LoadQuestionsFromJSON("identifyingPhishingEmails.json");
    //    List<Question> strongPasswordQuestions = LoadQuestionsFromJSON("strongPasswordCreation.json");

    //    // Add 5 questions from each category
    //    questions.AddRange(GetRandomQuestions(socialEngineeringQuestions, 3));
    //    questions.AddRange(GetRandomQuestions(suspiciousURLsQuestions, 3));
    //    questions.AddRange(GetRandomQuestions(phishingEmailsQuestions, 3));
    //    questions.AddRange(GetRandomQuestions(strongPasswordQuestions, 3));
    //}



    List<Question> GetRandomQuestions(List<Question> questionList, int count)
    {
        // Shuffle the list and return a specified number of questions
        ShuffleQuestions(questionList);
        return questionList.GetRange(0, Mathf.Min(count, questionList.Count));
    }

    //List<Question> LoadQuestionsFromJSON(string fileName)
    //{
    //    string filePath = Path.Combine(Application.dataPath, "Data/" + fileName);
    //    if (File.Exists(filePath))
    //    {
    //        string dataAsJson = File.ReadAllText(filePath);
    //        QuestionList loadedData = JsonUtility.FromJson<QuestionList>("{\"questions\":" + dataAsJson + "}");
    //        return new List<Question>(loadedData.questions);
    //    }
    //    else
    //    {
    //        Debug.LogError("Cannot find JSON file: " + fileName);
    //        return new List<Question>(); // Return an empty list if the file isn't found
    //    }
    //}
    IEnumerator LoadQuestionsFromJSON(string fileName)
    {
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);
        UnityWebRequest www = UnityWebRequest.Get(filePath);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError("Error loading JSON: " + www.error);
        }
        else
        {
            // Successfully loaded the JSON file
            string jsonData = www.downloadHandler.text;
            QuestionList loadedData = JsonUtility.FromJson<QuestionList>("{\"questions\":" + jsonData + "}");

            // Process loaded questions immediately here or pass them to another method for processing
            ProcessLoadedQuestions(new List<Question>(loadedData.questions), fileName);
        }
    }

    void ProcessLoadedQuestions(List<Question> loadedQuestions, string category)
    {
        // Add only 3 questions from the loaded list
        questions.AddRange(GetRandomQuestions(loadedQuestions, 3));
        categoriesLoaded++; // Increment the counter each time a category is loaded

        // Check if all categories have been loaded
        if (categoriesLoaded == categoriesToLoad)
        {
            // All questions are loaded; proceed with initialization
            InitializeQuiz();
        }
    }

    void InitializeQuiz()
    {
       
        if (questions.Count > 0)
        {
            LoadQuestion(currentQuestionIndex);
        }
        else
        {
            Debug.LogError("No questions were loaded!");
        }
        UpdateNavigationButtons();

        // Prepare UI or other elements as needed
    }

    void ShuffleQuestions(List<Question> questionList)
    {
        for (int i = 0; i < questionList.Count; i++)
        {
            int randomIndex = Random.Range(i, questionList.Count);
            Question temp = questionList[i];
            questionList[i] = questionList[randomIndex];
            questionList[randomIndex] = temp;
        }
    }

    public void OnToggleValueChanged(int selectedToggleIndex)
    {
        if (isUserInteraction)
        {
            userAnswers[currentQuestionIndex] = selectedToggleIndex;

            // Check if the selected answer is correct
            bool isAnswerCorrect = selectedToggleIndex == questions[currentQuestionIndex].correctAnswerIndex;

            // Log only if the answer is correct or not
            Debug.Log("Question " + (currentQuestionIndex + 1) + ": " + (isAnswerCorrect ? "Correct answer." : "Incorrect answer."));
        }

        for (int i = 0; i < answerToggles.Length; i++)
        {
            if (i != selectedToggleIndex)
            {
                answerToggles[i].isOn = false;
            }
        }
    }

    void LoadQuestion(int index)
    {
        if (index < questions.Count)
        {
            Question question = questions[index];
            questionText.text = question.questionText;

            // Temporarily disable user interaction to prevent firing OnToggleValueChanged
            isUserInteraction = false;

            for (int i = 0; i < answerToggles.Length; i++)
            {
                var toggle = answerToggles[i];
                toggle.isOn = false; // Reset all toggles to off
                toggle.gameObject.SetActive(i < question.answers.Length); // Activate only the relevant toggles

                if (i < question.answers.Length)
                {
                    TextMeshProUGUI label = toggle.GetComponentInChildren<TextMeshProUGUI>();
                    label.text = question.answers[i]; // Set the text for the toggle
                }

                // If this answer was previously selected, turn the toggle on
                if (userAnswers[index] == i)
                {
                    toggle.isOn = true;
                }
            }

            // Re-enable user interaction after setting up the toggles
            isUserInteraction = true;

            questionCountText.text = $"Question {currentQuestionIndex + 1} of {questions.Count}";
        }
        else
        {
            Debug.LogError("Index out of range: " + index);
        }
    }





    void GoToNextQuestion()
    {
        if (currentQuestionIndex < questions.Count - 1)
        {
            currentQuestionIndex++;
            LoadQuestion(currentQuestionIndex);
            UpdateNavigationButtons();
        }
    }

    void GoToPreviousQuestion()
    {
        if (currentQuestionIndex > 0)
        {
            currentQuestionIndex--;
            LoadQuestion(currentQuestionIndex);
            UpdateNavigationButtons();
        }
    }

    void UpdateNavigationButtons()
    {
        previousButton.interactable = currentQuestionIndex > 0;
        nextButton.interactable = currentQuestionIndex < questions.Count - 1;
        submitButton.gameObject.SetActive(currentQuestionIndex == questions.Count - 1);
    }

    public void OnSubmitQuiz()
    {
        bool allQuestionsAnswered = true;
        foreach (var answer in userAnswers)
        {
            if (answer == -1) // -1 indicates a question was not answered
            {
                allQuestionsAnswered = false;
                break;
            }
        }

        if (!allQuestionsAnswered)
        {
            // Show alert panel if not all questions have been answered
            alertPanel.SetActive(true);
        }
        else
        {
            // Proceed with submission if all questions have been answered
            SubmitQuiz();
        }
    }


    //private void SubmitQuiz()
    //{
    //    int correctAnswers = 0;
    //    foreach (var userAnswer in userAnswers)
    //    {
    //        if (userAnswer != -1)
    //        {
    //            var question = questions[userAnswers.IndexOf(userAnswer)];
    //            if (question.correctAnswerIndex == userAnswer)
    //            {
    //                correctAnswers++;
    //                categoryScores[question.category]++;
    //            }
    //        }
    //    }

    //    float scorePercent = ((float)correctAnswers / questions.Count) * 100;
    //    Debug.Log($"Quiz completed! Score: {scorePercent}% ({correctAnswers} out of {questions.Count} correct)");

    //    // Save score and category scores, then load the feedback scene
    //    PlayerPrefs.SetFloat("LastScore", scorePercent);
    //    foreach (var entry in categoryScores)
    //    {
    //        PlayerPrefs.SetInt(entry.Key + "_Score", entry.Value);
    //    }
    //    PlayerPrefs.Save();
    //    SceneManager.LoadScene("RankAndFeedback");
    //}

    private void SubmitQuiz()
    {
        int correctAnswers = 0;
        Dictionary<string, int> localCategoryScores = new Dictionary<string, int>();

        // Debugging: Ensure we have the expected number of answers and each has a value.
        Debug.Log($"Total Questions: {questions.Count}, Total Answers: {userAnswers.Count}");

        for (int i = 0; i < questions.Count; i++)
        {
            var question = questions[i];
            int userAnswer = userAnswers[i];

            // Debugging: Log each question's correct answer and what the user selected.
            Debug.Log($"Question {i + 1}: Correct Answer Index: {question.correctAnswerIndex}, User Answer Index: {userAnswer}");

            if (userAnswer != -1 && userAnswer == question.correctAnswerIndex)
            {
                correctAnswers++;
                string category = question.category;
                if (!localCategoryScores.ContainsKey(category))
                {
                    localCategoryScores[category] = 0;
                }
                localCategoryScores[category]++;
            }
        }

        float scorePercent = ((float)correctAnswers / questions.Count) * 100;
        Debug.Log($"Quiz completed! Score: {scorePercent}% ({correctAnswers} out of {questions.Count} correct)");

        UserSession currentUserSession = LoadUserSession();
        string username = currentUserSession != null ? currentUserSession.username : "Guest";
        foreach (var categoryScore in localCategoryScores)
        {
            Debug.Log($"Category: {categoryScore.Key}, Score: {categoryScore.Value}");
        }

        
        QuizResultsManager.Instance.AddQuizResult(correctAnswers, questions.Count, localCategoryScores); 
        Destroy(gameObject);
        SceneManager.LoadScene("RankAndFeedback"); 
    }


    public void AlertPanelYes()
    {
        alertPanel.SetActive(false); // Hide the alert panel
        SubmitQuiz(); // Proceed to submit the quiz
    }


    public void AlertPanelNo()
    {
        alertPanel.SetActive(false); // Simply hide the alert panel
    }




}
