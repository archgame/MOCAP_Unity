using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetSeparateGridColor : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject orgGrid;
    private MeshRenderer orgMesh;

    void Start()
    {
        orgMesh = orgGrid.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Color centerColor;
        centerColor = orgMesh.material.GetColor("_centerColor");
        gameObject.GetComponent<MeshRenderer>().material.SetColor("_centerColor", centerColor);
    }
}
