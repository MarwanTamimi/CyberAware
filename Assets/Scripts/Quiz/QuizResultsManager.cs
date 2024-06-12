////using System.Collections.Generic;
////using System.IO;
////using System.Linq;
////using UnityEngine;

////[System.Serializable]
////public class CategoryScore
////{
////    public string category;
////    public int score;
////}

////[System.Serializable]
////public class QuizResult
////{
////    public string username;
////    public float scorePercent;
////    public List<CategoryScore> categoryScores = new List<CategoryScore>(); // Use this instead of Dictionary
////    public string rank;
////}


////[System.Serializable]
////public class QuizResultDatabase
////{
////    public List<QuizResult> results = new List<QuizResult>();
////}

////public class QuizResultsManager : MonoBehaviour
////{
////    public static QuizResultsManager Instance;

////    private QuizResultDatabase quizResultsDatabase;
////    private string resultsDatabasePath;

////    private void Awake()
////    {
////        if (Instance == null)
////        {
////            Instance = this;
////            DontDestroyOnLoad(gameObject);

////            resultsDatabasePath = Path.Combine(Application.dataPath, "Data/quizResults.json");
////            LoadQuizResultsDatabase();
////        }
////        else
////        {
////            Destroy(gameObject);
////        }
////    }

////    private void LoadQuizResultsDatabase()
////    {
////        if (File.Exists(resultsDatabasePath))
////        {
////            string json = File.ReadAllText(resultsDatabasePath);
////            quizResultsDatabase = JsonUtility.FromJson<QuizResultDatabase>(json);
////        }
////        else
////        {
////            quizResultsDatabase = new QuizResultDatabase();
////        }
////    }

////    public void SaveQuizResultsDatabase()
////    {
////        string json = JsonUtility.ToJson(quizResultsDatabase, true);
////        File.WriteAllText(resultsDatabasePath, json);
////    }

////    public void AddQuizResult(float scorePercent, Dictionary<string, int> categoryScores)
////    {
////        // Assuming GetCurrentUsername() retrieves the current user's username from the saved session.
////        string username = GetCurrentUsername();

////        QuizResult newResult = new QuizResult
////        {
////            username = username, // Set the username
////            scorePercent = scorePercent,
////            categoryScores = ConvertDictionaryToList(categoryScores)
////        };

////        quizResultsDatabase.results.Add(newResult);
////        SaveQuizResultsDatabase();
////    }
////    private string GetCurrentUsername()
////    {
////        // This path must match where you're saving the session info
////        string sessionPath = Path.Combine(Application.dataPath, "Data/userSession.json");
////        if (File.Exists(sessionPath))
////        {
////            string json = File.ReadAllText(sessionPath);
////            UserSession session = JsonUtility.FromJson<UserSession>(json);
////            return session.username;
////        }
////        return ""; // Return an empty string or handle this case as needed
////    }


////    public void UpdateQuizResultWithRank(string username, string rank)
////    {
////        // Find the last result for the user
////        QuizResult lastResult = quizResultsDatabase.results.LastOrDefault(result => result.username == username);
////        if (lastResult != null)
////        {
////            // Update the rank
////            lastResult.rank = rank;
////            SaveQuizResultsDatabase(); // Save the updated database to JSON
////        }
////    }
////    List<CategoryScore> ConvertDictionaryToList(Dictionary<string, int> dict)
////    {
////        List<CategoryScore> list = new List<CategoryScore>();
////        foreach (var item in dict)
////        {
////            list.Add(new CategoryScore { category = item.Key, score = item.Value });
////        }
////        return list;
////    }
////}
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using UnityEngine;
//using UnityEngine.Networking;

//[System.Serializable]
//public class CategoryScore
//{
//    public string category;
//    public int score;
//}

//[System.Serializable]
//public class QuizResult
//{
//    public string username;
//    public float scorePercent;
//    public List<CategoryScore> categoryScores = new List<CategoryScore>();
//    public string rank;
//    public int score; // Current score obtained
//    public int totalScore; // Total possible score

//}

//[System.Serializable]
//public class QuizResultDatabase
//{
//    public List<QuizResult> results = new List<QuizResult>();
//}

//public class QuizResultsManager : MonoBehaviour
//{
//    public static QuizResultsManager Instance;

//    private QuizResultDatabase quizResultsDatabase;
//    private string resultsDatabasePath = "quizResults.json"; // Name of the file in StreamingAssets

//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject);

//            if (PlayerPrefs.HasKey("QuizResultsDatabase"))
//            {
//                string json = PlayerPrefs.GetString("QuizResultsDatabase");
//                quizResultsDatabase = JsonUtility.FromJson<QuizResultDatabase>(json);
//            }
//            else
//            {
//                // Load the initial database from StreamingAssets
//                StartCoroutine(LoadQuizResultsDatabaseFromStreamingAssets(resultsDatabasePath));
//            }
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    private IEnumerator LoadQuizResultsDatabaseFromStreamingAssets(string path)
//    {
//        string uri = Path.Combine(Application.streamingAssetsPath, path);
//        using (UnityWebRequest www = UnityWebRequest.Get(uri))
//        {
//            yield return www.SendWebRequest();
//            if (www.isNetworkError || www.isHttpError)
//            {
//                Debug.LogError("Error loading initial quiz results database: " + www.error);
//                quizResultsDatabase = new QuizResultDatabase(); // Initialize an empty database on error
//            }
//            else
//            {
//                string json = www.downloadHandler.text;
//                quizResultsDatabase = JsonUtility.FromJson<QuizResultDatabase>(json);
//                // Save the initial state to PlayerPrefs for subsequent runs
//                SaveQuizResultsDatabase();
//            }
//        }
//    }

