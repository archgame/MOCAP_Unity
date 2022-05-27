using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslatePRSFromJoint : MonoBehaviour
{
    public Transform joint;
    public bool offsetPosition = true;
    public bool followRotation = true;
    public Quaternion extraRotation = new Quaternion(0, 0, 0, 0);
    public float scale = 1.0f;

    private Vector3 startingDist;
    private Quaternion startingRot;


    // Start is called before the first frame update
    void Start()
    {
        startingDist = joint.position - transform.position;
        startingRot = joint.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if(offsetPosition){
            Vector3 position = (joint.position - startingDist);
            position.x *= scale;
            position.y *= scale;
            position.z *= scale;
            transform.position = position;
        } else {
            Vector3 position = joint.position;
            position.x *= scale;
            position.y *= scale;
            position.z *= scale;
            transform.position = position;
        }
        
        if(followRotation) {
            // Checking if any extra rotation has been applied
            float sumExtraRot = extraRotation.x + extraRotation.y + extraRotation.z + extraRotation.w;
            if (sumExtraRot != 0) {
                transform.rotation = joint.rotation * startingRot * extraRotation;
            } else {
                transform.rotation = joint.rotation * startingRot;
            }
        } 
        
    }
}
