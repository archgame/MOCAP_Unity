using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VisualEventManager : MonoBehaviour
{
    //delcare events
    public static event Action VelocityThreshold0;
    public static event Action VelocityThreshold1;

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




    private void Start()
    {

    }
    private void Update()
    {
        data = dataSource.GetComponent<DataSubscription>();

        //acquire velocity data from DataSubscriber
        rigVelocity[0] = data.velocity[0];
        rigVelocity[1] = data.velocity[1];



        //timer to control interval
        timer1 += Time.deltaTime;
        timer2 += Time.deltaTime;

        if (rigVelocity[0] >= threshold && timer1 >= interval)
        {
            VelocityThreshold0?.Invoke();
            //Debug.Log("Event Invoked, Speed = " + rigVelocity[0] + "interval = " + timer1);
            timer1 = 0;
        }

        if (rigVelocity[1] >= threshold && timer2 >= interval)
        {
            VelocityThreshold1?.Invoke();
           // Debug.Log("Event Invoked, Speed = " + rigVelocity[1] + "interval = " + timer2);
            timer2 = 0;
        }


    }



}
