using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class DataSubscription : MonoBehaviour
{
    public class Avatar
    {
        public int avatarIndex;
        public BodyRigs head, hip, leftHand, rightHand, leftFoot, rightFoot; 
        public Avatar(int avatarIndex, BodyRigs head, BodyRigs hip, BodyRigs leftHand, BodyRigs rightHand, BodyRigs leftFoot, BodyRigs rightFoot)
        {
            this.avatarIndex = avatarIndex;
            this.head = head;
            this.head.name = "Head";
            this.hip = hip;
            this.hip.name = "Hip";
            this.leftHand = leftHand;
            this.leftHand.name = "Left Hand";
            this.rightHand = rightHand;
            this.rightHand.name = "Right Hand";
            this.leftFoot = leftFoot;
            this.leftFoot.name = "Left Foot";
            this.rightFoot = rightFoot;
            this.rightFoot.name = "Right Foot";
        }
    }
    public class BodyRigs
    {
        public string name;
        public GameObject rig;
        public Vector3 position;
        public Vector3 lastPosition;
        public Vector3 velocity;
        public float speed;
        public BodyRigs(GameObject rig)
        {
            this.rig = rig;
            this.lastPosition = rig.transform.position;
        }
    }


    //declare avatars, avatar[]= {head, hip, left hand, right hand, left foot, right foot}
    public GameObject[] avat0;
    public GameObject[] avat1;
    private Avatar avatar0;
    private Avatar avatar1;
    private BodyRigs[] avat0Rigs;
    private BodyRigs[] avat1Rigs;
    public int rigNumber;
    public float[] rigSpeed = new float[] {0f,0f};

    //declare vfx
    public VisualEffect[] effects;
    public GameObject[] grids;
    [SerializeField] private Vector2[] handDistV2;
    [SerializeField] private Material[] mtl;

    //declare distance parameter
    public float[] dist;


    // Start is called before the first frame update
    void Start()
    {
        //Construct  the Avatar Class 
        avatar0 = new Avatar(0, new BodyRigs(avat0[0]), new BodyRigs(avat0[1]), new BodyRigs(avat0[2]), new BodyRigs(avat0[3]), new BodyRigs(avat0[4]), new BodyRigs(avat0[5]));
        avatar1 = new Avatar(1, new BodyRigs(avat1[0]), new BodyRigs(avat1[1]), new BodyRigs(avat1[2]), new BodyRigs(avat1[3]), new BodyRigs(avat1[4]), new BodyRigs(avat1[5]));
        avat0Rigs = new BodyRigs[] { avatar0.head, avatar0.hip, avatar0.leftHand, avatar0.rightHand, avatar0.leftFoot, avatar0.rightFoot };
        avat1Rigs = new BodyRigs[] { avatar1.head, avatar1.hip, avatar1.leftHand, avatar1.rightHand, avatar1.leftFoot, avatar1.rightFoot };
        rigSpeed[0] = avat0Rigs[rigNumber].speed;
        rigSpeed[1] = avat1Rigs[rigNumber].speed;


        mtl[0] = grids[0].GetComponent<MeshRenderer>().material;
        mtl[1] = grids[1].GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        //velocity calc
        RigsVelocity(avatar0);
        RigsVelocity(avatar1);

        //distance calc 
        dist[0] = Distance(avat0[2], avat0[3]);
        dist[1] = Distance(avat0[2], avat1[2]);

        //subscribtions
        effects[0].SetFloat("handHipDist", dist[0]);
        effects[1].SetFloat("handHipDist", dist[1]);
        mtl[0].SetFloat("_handDistAf", dist[0]);
        mtl[0].SetFloat("_handDistBf", dist[1]);
        handDistV2[0] = new Vector2(dist[0], dist[0]);
        handDistV2[1] = new Vector2(dist[1], dist[1]);
        mtl[1].SetVector("_handDistA", handDistV2[0]);
        mtl[1].SetVector("_handDistB", handDistV2[1]);


        /* set swirl strength to work with speed
        effects[2].SetFloat("attractForceStrength", Mathf.Max(0.5f,a/2));
        effects[2].SetFloat("attractForceStrengthB", Mathf.Max(0.5f, b / 2));
        effects[2].SetFloat("swirlForceStrength", Mathf.Max(0.5f, a / 2));
        effects[2].SetFloat("swirlForceStrengthB", Mathf.Max(0.5f, b / 2));
        */


    }


    private void RigsVelocity(Avatar avat)
    {
        //calc speed and write to class per Avatar
        VelocityCalc(avat.head);
        VelocityCalc(avat.hip);
        VelocityCalc(avat.leftHand);
        VelocityCalc(avat.rightHand);
        VelocityCalc(avat.leftFoot);
        VelocityCalc(avat.rightFoot);
        //method to calc each rig's speed and write into its class
        void VelocityCalc(BodyRigs bodyRig){
            bodyRig.velocity = (bodyRig.rig.transform.position - bodyRig.lastPosition) / Time.deltaTime;
            bodyRig.lastPosition = bodyRig.rig.transform.position;
            bodyRig.speed = bodyRig.velocity.magnitude;
            Debug.Log("Speed for avatar" + avat.avatarIndex + "'s " + bodyRig.name + " is " + bodyRig.speed);
        }
    }



    private float Distance(GameObject a, GameObject b)
    {
        float distance = Vector3.Distance(a.transform.position, b.transform.position);
        return distance;
    }
}
