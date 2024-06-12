using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class FinishWall : MonoBehaviour
{
    public string sceneToLoad;
    public GameObject incompleteQuizPanel; // Assign in Inspector

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (MazeQuiz.QuizCompleted)
            {
                Debug.Log("Quiz completed, loading next scene.");
                SceneManager.LoadScene(sceneToLoad);
            }
            else
            {
                StartCoroutine(ShowIncompleteQuizPanel());
            }
        }
    }

    IEnumerator ShowIncompleteQuizPanel()
    {
        CanvasGroup canvasGroup = incompleteQuizPanel.GetComponent<CanvasGroup>();
        ResetCanvasGroupAlpha(canvasGroup);
        incompleteQuizPanel.SetActive(true);

        yield return new WaitForSeconds(3); // Wait for 3 seconds
        StartCoroutine(FadeOutPanel(incompleteQuizPanel.GetComponent<CanvasGroup>()));

    }
        void ResetCanvasGroupAlpha(CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = 1f; // Reset alpha to make sure the panel is fully visible
            canvasGroup.blocksRaycasts = true; // Enable interaction (if needed)
        }
        IEnumerator FadeOutPanel(CanvasGroup canvasGroup)
        {
            float duration = 1f;
            float currentTime = 0f;
            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, currentTime / duration);
                yield return null;
            }
            incompleteQuizPanel.SetActive(false);
        }
    }

