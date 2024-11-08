using UnityEngine;

public static class PlayerProgress
{
    // Set Stage as completed
    public static void MarkLevelCompleted(int levelNumber)
    {
        PlayerPrefs.SetInt(levelNumber.ToString(), 1); 
        PlayerPrefs.Save(); 
    }
    
    // Get Stage completion status
    public static bool IsLevelCompleted(int levelNumber)
    {
        return PlayerPrefs.GetInt(levelNumber.ToString(), 0) == 1;
    }
}