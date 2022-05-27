using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityAnimationBlendShape : MonoBehaviour {

    Animator anim;
    public float timeFloat;

    public Transform object1;
    public Transform object2;
    public float maxDistThreshold = 3.0f;
    public float minDist = 0;
    public float dist;

    public bool reverseBlend = false;
    public int numOfBlends = 1;

    SkinnedMeshRenderer skinnedMeshRenderer;
    Mesh skinnedMesh;
    float blend = 0f;

    void Start()
    {
        //Get the animator, which you attach to the GameObject you are intending to animate.
        anim = transform.parent.gameObject.GetComponent<Animator>();

        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer> ();
        skinnedMesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
    }

    void Update()
    {
        // distance between objects
        dist = Vector3.Distance(object1.position, object2.position);

        //Note: the petal model used has both a blend shape AND
        //an animation. So we map them both to the distance
        //between objects

        // Map blendshape key value to distance between objects
        if (reverseBlend) {
            if(dist<= maxDistThreshold) {
                blend = map(dist, minDist, maxDistThreshold, 100f, 0f);
                for(int i=0; i<numOfBlends; i++) {
                    skinnedMeshRenderer.SetBlendShapeWeight (i, blend);
                }
                
            } else {
                blend = 0f;
                for(int i=0; i<numOfBlends; i++) {
                    skinnedMeshRenderer.SetBlendShapeWeight (i, blend);
                }
            }
        } else {
            if(dist<= maxDistThreshold) {
                blend = map(dist, minDist, maxDistThreshold, 0f, 100f);
                for(int i=0; i<numOfBlends; i++) {
                    skinnedMeshRenderer.SetBlendShapeWeight (i, blend);
                }
            } else {
                blend = 100f;
                for(int i=0; i<numOfBlends; i++) {
                    skinnedMeshRenderer.SetBlendShapeWeight (i, blend);
                }
            }
        }

        // map distance between objects to animator
        timeFloat = map(dist, minDist, maxDistThreshold, 0f, 1f); 
        anim.Play("CINEMA_4D_Main", 0, timeFloat);
    }

    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s-a1)*(b2-b1)/(a2-a1);
    }

}
