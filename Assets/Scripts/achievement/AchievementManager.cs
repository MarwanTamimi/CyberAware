
//using UnityEngine;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;

//[System.Serializable]
//public class UserAchievement
//{
//    public string achievementId;
//    public bool unlocked;
//}

//[System.Serializable]
//public class UserAchievements
//{
//    public string username;
//    public List<UserAchievement> achievements = new List<UserAchievement>();
//}

//[System.Serializable]
//public class AchievementsDatabase
//{
//    public List<UserAchievements> usersAchievements = new List<UserAchievements>();
//}

//public class AchievementManager : MonoBehaviour
//{
//    public static AchievementManager Instance { get; private set; }

//    public AchievementDatabase database;
//    public AchievementController notificationController;

//    private AchievementsDatabase achievementsDatabase;
//    private string databasePath;
//    public string username = AuthManager.GetCurrentUserName();

//    void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject);

//            databasePath = Path.Combine(Application.dataPath, "Data/achievements.json");
//            LoadAchievementsDatabase();
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    private void LoadAchievementsDatabase()
//    {
//        if (File.Exists(databasePath))
//        {
//            string json = File.ReadAllText(databasePath);
//            achievementsDatabase = JsonUtility.FromJson<AchievementsDatabase>(json);
//        }
//        else
//        {
//            achievementsDatabase = new AchievementsDatabase();
//        }
//    }

//    public void SaveAchievementsDatabase()
//    {
//        string json = JsonUtility.ToJson(achievementsDatabase, true);
//        File.WriteAllText(databasePath, json);
//    }

//    public void UnlockAchievement(AchievementID id)
//    {
//        // Retrieve the current username from a session or similar
//        string username = GetCurrentUsername();
//        if (string.IsNullOrEmpty(username))
//        {
//            Debug.LogError("No current user session found. Cannot unlock achievement.");
//            return;
//        }

//        string achievementId = id.ToString();
//        var userAchievements = achievementsDatabase.usersAchievements.FirstOrDefault(u => u.username == username);

//        if (userAchievements == null)
//        {
//            userAchievements = new UserAchievements { username = username };
//            achievementsDatabase.usersAchievements.Add(userAchievements);
//        }

//        if (!userAchievements.achievements.Any(a => a.achievementId == achievementId))
//        {
//            userAchievements.achievements.Add(new UserAchievement { achievementId = achievementId, unlocked = true });
//            SaveAchievementsDatabase();

//            Debug.Log($"{username} unlocked achievement: {achievementId}");

//            // Display notification
//            var achievement = database.achievements.FirstOrDefault(a => a.id == achievementId);
//            if (achievement != null && notificationController != null)
//            {
//                notificationController.ShowNotification(achievement);
//            }
//        }
//        else
//        {
//            Debug.LogWarning($"{username} attempted to unlock already unlocked achievement: {achievementId}");
//        }
//    }
//    private string GetCurrentUsername()
//    {
//        // Assuming you have a UserSessionManager or similar that handles user sessions
//        // Replace with your actual method of retrieving the current user's username
//        // For example, reading from a file, PlayerPrefs, or a static variable
//        return AuthManager.GetCurrentUserName();
//    }

//    public bool IsAchievementUnlocked(AchievementID id)
//    {
//        string username1 = username.ToString();
//        string achievementId = id.ToString();
//        var userAchievements = achievementsDatabase.usersAchievements.FirstOrDefault(u => u.username == username);
//        return userAchievements != null && userAchievements.achievements.Any(a => a.achievementId == achievementId && a.unlocked);
//    }
//}
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SocialPlatforms.Impl;

[System.Serializable]
public class UserAchievement
{
    public string achievementId;
    public bool unlocked;
}

[System.Serializable]
public class UserAchievements
{
    public string username;
    public List<UserAchievement> achievements = new List<UserAchievement>();
}

[System.Serializable]
public class AchievementsDatabase
{
    public List<UserAchievements> usersAchievements = new List<UserAchievements>();
}

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }

    public string username = AuthManager.GetCurrentUserName();


    public AchievementDatabase database;
    public AchievementController notificationController;

    private AchievementsDatabase achievementsDatabase;
    private string databasePath = "achievements.json"; // Used only for initial load
                                                       //private string achievementId;

    //private object authmanager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Start the loading process
            LoadAchievementsDatabaseFromPlayerPrefs();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator LoadAchievementsDatabase()
    {
        string uri = Path.Combine(Application.streamingAssetsPath, databasePath);
        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError("Error loading achievements: " + www.error);
                achievementsDatabase = new AchievementsDatabase(); // Initialize an empty database on error
            }
            else
            {
                string json = www.downloadHandler.text;
                achievementsDatabase = JsonUtility.FromJson<AchievementsDatabase>(json);
                SaveAchievementsDatabase(); // Save the initial load to PlayerPrefs
            }
        }
    }

    private void LoadAchievementsDatabaseFromPlayerPrefs()
    {
        if (PlayerPrefs.HasKey("UserAchievements"))
        {
            string json = PlayerPrefs.GetString("UserAchievements");
            achievementsDatabase = JsonUtility.FromJson<AchievementsDatabase>(json);
            Debug.Log("Achievements database loaded from PlayerPrefs.");
        }
        else
        {
            Debug.Log("Starting coroutine to load achievements database from StreamingAssets.");
            StartCoroutine(LoadAchievementsDatabase());
        }
    }

    public void SaveAchievementsDatabase()
    {
        string json = JsonUtility.ToJson(achievementsDatabase, true);
        PlayerPrefs.SetString("UserAchievements", json);
        PlayerPrefs.Save();
        Debug.Log("Achievements database saved to PlayerPrefs.");
    }

    public void UnlockAchievement(AchievementID id)
    {
        string username = AuthManager.GetCurrentUserName();
        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("No current user session found. Cannot unlock achievement.");
            return;
        }

        if (achievementsDatabase == null)
        {
            Debug.LogError("Achievements database is not loaded.");
            return;
        }

        string achievementId = id.ToString();
        var userAchievements = achievementsDatabase.usersAchievements.FirstOrDefault(u => u.username == username);

        if (userAchievements == null)
        {
            userAchievements = new UserAchievements { username = username };
            achievementsDatabase.usersAchievements.Add(userAchievements);
        }

        if (userAchievements.achievements.Any(a => a.achievementId == achievementId))
        {
            Debug.LogWarning($"{username} attempted to unlock already unlocked achievement: {achievementId}");
            return;
        }

        userAchievements.achievements.Add(new UserAchievement { achievementId = achievementId, unlocked = true });
        SaveAchievementsDatabase();
        Debug.Log($"{username} unlocked achievement: {achievementId}");

        if (database == null)
        {
            Debug.LogError("Achievement database is null.");
            return;
        }

        var achievement = database.achievements.FirstOrDefault(a => a.id == achievementId);
        if (achievement == null)
        {
            Debug.LogError("Achievement not found in database.");
            return;
        }

        if (notificationController == null)
        {
            Debug.LogError("Notification controller is not set.");
            return;
        }

        notificationController.ShowNotification(achievement);
    }






    public bool IsAchievementUnlocked(AchievementID id)
    {
        string achievementId = id.ToString();
        var userAchievements = achievementsDatabase.usersAchievements.FirstOrDefault(u => u.username == AuthManager.GetCurrentUserName());
        return userAchievements != null && userAchievements.achievements.Any(a => a.achievementId == achievementId && a.unlocked);
    }
}
