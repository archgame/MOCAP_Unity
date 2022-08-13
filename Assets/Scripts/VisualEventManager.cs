using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VisualEventManager : MonoBehaviour
{
    //delcare events
    public static event Action VelocityThreshold;

    //refer to methods in DataSubscription class
    public GameObject dataSource;
    private DataSubscription data;
    [SerializeField] private float interval;
    private float timer;

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
        timer += Time.deltaTime;

        if (rigVelocity[0] >= threshold && timer >= interval)
        {
            VelocityThreshold?.Invoke();
            Debug.Log("Event Invoked, Speed = " + rigVelocity[0] + "interval = " + timer);
            timer = 0;
        }


    }



}
