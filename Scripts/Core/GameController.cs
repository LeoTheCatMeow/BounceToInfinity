using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    void Awake()
    {
        //QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    public void StartGame()
    {
        if (PlayerData.tutorialEnabled)
        {
            GameEvents.PlayTutorial();
        } else
        {
            GameEvents.GameStart();
        }
    }

    public void ReturnToMenu()
    {
        GameEvents.GameReset();
        GameEvents.ReturnToMenu();
    }
}
