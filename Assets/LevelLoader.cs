using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;

    // Call this wherever you want to load the next level
    //void Update()
    //{
    //    // This checks if the player has pressed the space bar.
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        // If they have, we call the LoadNextLevel method.
    //        LoadNextLevel();
    //    }
    //}
        public void LoadNextLevel()
    {
        // Start the coroutine to load the next level with the index after the current one
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        // Play animation
        transition.SetTrigger("Start");

        // Wait for the animation to finish
        yield return new WaitForSeconds(transitionTime);

        // Load the scene
        SceneManager.LoadScene(levelIndex);
    }
}
