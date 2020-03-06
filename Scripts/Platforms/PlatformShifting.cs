using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformShifting : MonoBehaviour
{
    public ParticleSystem contactEffect;

    private Transform passenger;
    private Vector3 direction;
    private void OnEnable()
    {
        int sign = Random.Range(0, 2) * 2 - 1;
        if (Random.value > 0.5)
        {
            direction = sign * transform.right;
        } else
        {
            direction = sign * transform.forward;
        }
        StartCoroutine(Shift());
    }
    private void OnDisable()
    {
        StopAllCoroutines();
        passenger = null;
    }

    private IEnumerator Shift()
    {
        while (true)
        {
            for (int i = 0; i < 50; i++)
            {
                transform.position += direction * 1.3f / 50f;
                if (passenger != null)
                {
                    passenger.transform.position += direction * 1.3f / 50f;
                }
                yield return new WaitForFixedUpdate();
            }

            yield return new WaitForSeconds(1.5f);

            for (int i = 0; i < 50; i++)
            {
                transform.position -= direction * 1.3f / 50f;
                if (passenger != null)
                {
                    passenger.transform.position -= direction * 1.3f / 50f;
                }
                yield return new WaitForFixedUpdate();
            }

            yield return new WaitForSeconds(1.5f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            passenger = collision.transform;
            contactEffect.Play(true);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            passenger = null;
        }
    }
}
