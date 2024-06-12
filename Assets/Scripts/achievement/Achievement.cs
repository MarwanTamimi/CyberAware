using UnityEngine;

[System.Serializable]
public class Achievement
{
    public string id; // Unique identifier for the achievement
    public string title; // The title of the achievement
    public string description; // A brief description of the achievement
    [HideInInspector] public bool Unlocked = false; // Tracks whether the achievement is unlocked
}
