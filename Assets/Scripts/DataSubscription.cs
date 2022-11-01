using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using System;
using Unity.Mathematics;

public class DataSubscription : MonoBehaviour
{
    public class Avatar
    {
        public int avatarIndex;
        public BodyRigs head, hip, leftHand, rightHand, leftFoot, rightFoot;
        public bool isJump, isSpin;
        public float jumpCount = 0, accuSpinAngle = 0;
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
        
        //declare indexer for easier assign
        public BodyRigs this[int index]
        {
            get {
                switch (index) {
                    case 0: return this.head;
                    case 1: return this.hip;
                    case 2: return this.leftHand;
                    case 3: return this.rightHand;
                    case 4: return this.leftFoot;
                    case 5: return this.rightFoot;
                    default: throw new IndexOutOfRangeException("Index");}
                }
        }
    }
    public class BodyRigs
    {
        public string name;
        public GameObject rig;
        public Vector3 initialFootHeight;
        public Vector3 position;
        public Vector3 lastPosition;
        public Vector3 velocity;
        public Vector3 lastRotation;
        public Vector3 rotation;
        public float speed;
        public Vector3 rotOmega;
        public float deltaYlastFrame;
        public float rotationSpeed;
        public BodyRigs(GameObject rig)
        {
            this.rig = rig;
            this.lastPosition = rig.transform.position;
            this.lastRotation = rig.transform.eulerAngles;
        }
    }


    //declare avatars, avatar[]= {head, hip, left hand, right hand, left foot, right foot}
    public GameObject[] avat0;
    public GameObject[] avat1;
    public Avatar avatar0;
    public Avatar avatar1;
    /*public BodyRigs[] avat0Rigs ;
    public BodyRigs[] avat1Rigs ;*/

    //declare vfx
    public VisualEffect[] effects;
    public GameObject[] grids;
    [SerializeField] private Vector2[] handDistV2;
    [SerializeField] private Material[] mtl;

    //declare distance parameter
    public float[] dist;

    public bool isAlienMorph;
    public float gridStretchTime;


    //declare character
    private GameObject chars;
    private CharacterManager charManager;

    // Start is called before the first frame update
    void Start()
    {
        isAlienMorph = false;
        gridStretchTime = 2f;
        //Construct  the Avatar Class 
        avatar0 = new Avatar(0, new BodyRigs(avat0[0]), new BodyRigs(avat0[1]), new BodyRigs(avat0[2]), new BodyRigs(avat0[3]), new BodyRigs(avat0[4]), new BodyRigs(avat0[5]));
        avatar1 = new Avatar(1, new BodyRigs(avat1[0]), new BodyRigs(avat1[1]), new BodyRigs(avat1[2]), new BodyRigs(avat1[3]), new BodyRigs(avat1[4]), new BodyRigs(avat1[5]));


        mtl[0] = grids[0].GetComponent<MeshRenderer>().material;
        mtl[1] = grids[1].GetComponent<MeshRenderer>().material;

        chars = GameObject.Find("_CHARACTERS");
        charManager = chars.GetComponent<CharacterManager>();


    }

