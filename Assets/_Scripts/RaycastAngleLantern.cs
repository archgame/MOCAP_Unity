using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastAngleLantern : MonoBehaviour
{
    public GameObject avatar;
    public float angle;
    
    Ray centreLight;
    Ray pointAtAvatar;

    // Start is called before the first frame update
    void Start()
    {
        // centreLight = new Ray(transform.position, transform.TransformDirection(Vector3.forward));
        // pointAtAvatar = new Ray(transform.position, (avatar.transform.position - transform.position));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        centreLight.origin = transform.position;
        centreLight.direction = transform.TransformDirection(Vector3.forward);
        Debug.DrawRay(centreLight.origin, centreLight.direction * 20,  Color.blue);

        pointAtAvatar.origin = transform.position;
        pointAtAvatar.direction = avatar.transform.position - transform.position;
        Debug.DrawRay(pointAtAvatar.origin, pointAtAvatar.direction * 20,  Color.red);

        angle = Vector3.Angle((avatar.transform.position - transform.position), centreLight.direction);
    }
}
