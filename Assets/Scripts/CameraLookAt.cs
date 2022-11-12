using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookAt : MonoBehaviour
{
    GameObject host;
    public GameObject hip0;
    public GameObject hip1;
    public Camera cam;
    public Camera buddyCam;

    public bool auto;
    // Start is called before the first frame update
    void Start()
    {
        host = new GameObject();
    }

    // Update is called once per frame
    void Update()
    {
        host.transform.position = (hip0.transform.position + hip1.transform.position) / 2f;
        host.transform.position = new Vector3(host.transform.position.x, 1.2f, host.transform.position.z);
        if (auto) {
            cam.transform.LookAt(host.transform);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0)) {
            cam.transform.LookAt(host.transform);
        }


        if (true) {
            /*if (Input.GetMouseButton(1)) {
                 cam.transform.Rotate(0f, Input.GetAxis("Mouse X"), 0f, Space.World);
                 cam.transform.Rotate(Input.GetAxis("Mouse Y"), 0f, 0f, Space.World);
                 Vector3 rotation = new Vector3(cam.transform.localEulerAngles.x, cam.transform.localEulerAngles.y, 0f);
                 cam.transform.rotation = Quaternion.Euler(rotation);
             }*/
            //if (Input.GetKey(KeyCode.Q)) { cam.transform.Translate(3 * Vector3.up * Time.deltaTime); }
            //if (Input.GetKey(KeyCode.E)) { cam.transform.Translate(3 * Vector3.down * Time.deltaTime); }
            if (buddyCam != null ){
                buddyCam.transform.position = cam.transform.position;
                buddyCam.transform.rotation = cam.transform.rotation;
            }
            if (Input.GetKey(KeyCode.I)) { cam.transform.Translate(3 * Vector3.forward * Time.deltaTime); }
            if (Input.GetKey(KeyCode.K)) { cam.transform.Translate(3 * Vector3.back * Time.deltaTime); }
            if (Input.GetKey(KeyCode.J)) { cam.transform.Translate(3 * Vector3.left * Time.deltaTime); }
            if (Input.GetKey(KeyCode.L)) { cam.transform.Translate(3 * Vector3.right * Time.deltaTime); }
            if (Input.GetMouseButton(1)) {
                cam.transform.Rotate(0f, Input.GetAxis("Mouse X"), 0f, Space.World);
                cam.transform.Rotate(Input.GetAxis("Mouse Y"), 0f, 0f, Space.World);
                Vector3 rotation = new Vector3(cam.transform.localEulerAngles.x, cam.transform.localEulerAngles.y, 0f);
                cam.transform.rotation = Quaternion.Euler(rotation);
            }

        }
    }
    
    public void autoToggle(bool set)
    {
        auto = set;
    }
}
