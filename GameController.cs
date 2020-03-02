using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    void Start()
    {
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
