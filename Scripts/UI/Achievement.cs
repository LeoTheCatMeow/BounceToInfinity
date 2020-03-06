using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Achievement : MonoBehaviour
{
    public Text bestScore;
    public Text totalPlayTime;
    public Text numberOfAttempts;
    public Text totalDistance;

    void OnEnable()
    {
        bestScore.text = PlayerData.bestScore.ToString("#.00");
        totalPlayTime.text = (PlayerData.totalPlayTime / 3600f).ToString("#.00") + " H";
        numberOfAttempts.text = PlayerData.numberOfAttempts.ToString();
        totalDistance.text = PlayerData.totalDistance.ToString("#.00");
    }

    public void ClearAll()
    {
        bestScore.text = "0.00";
        totalPlayTime.text = "0.00 H";
        numberOfAttempts.text = "0";
        totalDistance.text = "0.00";
        PlayerData.bestScore = 0f;
        PlayerData.totalPlayTime = 0f;
        PlayerData.numberOfAttempts = 0;
        PlayerData.totalDistance = 0f;
    }

}
