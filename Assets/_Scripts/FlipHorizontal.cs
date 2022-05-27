using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipHorizontal : MonoBehaviour
{
    public bool flipHorizontal;

    // Start is called before the first frame update
    void Start()
    {
        if (flipHorizontal)
        {

            transform.position = Vector3.Reflect(transform.position, Vector3.right);
            
            //new Vector3(transform.Scale.x * scale.x, transform.Scale.y * scale.y, transform.Scale.z * scale.z) ;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

