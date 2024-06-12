using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;

public class EmailManager : MonoBehaviour
{
    public EmailList emailList;

    private List<int> viewedEmailIndices = new List<int>();
    public Dictionary<int, bool> emailResponses = new Dictionary<int, bool>();
    private int currentEmailIndex = 0;
    private string emailDataPath = "emails.json";

    //private string emailDataPath = "Data/emails.json";



    void Start()
    {
        StartCoroutine(LoadEmailDataFromStreamingAssets(emailDataPath));
    }

    // Call this method to get a random email from the loaded emails
    public Email GetRandomEmail()
    {
        if (emailList != null && emailList.emails.Length > 0)
        {
            int randomIndex = Random.Range(0, emailList.emails.Length); // Get a random index
            return emailList.emails[randomIndex]; // Return the email at the random index
        }
        else
        {
            Debug.LogError("Email list is empty or not loaded.");
            return null; // Return null if there's no email to return
        }
    }

    //void LoadEmailData()
    //{
    //    string filePath = Path.Combine(Application.dataPath, "Data/emails.json");
    //    if (File.Exists(filePath))
    //    {
    //        string dataAsJson = File.ReadAllText(filePath);
    //        emailList = JsonUtility.FromJson<EmailList>(dataAsJson);
    //    }
    //    else
    //    {
    //        Debug.LogError("Cannot find emails.json file.");
    //    }
   // }
    private IEnumerator LoadEmailDataFromStreamingAssets(string path)
    {
        string uri = Path.Combine(Application.streamingAssetsPath, path);
        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError("Error loading email data: " + www.error);
            }
            else
            {
                string json = www.downloadHandler.text;
                emailList = JsonUtility.FromJson<EmailList>(json);
            }
        }
    }
public Email[] GetEmails()
{
    return emailList?.emails ?? new Email[0];
}
public void SaveChallengeState()
    {
        PlayerPrefs.SetInt("CurrentEmailIndex", currentEmailIndex);
        PlayerPrefs.SetString("ViewedEmailIndices", JsonUtility.ToJson(new SerializableList<int>(viewedEmailIndices)));
        PlayerPrefs.SetString("EmailResponses", JsonUtility.ToJson(new SerializableDictionary<int, bool>(emailResponses)));
        PlayerPrefs.Save();
    }

    public void LoadChallengeState()
    {
        if (PlayerPrefs.HasKey("CurrentEmailIndex"))
        {
            currentEmailIndex = PlayerPrefs.GetInt("CurrentEmailIndex");
            string viewedIndicesJson = PlayerPrefs.GetString("ViewedEmailIndices");
            string emailResponsesJson = PlayerPrefs.GetString("EmailResponses");

            viewedEmailIndices = JsonUtility.FromJson<SerializableList<int>>(viewedIndicesJson).list;
            emailResponses = JsonUtility.FromJson<SerializableDictionary<int, bool>>(emailResponsesJson).dictionary;
        }
    }

    public void ResetChallengeState()
    {
        viewedEmailIndices.Clear();
        emailResponses.Clear();
        currentEmailIndex = 0;

        SaveChallengeState();
    }
    public bool HasSavedState()
    {
        // Check if there is a saved state for the current email index or any saved responses
        return PlayerPrefs.HasKey("CurrentEmailIndex") || PlayerPrefs.HasKey("ViewedEmailIndices") || PlayerPrefs.HasKey("EmailResponses");
    }

    // Add utility methods to access and manipulate the challenge state as needed by your game logic
}

// Helper class to enable serialization of a list
[System.Serializable]
public class SerializableList<T>
{
    public List<T> list = new List<T>();
    public SerializableList(List<T> newList)
    {
        list = newList;
    }
}

// Helper class to enable serialization of a dictionary
[System.Serializable]
public class SerializableDictionary<TKey, TValue>
{
    public Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
    public SerializableDictionary(Dictionary<TKey, TValue> newDict)
    {
        dictionary = newDict;
    }
}