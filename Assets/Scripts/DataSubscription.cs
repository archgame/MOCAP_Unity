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
    public GameObject[] grids;
    [SerializeField] private Vector2[] handDistV2;
    [SerializeField] private Material[] mtl;

    //declare velocity parameter
    private Vector3[] lastPosition;
    private Vector3[] lastPosition2;
    public float[] velocity;
    public int rigNumber = 5;

    //declare distance parameter
    public float[] dist;


    // Start is called before the first frame update
    void Start()
    {
        mtl[0] = grids[0].GetComponent<MeshRenderer>().material;
        mtl[1] = grids[1].GetComponent<MeshRenderer>().material;
        lastPosition = new Vector3[] { Vector3.zero, Vector3.zero };
        lastPosition2 = new Vector3[] { Vector3.zero, Vector3.zero };
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
        mtl[0].SetFloat("_handDistAf", dist[0]);
        mtl[0].SetFloat("_handDistBf", dist[1]);
        handDistV2[0] = new Vector2(dist[0], dist[0]);
        handDistV2[1] = new Vector2(dist[1], dist[1]);
        mtl[1].SetVector("_handDistA", handDistV2[0]);
        mtl[1].SetVector("_handDistB", handDistV2[1]);


        float a = VelocityFloat(0, avatar0[5]); float b = VelocityFloat(1, avatar1[5]);
        effects[2].SetFloat("attractForceStrength", Mathf.Max(0.5f,a/2));
        effects[2].SetFloat("attractForceStrengthB", Mathf.Max(0.5f, b / 2));
        effects[2].SetFloat("swirlForceStrength", Mathf.Max(0.5f, a / 2));
        effects[2].SetFloat("swirlForceStrengthB", Mathf.Max(0.5f, b / 2));



    }

    private void Velocity(int avatarIndex, GameObject rig)
    {
        velocity[avatarIndex] = ((rig.transform.position - lastPosition[avatarIndex]) / Time.deltaTime).magnitude;
        lastPosition[avatarIndex] = rig.transform.position;
        //Debug.Log("Velocity=" + velocity[avatarIndex]);
    }


    private float VelocityFloat(int avatarIndex, GameObject rig)
    {
        float velo;
        velo = ((rig.transform.position - lastPosition2[avatarIndex]) / Time.deltaTime).magnitude;
        lastPosition2[avatarIndex] = rig.transform.position;
        return velo; 
        //Debug.Log("Velocity=" + velocity[avatarIndex]);
    }

    private float Distance(GameObject a, GameObject b)
    {
        float distance = Vector3.Distance(a.transform.position, b.transform.position);
        return distance;
    }
}
