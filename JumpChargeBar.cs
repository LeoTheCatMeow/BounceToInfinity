using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpChargeBar : MonoBehaviour
{
    public GameObject background;

    void Awake()
    {
        GameEvents._UpdateJumpCharge += UpdateJumpCharge;
    }

    void OnDestroy()
    {
        GameEvents._UpdateJumpCharge -= UpdateJumpCharge;
    }

    void OnEnable()
    {
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(false);
        }

        background.SetActive(false);
    }

    private void UpdateJumpCharge(float value)
    {
       for (int i = 1; i <= transform.childCount; i++)
        {
            if (((float)i) / ((float)transform.childCount) <= value)
            {
                transform.GetChild(i - 1).gameObject.SetActive(true);
            } else
            {
                transform.GetChild(i - 1).gameObject.SetActive(false);
            }
        }

       if (value > 0)
        {
            background.SetActive(true);
        } else
        {
            background.SetActive(false);
        }
    }
}
