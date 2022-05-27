using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFollow : MonoBehaviour
{
    public GameObject avatar;
    public Transform neck;
    float rotationAmt;
    public float offset = 90.0f;
    Quaternion offsetRotation;
    Vector3 rotAngle; 


    void Start() {
        offsetRotation = transform.rotation;
    }

    void Update()
    {
        
        //transform.LookAt(neck);
        //transform.LookAt(avatar);

        // rotationAmt = neck.transform.rotation.y + offset;

        // transform.Rotate(rotationAmt, 0, 0, Space.World);

        transform.position = new Vector3(avatar.transform.position.x, transform.position.y, avatar.transform.position.z);
        
        transform.rotation = neck.rotation * offsetRotation ;
    }
}
