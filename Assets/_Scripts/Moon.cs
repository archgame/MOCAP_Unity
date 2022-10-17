using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moon : MonoBehaviour
{
    public GameObject Actor0;
    public GameObject Actor1;

    public float MoonRelativeScale = 1.0f;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        //toggle moon renderer on and off
        if (Input.GetKeyDown(KeyCode.M))
        {
            var renderer = GetComponent<MeshRenderer>();
            renderer.enabled = !renderer.enabled;
        }

        //calculate moon position
        Vector3 actor0 = Actor0.transform.position;
        Vector3 actor1 = Actor1.transform.position;
        Vector3 newPosition = (actor0 + actor1) / 2.00f;
        Debug.Log(newPosition);

        //calculate moon scale
        float dist = Vector3.Distance(actor1, actor0);
        dist *= MoonRelativeScale;
        Vector3 newScale = new Vector3(dist, dist, dist);

        //gradually update position and scale
        float rate = 2;
        Vector3 currentPosition = transform.position;
        Vector3 updatePosition = Vector3.Lerp(currentPosition, newPosition, rate * Time.deltaTime);
        transform.position = updatePosition;
        Vector3 currentScale = transform.localScale;
        Vector3 updateScale = Vector3.Lerp(currentScale, newScale, rate * Time.deltaTime);
        transform.localScale = updateScale;
    }
}