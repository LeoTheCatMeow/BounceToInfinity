using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Relic : MonoBehaviour
{
    public GameObject onCollectEffect;

    void FixedUpdate()
    {
        transform.eulerAngles += new Vector3(0f, 1f, 0f);        
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.tag == "Player")
        { 
            GameEvents.RelicCollected(transform.GetSiblingIndex());
            GameEvents.DisplayPopUp(gameObject.name + " collected", 3f);

            GameObject effect = Instantiate(onCollectEffect, transform.position, Quaternion.identity);
            Destroy(effect, 3f);
            gameObject.SetActive(false);
        }
    }
}
