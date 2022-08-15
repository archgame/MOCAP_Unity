using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VisualEventManager : MonoBehaviour
{
    //delcare events
    public static event Action VelocityThreshold0;
    public static event Action VelocityThreshold1;
    public static event Action AccumalatedSpeed;

    //refer to methods in DataSubscription class
    public GameObject dataSource;
    private DataSubscription data;
    [SerializeField] private float interval;
    private float timer1;
    private float timer2;

    //delcare event parameter
        //declare velocity event parameter
    public float threshold;
    private float[] rigVelocity = new float[] {0,0};

    public float accumTime = 1; 
    private float accumuSpeed = 0.2f;
    private float timer3 = 0;
    private float timer4 = 0;



    private void Start()
    {

    }
    private void Update()
    {
        data = dataSource.GetComponent<DataSubscription>();

        //acquire velocity data from DataSubscriber
        rigVelocity[0] = data.velocity[0];
        rigVelocity[1] = data.velocity[1];

        
        if (data.velocity[0] > accumuSpeed) { accumuSpeed = data.velocity[0]; timer3 += Time.deltaTime; }
        else { Debug.Log("Acceleration peiod = " + timer3); timer3 = 0;  }
        if (timer3 >= accumTime || Input.GetKeyDown(KeyCode.S)) { AccumalatedSpeed?.Invoke(); timer3 = 0; accumuSpeed = 0;}

        if (data.velocity[1] > accumuSpeed) { accumuSpeed = data.velocity[1]; timer3 += Time.deltaTime; }
        else { Debug.Log("Acceleration peiod = " + timer3); timer3 = 0; }
        if (timer3 >= accumTime || Input.GetKeyDown(KeyCode.D)) { AccumalatedSpeed?.Invoke(); timer3 = 0; accumuSpeed = 0; }



        //timer to control interval
        timer1 += Time.deltaTime;
        timer2 += Time.deltaTime;

        if (rigVelocity[0] >= threshold && timer1 >= interval)
        {
            VelocityThreshold0?.Invoke();
            Debug.Log("Event Invoked, Speed = " + rigVelocity[0] + "interval = " + timer1);
            timer1 = 0;
        }

        if (rigVelocity[1] >= threshold && timer2 >= interval)
        {
            VelocityThreshold1?.Invoke();
            Debug.Log("Event Invoked, Speed = " + rigVelocity[1] + "interval = " + timer2);
            timer2 = 0;
        }


    }



}
