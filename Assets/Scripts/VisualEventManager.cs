using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VisualEventManager : MonoBehaviour
{
    //delcare events
    public static event Action VelocityThreshold;
    public static event Action KeyPress;

    //refer to methods in DataSubscription class
    private DataSubscription data;

    //delcare event parameter
        //declare velocity event parameter
    public GameObject rigObj;
    public float threshold;
    [SerializeField] private float rigVelocity;



    private void Start()
    {

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            VelocityThreshold?.Invoke();
            //Debug.Log("Event Invoked");
        }


    }


    private bool isVelocityOver(int avatarIndex)
    {
        rigVelocity = data.Velocity(avatarIndex, rigObj);
        return (rigVelocity >= threshold);
    }
}
