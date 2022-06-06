using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    public float TrailRenderTime = 2.0f;

    public TrailRenderer[] TrailRenderers;

    public SkinnedMeshRenderer BodyRenderer;

    // Start is called before the first frame update
    private void Start()
    {
        TrailRenderers = FindObjectsOfType<TrailRenderer>();
    }

    private float _timeIncrement = 0.01f;

    // Update is called once per frame
    private void Update()
    {
        //quit application
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Quit Application");
            Application.Quit();
        }

        //increase trail render time
        float tempTime = TrailRenderTime;
        if (Input.GetKey(KeyCode.UpArrow)) { tempTime += _timeIncrement; Debug.Log("Up"); }
        if (Input.GetKey(KeyCode.DownArrow)) { tempTime -= _timeIncrement; Debug.Log("Down"); }
        if (TrailRenderTime > 5) { tempTime = 5; }
        if (TrailRenderTime < 0) { tempTime = 0; }
        if (tempTime != TrailRenderTime)
        {
            TrailRenderTime = tempTime;
            UpdateTrailRenderers();
        }

        //toggle body renderer on and off
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (BodyRenderer != null)
            {
                BodyRenderer.enabled = !BodyRenderer.enabled;
            }
        }
    }

    private void UpdateTrailRenderers()
    {
        foreach (TrailRenderer trail in TrailRenderers)
        {
            trail.time = TrailRenderTime;
        }
    }
}