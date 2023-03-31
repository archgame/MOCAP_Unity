using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.VFX;
using Unity.Mathematics;
using static AvatarsData;
using static CharacterManager;


public class VisualEventManager : MonoBehaviour
{
    //delcare events
    public VisualEffect[] effects;
    public GameObject[] grids;

    //refer to methods in DataSubscription class
    [SerializeField] private float interval;
    private float timer1;
    private float timer2;

    //delcare speed trigger parameter
    public int rigNumber;
    public float threshold;
    private float[] rigSpeed = new float[] { 0, 0, 0 };

    //decalre jump action parameter
    [SerializeField] private float jumpTimeThreshold = 0.3f;
    private float jumpTimer0 = 0f;
    private float jumpTimer1 = 0f;
    private float jumpTimer2 = 0f;
    private float jumpTimer3 = 0f;

    //declare grids
    public GameObject grid;



    //declare spin trigger parameter
    public float spinTimer0 = 0f;
    private float spinTimer1 = 0f;
    private float spinTimerThreshold = 1.5f;
    public float spinThresholdAngle;

    //clap threshold
    private float clapThreshold = 0.2f;




    private void Start()
    {

    }
    private void Update()
    {
        //acquire velocity data from DataSubscriber
        rigSpeed[0] = RigNumberToRig(rigNumber, avatar0).speed;
        rigSpeed[1] = RigNumberToRig(rigNumber, avatar1).speed;
        rigSpeed[2] = RigNumberToRig(rigNumber, avatar2).speed;

        //timer to control interval
        timer1 += Time.deltaTime;
        timer2 += Time.deltaTime;


        //jump
        JumpActions(avatar0, ref jumpTimer0, effects[0]);
        JumpActions(avatar1, ref jumpTimer1, effects[1]);
        JumpActions(avatar2, ref jumpTimer2, effects[2]);





        //Clap and high five
        float[,] dist = new float[2, 3];
        dist[0, 0] = Distance(avatar0[2].rig, avatar0[3].rig);
        dist[1, 0] = Distance(avatar1[2].rig, avatar0[3].rig);

        dist[0, 1] = Distance(avatar0[2].rig, avatar1[2].rig);
        dist[0, 2] = Distance(avatar0[2].rig, avatar1[3].rig);

        dist[1, 1] = Distance(avatar0[3].rig, avatar1[2].rig);
        dist[1, 2] = Distance(avatar0[3].rig, avatar1[3].rig);

        //clap
        if (dist[0, 0] <= clapThreshold) {; }
        if (dist[1, 0] <= clapThreshold) {; }

        //high five
        if (dist[0, 1] <= clapThreshold || dist[0, 2] <= clapThreshold || dist[1, 1] <= clapThreshold || dist[1, 2] <= clapThreshold) {; }

        //spin
        if (AvatarsData.avatar0.isSpin == true) {
            //spinTimer0 += Time.deltaTime;
            if (avatar0.accuSpinAngle >= spinThresholdAngle) {
                Debug.Log("0 Spinned!!!!");
                //AvatarsData.avatar0.isSpin = false;
                avatar0.accuSpinAngle = 0;
            }
        }

        effects[4].SetVector3("VelocityA", avatar0.rightHand.velocity);
        effects[5].SetVector3("VelocityA", avatar1.rightHand.velocity);
        effects[6].SetVector3("VelocityA", avatar2.rightHand.velocity);
        effects[7].SetVector3("VelocityA", avatar0.rightHand.velocity);
        effects[8].SetVector3("VelocityA", avatar1.rightHand.velocity);
        effects[9].SetVector3("VelocityA", avatar2.rightHand.velocity);
        //effects[3].SetVector3("_Increment", avatar0.rightHand.velocity * Time.deltaTime);
        //effects[4].SetVector3("_Increment", avatar1.rightHand.velocity * Time.deltaTime);
        //effects[5].SetVector3("_Increment", avatar0.rightHand.velocity * Time.deltaTime);
        //effects[6].SetVector3("_Increment", avatar1.rightHand.velocity * Time.deltaTime);



        //set swirl strength to work with speed
        float a = math.min(avatar0.hip.rotationSpeed, 500f);
        float b = math.min(avatar1.hip.rotationSpeed, 500f);
        float c = math.min(avatar2.hip.rotationSpeed, 500f);
        a = math.remap(0f, 500f, 0.8f, 6f, a);
        b = math.remap(0f, 500f, 0.8f, 6f, b);
        c = math.remap(0f, 500f, 0.8f, 6f, c);

        effects[3].SetFloat("swirlForceStrength", Mathf.Max(0.8f, a));
        effects[3].SetFloat("swirlForceStrengthB", Mathf.Max(0.8f, b));
        effects[3].SetFloat("swirlForceStrengthC", Mathf.Max(0.8f, c));

        effects[3].SetFloat("attractForceStrength", Mathf.Max(0.8f, a));
        effects[3].SetFloat("attractForceStrengthB", Mathf.Max(0.8f, b));
        effects[3].SetFloat("attractForceStrengthC", Mathf.Max(0.8f, c));

        //Sync color with character
        effects[0].SetVector4("BurstColor", CharMGM.colors[(CharMGM.Char0ColorIndex + 4) % 5]);
        effects[1].SetVector4("BurstColor", CharMGM.colors[(CharMGM.Char1ColorIndex + 4) % 5]);
        effects[2].SetVector4("BurstColor", CharMGM.colors[(CharMGM.Char2ColorIndex + 4) % 5]);
        effects[4].SetVector4("_endColor", CharMGM.colors[(CharMGM.Char0ColorIndex + 4) % 5]);
        effects[5].SetVector4("_endColor", CharMGM.colors[(CharMGM.Char1ColorIndex + 4) % 5]);
        effects[6].SetVector4("_endColor", CharMGM.colors[(CharMGM.Char2ColorIndex + 4) % 5]);
        effects[7].SetVector4("_endColor", CharMGM.colors[(CharMGM.Char0ColorIndex + 4) % 5]);
        effects[8].SetVector4("_endColor", CharMGM.colors[(CharMGM.Char1ColorIndex + 4) % 5]);
        effects[9].SetVector4("_endColor", CharMGM.colors[(CharMGM.Char2ColorIndex + 4) % 5]);


        //SetGrid Color
        if (grids[2].activeInHierarchy && grids[3].activeInHierarchy) {
            grids[2].GetComponent<MeshRenderer>().material.SetColor("_centerColor", CharMGM.colors[(CharMGM.Char0ColorIndex + 4) % 5]);
            grids[3].GetComponent<MeshRenderer>().material.SetColor("_centerColor", CharMGM.colors[(CharMGM.Char1ColorIndex + 4) % 5]);
        }

        if (grids[4].activeInHierarchy && grids[5].activeInHierarchy) {
            grids[4].GetComponent<MeshRenderer>().material.SetColor("_centerColor", CharMGM.colors[(CharMGM.Char0ColorIndex + 4) % 5]);
            grids[5].GetComponent<MeshRenderer>().material.SetColor("_centerColor", CharMGM.colors[(CharMGM.Char1ColorIndex + 4) % 5]);
        }
        /*if (rigSpeed[0] >= threshold && timer1 >= interval)
        {
            passEvent(effects[0], "Wave");
            //Debug.Log("Event Invoked, Speed = " + rigVelocity[0] + "interval = " + timer1);
            timer1 = 0;
        }

        if (rigSpeed[1] >= threshold && timer2 >= interval)
        {
            passEvent(effects[1], "Wave");
            // Debug.Log("Event Invoked, Speed = " + rigVelocity[1] + "interval = " + timer2);
            timer2 = 0;
        }*/



    }

