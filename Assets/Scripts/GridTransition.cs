using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTransition : MonoBehaviour
{
    public bool transition;
    private Material gridMtl;
    public float lerp;
    public float duration;

    // Start is called before the first frame update
    void Start()
    {
        gridMtl = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (transition)
        {
            if (lerp <= duration)
            { lerp += Time.deltaTime; }
            gridMtl.SetFloat("_materialTransition", lerp/duration);
        }

    }
}
