using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Thanks to Gamad on YouTube for their video on leveling systems. https://www.youtube.com/watch?v=ptcx-NnhQTI

public class LevelSystem : MonoBehaviour
{

    public int xp = 0;
    public int currentLevel = 0;
    public int nextLevelXP = 0;
    public int xpNeededForNextLevel = 0;

    public void AddXpToPlayerLevel(int _xp)
    {
        xp += _xp;
        int _currentLevel = (int)(0.1f * Mathf.Sqrt(xp));

        if(_currentLevel != currentLevel)
        {
            currentLevel = _currentLevel;
        }

        int _xpForNextLevel = 100 * (currentLevel + 1) * (currentLevel + 1);
        int _difference = _xpForNextLevel - xp;
        nextLevelXP = _xpForNextLevel;
        xpNeededForNextLevel = _difference;
    }
}
