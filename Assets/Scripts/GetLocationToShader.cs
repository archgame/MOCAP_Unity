using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetLocationToShader : MonoBehaviour
{
    private Material mtl;
    public GameObject Anchor0;
    // Start is called before the first frame update
    void Start()
    {
        mtl = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 loc1;
        loc1 = Anchor0.transform.position;
        CenterGrid(loc1);
    }

    private void CenterGrid(Vector3 center1)
    {
        mtl.SetVector("_targetALocation", center1);
    }
}
