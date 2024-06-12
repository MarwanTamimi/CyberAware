using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    // Screen object variables
    public GameObject loginUI;
    public GameObject registerUI;

    // Names of the scenes
    public string gameSceneName = "GameScene"; // Replace with game scene name
    public string mainMenuSceneName = "MainMenu"; // Replace with main menu scene name

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(gameObject);
        }
    }

    // Functions to change the login screen UI
    public void LoginScreen() // Back button
    {
        loginUI.SetActive(true);
        registerUI.SetActive(false);
    }

    public void RegisterScreen() // Register button
    {
        loginUI.SetActive(false);
        registerUI.SetActive(true);
    }
}