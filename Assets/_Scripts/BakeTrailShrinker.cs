using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BakeTrailShrinker : MonoBehaviour
{
    public float _shrinkDelay = 60;
    public float _shrinkSpeed = 2;

    private bool _doShrink;

    // Start is called before the first frame update
    void Start()
    {
        GameObject controls = GameObject.Find("Controls");
        //_shrinkDelay = controls.GetComponent<Controls>().bakedTrailLifeTime;
        //_shrinkSpeed = controls.GetComponent<Controls>().bakedTrailShrinkSpeed;


        //Waits for an amount of time defined by _shrinkDelay before running the doDrinkSet method
        Invoke("doDrinkSet", _shrinkDelay);
    }

    
    void doDrinkSet()
    {
        //Sets _doShrink to true
        _doShrink = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Exicutes if _doShrink is equal to true
        if (_doShrink == true)
        {
            //Shrinks the games object
            transform.localScale = Vector3.Lerp(transform.localScale, transform.localScale / 2, Time.deltaTime * _shrinkSpeed);

            //Destroys thhe game object if it scale is less than 0.1
            if (transform.localScale.x <= 0.1f)
            {
                Destroy(this.gameObject);
            }
        }
           
    }
}
