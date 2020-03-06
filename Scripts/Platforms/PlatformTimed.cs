using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTimed : MonoBehaviour
{
    public ParticleSystem deathEffect;
    public Renderer core;
    public Color coreStartColor;
    public Color coreDeathColor;

    private MaterialPropertyBlock matProperties;
    private bool triggered;

    void Awake()
    {
        matProperties = new MaterialPropertyBlock();
        matProperties.SetColor("_EmissionColor", coreStartColor);
        core.SetPropertyBlock(matProperties);
    }

    void OnDisable()
    {
        StopAllCoroutines();
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().isTrigger = false;
        triggered = false;

        matProperties.SetColor("_EmissionColor", coreStartColor);
        core.SetPropertyBlock(matProperties);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && !triggered)
        {
            triggered = true;
            deathEffect.Play(true);
            StartCoroutine(Fall());
        }
    }

    private IEnumerator Fall()
    {
        core.GetPropertyBlock(matProperties);
        for (int i = 0; i < 60; i++)
        {
            matProperties.SetColor("_EmissionColor", Color.Lerp(coreStartColor, coreDeathColor, i / 60f));
            core.SetPropertyBlock(matProperties);
            yield return new WaitForSeconds(0.02f);
        }
        yield return new WaitForSeconds(0.8f);

        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Collider>().isTrigger = true;
    }
}
