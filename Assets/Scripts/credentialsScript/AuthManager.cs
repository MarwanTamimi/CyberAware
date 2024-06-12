//using System.Collections.Generic;
//using UnityEngine;
//using System.IO;
//using TMPro;
//using UnityEngine.SceneManagement;
//using static System.TimeZoneInfo;
//using System.Collections;

//[System.Serializable]
//public class User
//{
//    public string username;
//    public string email;
//    public string password; // Storing passwords in plain text is insecure!
//}

//[System.Serializable]
//public class UserDatabase
//{
//    public List<User> users = new List<User>();
//}
//[System.Serializable]
//public class UserSession
//{
//    public string username;
//}



//public class AuthManager : MonoBehaviour
//{
//    [Header("Login")]
//    public TMP_InputField usernameLoginField;
//    public TMP_InputField passwordLoginField;

//    [Header("Register")]
//    public TMP_InputField usernameRegisterField;
//    public TMP_InputField emailRegisterField;
//    public TMP_InputField passwordRegisterField;
//    public TMP_InputField passwordRegisterVerifyField;

//    private UserDatabase userDatabase = new UserDatabase();
//    private string databasePath;
//    private string sessionPath;


//    private static UserSession currentUserSession;
//    void Awake()
//    {
//        databasePath = Path.Combine(Application.dataPath, "Data/userDatabase.json");
//        sessionPath = Path.Combine(Application.dataPath, "Data/userSession.json");
//        LoadUserDatabase();
//        LoadAndSetUserSession();
//    }

//    private void LoadUserDatabase()
//    {
//        // Load the user database from JSON
//        if (File.Exists(databasePath))
//        {
//            string json = File.ReadAllText(databasePath);
//            userDatabase = JsonUtility.FromJson<UserDatabase>(json);
//        }
//    }
//    private void LoadAndSetUserSession()
//    {
//        UserSession loadedSession = LoadUserSession();
//        if (loadedSession != null)
//        {
//            currentUserSession = loadedSession; // Update the static variable
//            Debug.Log($"Session loaded for user: {currentUserSession.username}");
//        }
//        else
//        {
//            Debug.Log("No user session found.");
//        }
//    }
//    private void SaveUserDatabase()
//    {
//        // Save the user database to JSON
//        string json = JsonUtility.ToJson(userDatabase, true);
//        File.WriteAllText(databasePath, json);
//    }
//    private void SaveUserSession(string username)
//    {
//        UserSession session = new UserSession { username = username };
//        string json = JsonUtility.ToJson(session, true);
//        File.WriteAllText(sessionPath, json);
//    }

//    private UserSession LoadUserSession()
//    {
//        if (File.Exists(sessionPath))
//        {
//            string json = File.ReadAllText(sessionPath);
//            return JsonUtility.FromJson<UserSession>(json);
//        }
//        return null; 
//    }
//    public static string GetCurrentUserName()
//    {
//        return currentUserSession != null ? currentUserSession.username : "Guest";
//    }

//    public void LoginButton()
//    {
//        // Check the user's credentials
//        User user = userDatabase.users.Find(x => x.username == usernameLoginField.text);
//        if (user != null && user.password == passwordLoginField.text)
//        {

//            // User authenticated
//            Debug.Log("Logged in successfully.");

//            SaveUserSession(usernameLoginField.text);
//            // Proceed to log the user in
//            SceneManager.LoadScene(("Menu"));
//        }
//        else
//        {
//            Debug.Log("Login failed. Please check your username and password.");
//        }
//    }

//    public void RegisterButton()
//    {
//        // Validate new user data and register user
//        if (passwordRegisterField.text != passwordRegisterVerifyField.text)
//        {
//            Debug.Log("Passwords do not match!");
//            return;
//        }

//        if (userDatabase.users.Exists(x => x.username == usernameRegisterField.text))
//        {
//            Debug.Log("Username already exists!");
//            return;
//        }

