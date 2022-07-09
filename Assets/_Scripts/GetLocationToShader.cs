using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetLocationToShader : MonoBehaviour
{
    private Material mtl;
    public GameObject Anchor;
    // Start is called before the first frame update
    void Start()
    {
        mtl = GetComponent<MeshRenderer>().sharedMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 loc;
        loc = Anchor.transform.position;
        CenterGrid(loc);
    }

    private void CenterGrid(Vector3 center)
    {
        mtl.SetVector("_targetLocation", center);
    }
}
