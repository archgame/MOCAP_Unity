using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTopAnchor : MonoBehaviour
{
    GameObject host;
    public GameObject hip0;
    public GameObject hip1;
    public GameObject hip2;
    public Camera cam;

    public bool auto;
    // Start is called before the first frame update
    void Start()
    {
        host = new GameObject();
    }

    // Update is called once per frame
    void Update()
    {
        host.transform.position = (hip0.transform.position + hip1.transform.position + hip2.transform.position) / 3f;
        Vector3 anchor = new Vector3(host.transform.position.x, cam.transform.position.y, host.transform.position.z);

        if (auto) {
            cam.transform.position = anchor;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9)) {
            cam.transform.position = anchor;
        }

        if (cam.gameObject.activeInHierarchy == true) {
            if (Input.GetKey(KeyCode.U)) { cam.transform.Translate(5 * Vector3.up * Time.deltaTime, Space.World); }
            if (Input.GetKey(KeyCode.O)) { cam.transform.Translate(5 * Vector3.down * Time.deltaTime, Space.World); }
        }
    }
    public void autoToggle(bool set)
    {
        auto = set;
    }
}