    // Update is called once per frame
    void Update()
    {

        //velocity calc
        RigsVelocity(avatar0);
        RigsVelocity(avatar1);

        //rotation speed calc
        RigsRotation(avatar0);
        RigsRotation(avatar1);

        //jumping defition
        isFootHigherThanCalf(avatar0);
        isFootHigherThanCalf(avatar1);

        //Spin def
        isSpinning(avatar0);
        isSpinning(avatar1);

        //TrailStar initial speed
        effects[3].SetVector3("VelocityA", avatar0.rightHand.velocity);
        effects[4].SetVector3("VelocityA", avatar1.rightHand.velocity);
        effects[5].SetVector3("VelocityA", avatar0.rightHand.velocity);
        effects[6].SetVector3("VelocityA", avatar1.rightHand.velocity);
        effects[3].SetVector3("_Increment", avatar0.rightHand.velocity * Time.deltaTime );
        effects[4].SetVector3("_Increment", avatar1.rightHand.velocity * Time.deltaTime);
        effects[5].SetVector3("_Increment", avatar0.rightHand.velocity * Time.deltaTime);
        effects[6].SetVector3("_Increment", avatar1.rightHand.velocity * Time.deltaTime);

        effects[0].SetVector4("BurstColor", charManager.colors[(charManager.Char0ColorIndex + 4) % 5]);
        effects[1].SetVector4("BurstColor", charManager.colors[(charManager.Char1ColorIndex + 4) % 5]);
        effects[3].SetVector4("_endColor", charManager.colors[(charManager.Char0ColorIndex + 4) % 5]);
        effects[4].SetVector4("_endColor", charManager.colors[(charManager.Char1ColorIndex + 4) % 5]);
        effects[5].SetVector4("_endColor", charManager.colors[(charManager.Char0ColorIndex + 4) % 5]);
        effects[6].SetVector4("_endColor", charManager.colors[(charManager.Char1ColorIndex + 4) % 5]);


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


        //set swirl strength to work with speed
        float a = math.min(avatar0.hip.rotationSpeed, 10000f);
        float b = math.min(avatar1.hip.rotationSpeed, 10000f);
        a = math.remap(100f, 10000f, 0.2f, 1.2f, a);
        b = math.remap(100f, 10000f, 0.2f, 1.2f, b);

        effects[2].SetFloat("swirlForceStrength", Mathf.Max(1.2f, a ));
        effects[2].SetFloat("swirlForceStrengthB", Mathf.Max(1.2f, b ));


        effects[2].SetFloat("attractForceStrength", Mathf.Max(1.2f, a));
        effects[2].SetFloat("attractForceStrengthB", Mathf.Max(1.2f, b));

        //set jump with size
        if (!isAlienMorph) {
            grids[0].GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_jumpCount0", avatar0.jumpCount);
            grids[0].GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_jumpCount1", avatar1.jumpCount);
            if (Distance(avatar0.hip.rig, avatar1.hip.rig) <= 3f) {
                float spacing = Mathf.Repeat(Time.time, 0.9f) + 0.1f;
                grids[0].GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_spacing", spacing);
            } else {
            grids[0].GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_spacing", 1f);
        }
        } else if(isAlienMorph) {

            if (gridStretchTime >= 0f) {
                float spacing = Mathf.Repeat(Time.time, 0.9f) + 0.1f;
                grids[0].GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_spacing", spacing);
                gridStretchTime -= Time.deltaTime;
            }
            else {
                grids[0].GetComponent<MeshRenderer>().sharedMaterial.SetVector("_targetALocation", Vector3.zero);
                grids[0].GetComponent<MeshRenderer>().sharedMaterial.SetVector("_targetBLocation", Vector3.zero);
                grids[0].GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_jumpCount0", 1f);
                grids[0].GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_jumpCount1", 1f);
                grids[0].GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_spacing", 1f);

            }

        }
       

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
            bodyRig.position = bodyRig.rig.transform.position;
            bodyRig.speed = bodyRig.velocity.magnitude;
            //Debug.Log("Speed for avatar" + avat.avatarIndex + "'s " + bodyRig.name + " is " + bodyRig.speed);
        }
    }

    private void RigsRotation(Avatar avat)
    {
        //calc speed and write to class per Avatar
        RotateCalc(avat.head);
        RotateCalc(avat.hip);

        RotateCalc(avat.leftHand);
        RotateCalc(avat.rightHand);
        RotateCalc(avat.leftFoot);
        RotateCalc(avat.rightFoot);
        //Debug.Log("Rotationspeed for Avat" + avat.avatarIndex + "is " + avat.hip.rotationSpeed);
        //method to calc each rig's speed and write into its class
        void RotateCalc(BodyRigs bodyRig)
        {
            bodyRig.rotation = bodyRig.rig.transform.eulerAngles ;
            bodyRig.rotOmega = (bodyRig.rotation - bodyRig.lastRotation) / Time.deltaTime;
            bodyRig.deltaYlastFrame = Mathf.Abs(bodyRig.rotation.y - bodyRig.lastRotation.y);
            //Debug.Log("Delta Y is " + bodyRig.deltaYlastFrame);
            if (bodyRig.deltaYlastFrame >=200f) {
                //Debug.Log("  Delta Y is " + bodyRig.deltaYlastFrame);
                bodyRig.deltaYlastFrame = 360 - Mathf.Max(bodyRig.rotation.y, bodyRig.lastRotation.y) + Mathf.Min(bodyRig.rotation.y, bodyRig.lastRotation.y);
                /*Debug.Log("Current Y is " + bodyRig.rotation.y
                    + "  Last Y is " + bodyRig.lastRotation.y
                    + "  Delta Y is " + bodyRig.deltaYlastFrame
                    + "  Delta Time is " + Time.deltaTime);*/
            }
            bodyRig.lastRotation = bodyRig.rig.transform.eulerAngles;
            bodyRig.rotationSpeed = bodyRig.deltaYlastFrame / Time.deltaTime; 


            /*float y1 = bodyRig.rig.transform.eulerAngles.y;
            float y2 = bodyRig.lastRotation.y;
            float deltaY;
            if ( y1 * y2 < 0 && Mathf.Abs(y1 - y2) >= 270) {
                deltaY = 360 - Mathf.Abs(y1 - y2);
            } else { deltaY = Mathf.Abs(y1 - y2); }

            bodyRig.rotationSpeed = deltaY / Time.deltaTime;
            bodyRig.lastRotation = bodyRig.rig.transform.eulerAngles;*/

        }
    }


    public float Distance(GameObject a, GameObject b)
    {
        float distance = Vector3.Distance(a.transform.position, b.transform.position);
        return distance;
    }

    
    private void isFootHigherThanCalf(Avatar avat)
    {
        if (avat.leftFoot.rig.transform.position.y >= 0.15f && avat.rightFoot.rig.transform.position.y >= 0.15f) {
            avat.isJump = true;
        }
        else avat.isJump = false;
        //Debug.Log("Currnt Height is " + avat.leftFoot.rig.transform.position.y);

    }

    private void isSpinning(Avatar avat)
    {
        if (Mathf.Abs(avat[1].rotationSpeed) > 0.5f) {
            avat.isSpin = true;
            //if(avat.accuSpinAngle == 0f) { Debug.Log(avat.avatarIndex + "Spin Star!"); }
            avat.accuSpinAngle += avat[1].deltaYlastFrame;
        }
        else { 
            avat.isSpin = false;
            //if(avat.accuSpinAngle> 200f) { Debug.Log("Aborted, " + avat.avatarIndex + "'s accu angle is " + avat.accuSpinAngle); }
            //Debug.Log("Aborted, " + avat.avatarIndex + "'s accu angle is " + avat.accuSpinAngle);
            avat.accuSpinAngle = 0f; }
        

    }

    public void AlienMorph(bool bo) { isAlienMorph = bo; }

}