    private void passEvent(VisualEffect vfx, string eventName)
    {
        vfx.SendEvent(eventName);
        //Debug.Log("Event " + eventName + " sent to " + vfx.name);
    }
    private AvatarsData.BodyRigs RigNumberToRig(int number, AvatarsData.Avatar avat)
    {
        if (number == 0) { return avat.head; }
        if (number == 1) { return avat.hip; }
        if (number == 2) { return avat.leftHand; }
        if (number == 3) { return avat.rightHand; }
        if (number == 4) { return avat.leftFoot; }
        if (number == 5) { return avat.rightFoot; }
        else return null;

    }

    private void JumpActions(AvatarsData.Avatar avatar, ref float jumpTimer, VisualEffect effect)
    {
        if (avatar.isJump) {
            if (grid.activeInHierarchy) {
                jumpTimer += Time.deltaTime;

                if (jumpTimer > jumpTimeThreshold) {
                    avatar.jumpCount += 1;
                    avatar.isJump = false;
                    jumpTimer = 0f;
                }
            }

            if (effect.isActiveAndEnabled) {
                jumpTimer += Time.deltaTime;

                if (jumpTimer > jumpTimeThreshold) {
                    passEvent(effect, "Wave");
                    avatar.isJump = false;
                    jumpTimer = 0f;
                }
            }
        }

    }
}

