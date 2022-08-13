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
    public int rigNumber = 5;

    //declare distance parameter
    public float[] dist;


    // Start is called before the first frame update
    void Start()
    {
        lastPosition = new Vector3[] { Vector3.zero, Vector3.zero };
    }

    // Update is called once per frame
    void Update()
    {
        //velocity calc
        Velocity(0, avatar0[rigNumber]);
        Velocity(1, avatar1[rigNumber]);
        //Debug.Log(velocity[0] + " ------ " + velocity[1]);

        //distance calc 
        dist[0] = Distance(avatar0[2], avatar0[3]);
        dist[1] = Distance(avatar1[2], avatar1[3]);

        //subscribtions
        effects[0].SetFloat("handHipDist", dist[0]);
        effects[1].SetFloat("handHipDist", dist[1]);

    }

    private void Velocity(int avatarIndex, GameObject rig)
    {
        velocity[avatarIndex] = ((rig.transform.position - lastPosition[avatarIndex]) / Time.deltaTime).magnitude;
        lastPosition[avatarIndex] = rig.transform.position;
        //Debug.Log("Velocity=" + velocity[avatarIndex]);
    }

    private float Distance(GameObject a, GameObject b)
    {
        float distance = Vector3.Distance(a.transform.position, b.transform.position);
        return distance;
    }
}
