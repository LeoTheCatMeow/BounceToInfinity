using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformFallthrough : MonoBehaviour
{
    public Renderer core;

    void OnDisable()
    {
        core.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GetComponent<AudioSource>().Play();
            StartCoroutine(FallthroughAnimation());
        }
    }

    private IEnumerator FallthroughAnimation()
    {
        for (int i = 0; i < 4; i++)
        {
            core.enabled = !core.enabled;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
