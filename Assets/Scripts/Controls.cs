using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        //quit application
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Quit Application");
            Application.Quit();
        }
    }
}