//    public void SaveQuizResultsDatabase()
//    {
//        string json = JsonUtility.ToJson(quizResultsDatabase, true);
//        PlayerPrefs.SetString("QuizResultsDatabase", json);
//        PlayerPrefs.Save();
//    }

//    public void AddQuizResult(float scorePercent, Dictionary<string, int> categoryScores)
//    {
//        string username = GetCurrentUsername();

//        QuizResult newResult = new QuizResult
//        {
//            username = username,
//            scorePercent = scorePercent,
//            categoryScores = ConvertDictionaryToList(categoryScores)
//        };

//        quizResultsDatabase.results.Add(newResult);
//        SaveQuizResultsDatabase();
//    }

//    private string GetCurrentUsername()
//    {
//        // This method is just a placeholder. Implement according to how you manage user sessions.
//        return "Guest"; // Example username
//    }

//    public void UpdateQuizResultWithRank(string username, string rank)
//    {
//        QuizResult lastResult = quizResultsDatabase.results.LastOrDefault(result => result.username == username);
//        if (lastResult != null)
//        {
//            lastResult.rank = rank;
//            SaveQuizResultsDatabase();
//        }
//    }

//    private List<CategoryScore> ConvertDictionaryToList(Dictionary<string, int> dict)
//    {
//        return dict.Select(item => new CategoryScore { category = item.Key, score = item.Value }).ToList();
//    }
//    public QuizResult GetLastResultForCurrentUser()
//    {
//        string currentUsername = GetCurrentUsername(); // Ensure this method correctly identifies the current user
//        return quizResultsDatabase.results.LastOrDefault(result => result.username == currentUsername);
//    }

//}
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class CategoryScore
{
    public string category;
    public int score;
}

[System.Serializable]
public class QuizResult
{
    public string username;
    public float scorePercent; // This might be redundant if using score and totalScore directly
    public List<CategoryScore> categoryScores = new List<CategoryScore>();
    public int score; // Actual score obtained
    public int totalScore; // Total possible score
    public string feedback;
}

[System.Serializable]
public class QuizResultDatabase
{
    public List<QuizResult> results = new List<QuizResult>();
}

public class QuizResultsManager : MonoBehaviour
{
    public static QuizResultsManager Instance;

    private QuizResultDatabase quizResultsDatabase;
    private string resultsDatabasePath = "quizResults.json"; // File name in StreamingAssets

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (PlayerPrefs.HasKey("QuizResultsDatabase"))
            {
                string json = PlayerPrefs.GetString("QuizResultsDatabase");
                quizResultsDatabase = JsonUtility.FromJson<QuizResultDatabase>(json);
            }
            else
            {
                StartCoroutine(LoadQuizResultsDatabaseFromStreamingAssets(resultsDatabasePath));
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator LoadQuizResultsDatabaseFromStreamingAssets(string path)
    {
        string uri = Path.Combine(Application.streamingAssetsPath, path);
        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError("Error loading initial quiz results database: " + www.error);
                quizResultsDatabase = new QuizResultDatabase();
            }
            else
            {
                string json = www.downloadHandler.text;
                quizResultsDatabase = JsonUtility.FromJson<QuizResultDatabase>(json);
                SaveQuizResultsDatabase();
            }
        }
    }

    public void SaveQuizResultsDatabase()
    {
        string json = JsonUtility.ToJson(quizResultsDatabase, true);
        PlayerPrefs.SetString("QuizResultsDatabase", json);
        PlayerPrefs.Save();
    }

    public void AddQuizResult(int score, int totalScore, Dictionary<string, int> categoryScores)
    {
        string username = GetCurrentUsername();

        QuizResult newResult = new QuizResult
        {
            username = username,
            score = score,
            totalScore = totalScore,
            categoryScores = ConvertDictionaryToList(categoryScores)
        };

        quizResultsDatabase.results.Add(newResult);
        SaveQuizResultsDatabase();
    }

    private string GetCurrentUsername()
    {
        // This method is just a placeholder. Implement according to how you manage user sessions.
        return "Guest"; // Example username
    }

    private List<CategoryScore> ConvertDictionaryToList(Dictionary<string, int> dict)
    {
        return dict.Select(item => new CategoryScore { category = item.Key, score = item.Value }).ToList();
    }

    public QuizResult GetLastResultForCurrentUser()
    {
        string currentUsername = GetCurrentUsername();
        return quizResultsDatabase.results.LastOrDefault(result => result.username == currentUsername);
    }
    public void UpdateQuizResultWithFeedback(string username, string feedback)
    {
        // Find the last result for the user
        QuizResult lastResult = quizResultsDatabase.results.LastOrDefault(result => result.username == username);
        if (lastResult != null)
        {
            // Assuming you want to store the feedback in the QuizResult
            lastResult.feedback = feedback; // Make sure to add a `feedback` property to the QuizResult class
            SaveQuizResultsDatabase(); // Save the updated database to JSON or PlayerPrefs
        }
    }

}

