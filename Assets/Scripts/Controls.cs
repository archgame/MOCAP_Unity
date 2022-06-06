using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    public float TrailRenderTime = 2.0f;

    public TrailRenderer[] TrailRenderers;

    public SkinnedMeshRenderer BodyRenderer;

    private GameObject parent;

    // Start is called before the first frame update
    private void Start()
    {
        //get all trail renderers
        TrailRenderers = FindObjectsOfType<TrailRenderer>();

        //create parent transform for bake trail renderers
        parent = new GameObject();
        parent.transform.parent = this.gameObject.transform;
        parent.name = "Parent";
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

        //bake trail renderer as mesh
        if (Input.GetKeyDown(KeyCode.Space))
        {
            BakeTrailRenderers();
        }
    }

    private void UpdateTrailRenderers()
    {
        foreach (TrailRenderer trail in TrailRenderers)
        {
            trail.time = TrailRenderTime;
        }
    }

    private void BakeTrailRenderers()
    {
        //delete any existing children
        foreach (Transform child in parent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        //bake trail renderer and add to parent
        foreach (TrailRenderer trail in TrailRenderers)
        {
            //bake mesh
            Mesh bakedMesh = new Mesh();
            trail.BakeMesh(bakedMesh);

            // Recalcultate the bounding volume of the mesh from the vertices.
            bakedMesh.RecalculateBounds();
            //Debug.Log("Baked mesh bounds: " + bakedMesh.bounds.ToString());

            // Adding MeshCollider and assigning the bakedMesh.
            GameObject go = new GameObject();
            MeshFilter meshFilter = go.AddComponent<MeshFilter>();
            meshFilter.mesh = bakedMesh;
            MeshRenderer renderer = go.AddComponent<MeshRenderer>();
            renderer.material = trail.material;

            //add to parent
            go.transform.parent = parent.transform;
        }
    }
}