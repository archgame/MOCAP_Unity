using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchColour : MonoBehaviour
{

    // public Color defaultColor = new Color(0.1f, 0.1f, 0.1f, 1.0f);
    // public Color colorWhenTouched = Color.cyan;

    public Material matDefault;
    public Material matTouched;


    Renderer rend;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer> ();
        //rend.material.color = defaultColor;
        rend.material = matDefault;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag != gameObject.tag) {
            //Debug.Log("Something touched me!");
            //rend.material.color = colorWhenTouched;
            rend.material = matTouched;
        }

    }
}