//        User newUser = new User
//        {
//            username = usernameRegisterField.text,
//            email = emailRegisterField.text,
//            password = passwordRegisterField.text // Storing passwords in plain text is insecure!
//        };

//        userDatabase.users.Add(newUser);
//        SaveUserDatabase();
//        SaveUserSession(usernameRegisterField.text);

//        Debug.Log("Registration successful!");
//        // Proceed to log the new user in or return to the login screen
//        SceneManager.LoadScene(("Menu"));
//    }

//}
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class User
{
    public string username;
    public string email;
    public string password; 
}

[System.Serializable]
public class UserDatabase
{
    public List<User> users = new List<User>();
}

[System.Serializable]
public class UserSession
{
    public string username;
}

public class AuthManager : MonoBehaviour
{
    [Header("Login")]
    public TMP_InputField usernameLoginField;
    public TMP_InputField passwordLoginField;

    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;

    private UserDatabase userDatabase = new UserDatabase();
    private string databasePath = "userDatabase.json"; // Name of the file in StreamingAssets
    private static UserSession currentUserSession;

    void Awake()
    {
        LoadUserDatabase();
        LoadAndSetUserSession();
    }

    private IEnumerator LoadUserDatabaseFromStreamingAssets(string path)
    {
        string uri = Path.Combine(Application.streamingAssetsPath, path);
        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError("Error loading user database: " + www.error);
                userDatabase = new UserDatabase(); // Initialize an empty database on error
            }
            else
            {
                string json = www.downloadHandler.text;
                userDatabase = JsonUtility.FromJson<UserDatabase>(json);
            }
        }
    }

    private void LoadUserDatabase()
    {
       
        if (PlayerPrefs.HasKey("UserDatabase"))
        {
            string json = PlayerPrefs.GetString("UserDatabase");
            userDatabase = JsonUtility.FromJson<UserDatabase>(json);
        }
        else
        {
            // As a fallback, load from StreamingAssets
            StartCoroutine(LoadUserDatabaseFromStreamingAssets(databasePath));
        }
    }

    private void SaveUserDatabase()
    {
        string json = JsonUtility.ToJson(userDatabase, true);
        PlayerPrefs.SetString("UserDatabase", json);
        PlayerPrefs.Save();
    }

    private void LoadAndSetUserSession()
    {
        if (PlayerPrefs.HasKey("UserSession"))
        {
            string json = PlayerPrefs.GetString("UserSession");
            currentUserSession = JsonUtility.FromJson<UserSession>(json);
            Debug.Log($"Session loaded for user: {currentUserSession.username}");
        }
        else
        {
            Debug.Log("No user session found.");
        }
    }

    private void SaveUserSession(string username)
    {
        UserSession session = new UserSession { username = username };
        string json = JsonUtility.ToJson(session, true);
        PlayerPrefs.SetString("UserSession", json);
        PlayerPrefs.Save();
    }

    public static string GetCurrentUserName()
    {
        return currentUserSession != null ? currentUserSession.username : "Guest";
    }

    public void LoginButton()
    {
        User user = userDatabase.users.Find(x => x.username == usernameLoginField.text && x.password == passwordLoginField.text);
        if (user != null)
        {
            Debug.Log("Logged in successfully.");
            SaveUserSession(usernameLoginField.text);
            SceneManager.LoadScene("Menu");
        }
        else
        {
            Debug.Log("Login failed. Please check your username and password.");
        }
    }

    public void RegisterButton()
    {
        if (passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            Debug.Log("Passwords do not match!");
            return;
        }
        if (userDatabase.users.Exists(x => x.username == usernameRegisterField.text))
        {
            Debug.Log("Username already exists!");
            return;
        }
        User newUser = new User
        {
            username = usernameRegisterField.text,
            email = emailRegisterField.text,
            password = passwordRegisterField.text
        };
        userDatabase.users.Add(newUser);
        SaveUserDatabase();
        SaveUserSession(usernameRegisterField.text);
        Debug.Log("Registration successful!");
        SceneManager.LoadScene("Menu");
    }
}

