using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartSystControl : MonoBehaviour
{

    public bool emissionsEnabled = true;
    public float maxEmissionRate = 5.0f;
    public float maxMovement = 1.0f;
    public float amtMovement;

    ParticleSystem ps;

    Vector3 currPosition;
    Vector3 prevPosition;

    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        prevPosition = currPosition;
        currPosition = transform.parent.position;

        amtMovement = Vector3.Distance(currPosition, prevPosition);

        float emissionRate = map (amtMovement, 0, maxMovement, 0, maxEmissionRate);

        var emission = ps.emission;

        if(emissionsEnabled){
            emission.enabled = true;
        } else {
            emission.enabled = false;
        }

        emission.rateOverTime = emissionRate;

    }

    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s-a1)*(b2-b1)/(a2-a1);
    }
}
