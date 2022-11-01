using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera UICamera;
    public Camera Display2Camera;
    public Camera Display3Camera;

    // Start is called before the first frame update
    private void Start()
    {
        if (UICamera == null || Display2Camera == null || Display3Camera == null) { return; }

        Debug.Log("Display Count: " + Display.displays.Length);
        for (int i = 0; i < Display.displays.Length; i++)
        {
            var disp = Display.displays[i];
            disp.Activate(disp.systemWidth, disp.systemHeight, 60);
            if (i == 0) { UICamera.targetDisplay = 0; Debug.Log("Camera Display 0"); }
            if (i == 1) { Display2Camera.targetDisplay = 1; Debug.Log("Camera Display 1"); }
            if (i == 2) { Display3Camera.targetDisplay = 2; Debug.Log("Camera Display 2"); }
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }
}