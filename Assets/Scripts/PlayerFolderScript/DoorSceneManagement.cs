//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class DoorSceneManagement : MonoBehaviour
//{
//    public string sceneToLoad; // Assign this in the Inspector to the scene you want to load
  

//    private void OnTriggerEnter2D(Collider2D collider)
//    {
//        Debug.Log("OnTriggerEnter2D called.");
//        if (collider.CompareTag("Player"))
//        {
//            Debug.Log($"Player tagged object collided with door, loading scene: {sceneToLoad}");
//            SceneManager.LoadScene(sceneToLoad);
//        }
//        else
//        {
//            Debug.Log("Collided object is not tagged as Player.");
//        }
//    }
//}
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Needed for IEnumerator and Coroutines

public class DoorSceneManagement : MonoBehaviour
{
    public Animator transition; // Assign your Animator component in the Inspector
    public float transitionTime = 1f; // Duration of the transition animation
    public string sceneToLoad; // Assign this in the Inspector to the scene you want to load

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            StartCoroutine(LoadLevel(sceneToLoad));
        }
    }

    IEnumerator LoadLevel(string levelIndex)
    {
        // Trigger the animation
        transition.SetTrigger("Start");

        // Wait for the transition animation to finish
        yield return new WaitForSeconds(transitionTime);

        // Load the scene
        SceneManager.LoadScene(levelIndex);
    }
}

