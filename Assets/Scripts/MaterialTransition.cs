using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialTransition : MonoBehaviour
{
    public Material mtl1;
    public Material mtl2;
    private MeshRenderer rend;
    private Material mtl;
    public float lerp = 0f;
    public float duration = 3f;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<MeshRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        /*if (lerp < duration)
        { lerp += Time.deltaTime; }*/
        mtl.Lerp(mtl1, mtl2, lerp / duration);
        rend.material = mtl;
    }
}
