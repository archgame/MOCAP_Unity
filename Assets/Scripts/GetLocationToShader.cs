using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GetLocationToShader : MonoBehaviour
{
    private Material mtl;
    public GameObject[] Anchors;
    public string[] paramNames;

    public bool isAlienMorph;


    // Start is called before the first frame update
    void Start()
    {
        mtl = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlienMorph) {
            for (int i = 0; i < Anchors.Length; i++) {
                if (Anchors[i] != null && paramNames[i] != null) { mtl.SetVector(paramNames[i], Anchors[i].transform.position); }
                else Debug.Log("Anchor or Param " + i + "not assigned");
            }
        } else if(isAlienMorph) { return; }

    }

    public void AlienMorph(bool bo) { isAlienMorph = bo; }

}
