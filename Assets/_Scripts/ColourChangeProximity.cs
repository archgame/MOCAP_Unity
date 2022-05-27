using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourChangeProximity : MonoBehaviour
{
    public Transform thisAvatar;
    public Transform otherAvatar;
    public float maxDist = 3.0f;

    public Color colorWhenFar = new Color(0.1f, 0.1f, 0.1f, 1.0f);
    public Color colorWhenClose = Color.cyan;
    
    Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer> ();
    }

    void FixedUpdate()
    {
        if (otherAvatar)
        {
            // distance with other avatar
            float dist = Vector3.Distance(otherAvatar.position, thisAvatar.position);
            //print("Distance to other: " + dist);

            // lerp colour with distance ratio
            float lerp = dist/maxDist;
            rend.material.color = Color.Lerp(colorWhenClose, colorWhenFar, lerp);
        }
        else
        {
            rend.material.color = colorWhenFar;
        }


    }
}
