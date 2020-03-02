using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Slider volume;
    public Slider moveSensitivity;
    public Slider jumpSensitivity;
    public Slider turnSensitivity;
    public Toggle enableTutorial;

    void OnEnable()
    {
        volume.value = PlayerData.volumePref * 100f;
        moveSensitivity.value = PlayerData.moveSensitivityPref * 100f;
        jumpSensitivity.value = PlayerData.jumpSensitivityPref * 100f;
        turnSensitivity.value = PlayerData.turnSensitivityPref * 100f;
        enableTutorial.isOn = PlayerData.tutorialEnabled;
    }
}
