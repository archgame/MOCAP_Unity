using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    public float TrailRenderTime = 2.0f;

    public TrailRenderer[] TrailRenderers;

    public ParticleSystem[] ParticleSystems;

    public SkinnedMeshRenderer BodyRenderer;

    private GameObject parent;

    // Start is called before the first frame update
    private void Start()
    {
        //get all trail renderers
        TrailRenderers = FindObjectsOfType<TrailRenderer>();

        //get all particle systems
        ParticleSystems = FindObjectsOfType<ParticleSystem>();

        //create parent transform for bake trail renderers
        parent = new GameObject();
        parent.transform.parent = this.gameObject.transform;
        parent.name = "Parent";
        parent.transform.localPosition = Vector3.zero;
    }

    private float _timeIncrement = 0.01f;
    private bool _trailRendererEnabled = true;

    /*/
    Up/Down: Increase/Decrease Trace Length
    B: turn off body renderer
    Space: Bake Trail Renders
    X: Delete Baked Trail Renderers
    P: Toggle Particles/Trail Renderers
    //*/

    // Update is called once per frame
    private void Update()
    {
        //quit application
        if (Input.GetKeyDown(KeyCode.Escape)) { Debug.Log("Quit Application"); Application.Quit(); }

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
        if (Input.GetKeyDown(KeyCode.B) && BodyRenderer != null) { BodyRenderer.enabled = !BodyRenderer.enabled; }

        //bake trail renderer as mesh
        if (Input.GetKeyDown(KeyCode.Space)) { BakeTrailRenderers(); }

        //delete drawing
        if (Input.GetKeyDown(KeyCode.X)) { DeleteBakeTrailRenderers(); }

        //toggle trail renderer and particle systems
        if (Input.GetKeyDown(KeyCode.P)) { ToggleTrailRendererParticleSystem(); }

        //rotate parent
        float rot = 20;
        parent.transform.Rotate(this.transform.up * rot * Time.deltaTime);
        parent.transform.Rotate(this.transform.forward * rot * Time.deltaTime);
        parent.transform.Rotate(this.transform.right * rot * Time.deltaTime);
    }

    private void ToggleTrailRendererParticleSystem()
    {
        bool trailOn = true;
        bool particleOn = false;
        _trailRendererEnabled = !_trailRendererEnabled;
        if (_trailRendererEnabled)
        {
            trailOn = false;
            particleOn = true;
        }

        //toggle trail renderers
        foreach (TrailRenderer trail in TrailRenderers)
        {
            trail.enabled = !trail.enabled;
        }

        //toggle particle systems
        foreach (ParticleSystem system in ParticleSystems)
        {
            system.enableEmission = !system.enableEmission;
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
        DeleteBakeTrailRenderers();

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

    private void DeleteBakeTrailRenderers()
    {
        //delete any existing children
        foreach (Transform child in parent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}