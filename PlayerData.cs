using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerData : MonoBehaviour
{
    //saved
    public static float volumePref;
    public static float moveSensitivityPref;
    public static float jumpSensitivityPref;
    public static float turnSensitivityPref;
    public static bool tutorialEnabled;
    public static float bestScore;
    public static float totalPlayTime;
    public static int numberOfAttempts;
    public static float totalDistance;
    public static int relicsCollected;
    public static int relicActivated;

    //references
    public AudioMixer masterAudio;
    static PlayerData instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadData();
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    void Start()
    {
        masterAudio.SetFloat("MasterVolume", (volumePref - 0.5f) * 20f);
    }

    void Update()
    {
        totalPlayTime += Time.deltaTime;
    }

    public void AdjustVolumePref(float value)
    {
        volumePref = value / 100f;
        masterAudio.SetFloat("MasterVolume", (volumePref - 0.5f) * 20f);
    }

    public void AdjustMoveSensitivityPref(float value)
    {
        moveSensitivityPref = value / 100f;
    }

    public void AdjustJumpSensitivityPref(float value)
    {
        jumpSensitivityPref = value / 100f;
    }

    public void AdjustTurnSensitivityPref(float value)
    {
        turnSensitivityPref = value / 100f;
    }

    public void AdjustTutorialEnabledState(bool state)
    {
        tutorialEnabled = state;
    }

    void OnApplicationPause(bool pause)
    {
        SaveData();
    }

    void OnApplicationQuit()
    {
        SaveData();
    }

    private void SaveData()
    {
        PlayerPrefs.SetFloat("VP", volumePref);
        PlayerPrefs.SetFloat("MP", moveSensitivityPref);
        PlayerPrefs.SetFloat("JP", jumpSensitivityPref);
        PlayerPrefs.SetFloat("TP", turnSensitivityPref);
        PlayerPrefs.SetInt("TU", tutorialEnabled ? 1 : 0);
        PlayerPrefs.SetFloat("BS", bestScore);
        PlayerPrefs.SetFloat("T", totalPlayTime);
        PlayerPrefs.SetInt("N", numberOfAttempts);
        PlayerPrefs.SetFloat("D", totalDistance);
        PlayerPrefs.SetInt("R", relicsCollected);
        PlayerPrefs.SetInt("RA", relicActivated);
        PlayerPrefs.Save();
    }

    private void LoadData()
    {
        volumePref = PlayerPrefs.GetFloat("VP", 0.5f);
        moveSensitivityPref = PlayerPrefs.GetFloat("MP", 0.5f);
        jumpSensitivityPref = PlayerPrefs.GetFloat("JP", 0.5f);
        turnSensitivityPref = PlayerPrefs.GetFloat("TP", 0.5f);
        tutorialEnabled = PlayerPrefs.GetInt("TU", 1) == 0 ? false : true;
        bestScore = PlayerPrefs.GetFloat("BS", 0f);
        totalPlayTime = PlayerPrefs.GetFloat("T", 0f);
        numberOfAttempts = PlayerPrefs.GetInt("N", 0);
        totalDistance = PlayerPrefs.GetFloat("D", 0f);
        relicsCollected = PlayerPrefs.GetInt("R", 0);
        relicActivated = PlayerPrefs.GetInt("RA", -1);
    }
}
