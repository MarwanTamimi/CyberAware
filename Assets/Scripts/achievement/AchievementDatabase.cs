using UnityEngine;

[CreateAssetMenu(fileName = "New AchievementDatabase", menuName = "Achievement System/Achievement Database")]
public class AchievementDatabase : ScriptableObject
{
    public Achievement[] achievements; // This array should store all possible achievements
}
