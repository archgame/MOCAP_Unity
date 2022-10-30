using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera cam;

    // Start is called before the first frame update
    private void Start()
    {
        GameObject camera = GameObject.Find("cam01");
        cam = camera.GetComponent<Camera>();
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (cam == null) { return; }
        //transform.LookAt(cam.transform.position);
        //transform.forward = cam.transform.forward;
        transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
        //transform.rotation = Quaternion.Euler(0f,transform.rotation.eulerAngles.y,0f);
    }
}