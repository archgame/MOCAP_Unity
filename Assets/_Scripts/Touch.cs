using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Touch : MonoBehaviour
{
    public GameObject bodyPart; // link mesh that will be affected by this collider
    public Material matDefault;
    public Material matTouched;

    int children;
    Renderer rend;

    float t;
    float startTime;
    float stayOnDuration = 7;
    float fadeDuration = 3;

    Material prevMat;
    

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time - stayOnDuration - fadeDuration*2; // Initiate start time with value out of bounds

        // Set material for linked mesh
        rend = bodyPart.GetComponent<Renderer> ();
        rend.material = matDefault;
    }

    // Update is called once per frame
    void Update()
    { 
        t = Time.time;

        // Manage material updates when touched by the other hand
        if(rend.material != prevMat) {
            startTime = Time.time;
        }

        // Fade out touched material after a certain duration
        if(t >= startTime + stayOnDuration) {
            if( t < startTime + stayOnDuration + fadeDuration) {
                float lerp = map(t, startTime+stayOnDuration, startTime+stayOnDuration+fadeDuration, 0, 1);
                rend.material.Lerp(matTouched, matDefault, lerp); 
            } else {
                rend.material = matDefault;
                prevMat = rend.material;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {   // This will only be triggered by the HAND collider which has a Rigidbody
        startTime = Time.time;

        // Filtered via tag to avoid dancers activating their own colliders while dancing
        // Dancers only activate new material on other dancers
        if(other.tag != gameObject.tag) {
            // Debug.Log("Something touched me!");
            rend.material = matTouched;
            prevMat = rend.material;
        }

    }

    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s-a1)*(b2-b1)/(a2-a1);
    }
}
