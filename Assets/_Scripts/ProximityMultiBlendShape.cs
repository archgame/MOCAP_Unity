using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityMultiBlendShape : MonoBehaviour
{
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

    void Awake ()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer> ();
        skinnedMesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
    }

    void Start()
    {

    }

    void FixedUpdate()
    {
        // distance between objects
        dist = Vector3.Distance(object1.position, object2.position);

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
    }

    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s-a1)*(b2-b1)/(a2-a1);
    }
}
