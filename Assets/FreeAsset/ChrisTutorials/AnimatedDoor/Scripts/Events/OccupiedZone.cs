using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OccupiedZone : MonoBehaviour
{
    private List<Collider2D> occupants;
    private CanvasGroup doorCanvasGroup;

    public UnityEvent ZoneIsOccupied, ZoneContinuesToBeOccupied, LastOccupantLeft;
    public static OccupiedZone activeInstance;

    void Awake()
    {
        occupants = new List<Collider2D>();

        // Initialize CanvasGroup for fading the door
        doorCanvasGroup = GetComponent<CanvasGroup>();
        if (doorCanvasGroup == null)
        {
            doorCanvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        occupants.Add(collision);
        if (occupants.Count == 1 && ZoneIsOccupied.GetPersistentEventCount() != 0)
        {
            ZoneIsOccupied.Invoke();
            MazeQuiz.Instance.ShowNextQuestion();
            activeInstance = this;
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (ZoneContinuesToBeOccupied.GetPersistentEventCount() != 0)
        {
            ZoneContinuesToBeOccupied.Invoke();
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        occupants.Remove(collision);
        if (occupants.Count == 0 && LastOccupantLeft.GetPersistentEventCount() != 0)
        {
            LastOccupantLeft.Invoke();
        }
    }

    // Call this method to start fading out the door
    public IEnumerator AnimateDoorOut()
    {
        // Ensure there's a CanvasGroup to manage the door's visibility
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Animation settings
        float duration = 0.5f; // Total animation duration
        float currentTime = 0f;
        Vector3 originalScale = transform.localScale; // Store the original scale

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float progress = currentTime / duration;

            // Animate fade out
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, progress);

            // Animate scale down
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, progress);

            yield return null;
        }

        gameObject.SetActive(false); // Optionally deactivate the door entirely
    }




}
