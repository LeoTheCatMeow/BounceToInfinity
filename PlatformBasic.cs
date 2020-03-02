using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBasic : MonoBehaviour
{
    public Renderer lower;
    public Material originalMat;
    public Material highlightMat;
    public bool isOrigin;

    void OnEnable()
    {
        if (isOrigin)
        {
            lower.material = highlightMat;
        }    
    }
    void OnDisable()
    {       
        lower.material = originalMat;     
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && !isOrigin)
        {
            lower.material = highlightMat;
        }
    }
}
