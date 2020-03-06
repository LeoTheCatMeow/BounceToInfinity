using System.Collections;
using System.Collections.Generic;

public static class GameEvents 
{
    public delegate void voidNoParam();
    public static event voidNoParam _GameStart;
    public static event voidNoParam _PlayTutorial;
    public static event voidNoParam _GameOver;
    public static event voidNoParam _GameReset;
    public static event voidNoParam _ReturnToMenu;

    public delegate void voidFloat1(float value);
    public static event voidFloat1 _UpdateJumpCharge;
    public static event voidFloat1 _UpdateScore;

    public delegate void voidString1Float1(string info, float duration);
    public static event voidString1Float1 _DisplayPopUp;

    public delegate void voidInt1(int i);
    public static event voidInt1 _RelicCollected;
    public static event voidInt1 _RelicActivated;

    public static void GameStart()
    {
        _GameStart();  
    }

    public static void PlayTutorial()
    {
        _PlayTutorial();
    }
    public static void GameOver()
    {
        _GameOver();
    }

    public static void GameReset()
    {
        _GameReset();
    }

    public static void ReturnToMenu()
    {
        _ReturnToMenu();
    }

    public static void UpdateJumpCharge(float value)
    {
        _UpdateJumpCharge(value);
    }

    public static void UpdateScore(float value)
    {
        _UpdateScore(value);
    }

    public static void DisplayPopUp(string info, float duration)
    {
        _DisplayPopUp(info, duration);
    }

    public static void RelicCollected(int i)
    {
        _RelicCollected(i);
    }

    public static void RelicActivated(int i)
    {
        _RelicActivated(i);
    }
}
