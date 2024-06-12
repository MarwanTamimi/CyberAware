using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Include this for scene management

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float remainingTime;

    void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
        }
        else if (remainingTime <= 0) // Changed from < to <= to include when time is exactly 0
        {
            ReloadScene();
            return; // Early return to prevent further execution in this update
        }

        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void ReloadScene()
    {
        // Call the RestartQuiz method on the MazeQuiz instance
        if (MazeQuiz.Instance != null)
        {
            MazeQuiz.Instance.RestartQuiz();
        }
        else
        {
            // Fallback in case there's no MazeQuiz instance for some reason
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
