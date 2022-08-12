using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class DataSubscription : MonoBehaviour
{
    //declare avatars, avatar[]= {head, hip, left hand, right hand, left foot, right foot}
    public GameObject[] avatar0;
    public GameObject[] avatar1;

    //declare vfx
    public VisualEffect[] effects;

    //declare velocity parameter
    private Vector3[] lastPosition;
    public float[] velocity;
    private int rigNum = 5;


    // Start is called before the first frame update
    void Start()
    {
        lastPosition = new Vector3[] { Vector3.zero, Vector3.zero };
    }

    // Update is called once per frame
    void Update()
    {
        Velocity(0, avatar0[1]);
    }

    public float Velocity(int avatarIndex, GameObject rig)
    {
        velocity[avatarIndex] = ((rig.transform.position - lastPosition[0]) / Time.deltaTime).magnitude;
        lastPosition[avatarIndex] = rig.transform.position;
        return velocity[avatarIndex];
    }
}
