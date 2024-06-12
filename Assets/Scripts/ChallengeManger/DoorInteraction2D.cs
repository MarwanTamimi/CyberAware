using System;
using UnityEngine;

public class DoorInteraction2D : MonoBehaviour
{
    public string requiredAchievementId;
    public Collider2D doorCollider; // Ensure this is correctly assigned in the Unity Editor

    private void Start()
    {
        // Ensure the door is initially "closed" by enabling its collider
        CloseDoor();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider is from the player
        if (other.CompareTag("Player"))
        {
            AttemptToOpenDoor();
        }
    }

    private void AttemptToOpenDoor()
    {
        // Attempt to parse the string ID to the AchievementID enum
        if (Enum.TryParse(requiredAchievementId, out AchievementID achievementID))
        {
            bool isUnlocked = AchievementManager.Instance.IsAchievementUnlocked(achievementID);
            Debug.Log($"Is achievement '{requiredAchievementId}' unlocked? {isUnlocked}");

            if (isUnlocked)
            {
                OpenDoor();
            }
            else
            {
                CloseDoor();
                Debug.Log("Door remains closed. Complete the required achievement to unlock.");
            }
        }
        else
        {
            Debug.LogError($"Invalid achievement ID: {requiredAchievementId}");
        }
    }



    private void OpenDoor()
    {
        Debug.Log("Door is now open.");
        // Disable the collider to allow passage
        doorCollider.enabled = false;
    }

    private void CloseDoor()
    {
        Debug.Log("Door is closed.");
        // Enable the collider to block passage
        doorCollider.enabled = true;
    }
}
