using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarPulse : MonoBehaviour
{
    public float maxScale;
    public Transform trans;
    // Start is called before the first frame update
    void Start()
    {
        //gameObject.transform.parent = trans;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.parent = trans;
        gameObject.transform.position = trans.position;
        gameObject.transform.localScale = new Vector3 (0.2f,0.2f,0.2f) * (Mathf.Repeat(3f* Time.time, maxScale) + maxScale); 
    }
}
