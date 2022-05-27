using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceToLight : MonoBehaviour
{

    public Transform cone;
    public float dist = 0;

    private Rigidbody coneRb;

    // Start is called before the first frame update
    void Start()
    {
        coneRb = cone.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 avatarPos = transform.position;
        Vector3 closestOnCone = coneRb.ClosestPointOnBounds(avatarPos);

        dist = Vector3.Distance(avatarPos, closestOnCone);
    }
}
