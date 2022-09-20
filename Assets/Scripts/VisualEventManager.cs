using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.VFX;

public class VisualEventManager : MonoBehaviour
{
    //delcare events
    public VisualEffect[] effects;

    //refer to methods in DataSubscription class
    public GameObject dataSource;
    private DataSubscription data;
    [SerializeField] private float interval;
    private float timer1;
    private float timer2;

    //delcare speed trigger parameter
    public int rigNumber;
    public float threshold;
    private float[] rigSpeed = new float[] {0,0};

    //decalre jump action parameter
    [SerializeField]private float jumpTimeThreshold = 0.3f;
    private float jumpTimer0 = 0f;
    private float jumpTimer1 = 0f;

    //declare grids


    //declare spin trigger parameter
    private float spinTimer0 = 0f;
    private float spinTimer1 = 0f;


    private void Start()
    {

    }
    private void Update()
    {
        data = dataSource.GetComponent<DataSubscription>();

        //acquire velocity data from DataSubscriber
        rigSpeed[0] = RigNumberToRig(rigNumber, data.avatar0).speed;
        rigSpeed[1] = RigNumberToRig(rigNumber, data.avatar1).speed;

        //timer to control interval
        timer1 += Time.deltaTime;
        timer2 += Time.deltaTime;

        
        //jump
        if(data.avatar0.isJump == true) {
            jumpTimer0 += Time.deltaTime;
            //Debug.Log("Jumptime1 is " + jumpTimer0);
            if (jumpTimer0 > jumpTimeThreshold) {
                data.avatar0.jumpCount += 1;
                //Debug.Log("Jumped " + data.avatar0.jumpCount + " times");
                //passEvent(effects[0], "Wave");
                data.avatar0.isJump = false;
                jumpTimer0 = 0;
            }

        }
        if (data.avatar1.isJump == true) {
            jumpTimer1 += Time.deltaTime;
            if (jumpTimer1 > jumpTimeThreshold) {
                data.avatar1.jumpCount += 1;
                //passEvent(effects[0], "Wave");
                data.avatar0.isJump = false;
                jumpTimer1 = 0;
            }

        }
        

        //spin




        if (rigSpeed[0] >= threshold && timer1 >= interval)
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
        }


    }

    private void passEvent(VisualEffect vfx, string eventName)
    {
        vfx.SendEvent(eventName);
        //Debug.Log("Event " + eventName + " sent to " + vfx.name);
    }
    private  DataSubscription.BodyRigs RigNumberToRig(int number, DataSubscription.Avatar avat)
    {
        if (number == 0) { return avat.head; }
        if (number == 1) { return avat.hip; }
        if (number == 2) { return avat.leftHand; }
        if (number == 3) { return avat.rightHand; }
        if (number == 4) { return avat.leftFoot; }
        if (number == 5) { return avat.rightFoot; }
        else return null;

    }


}
