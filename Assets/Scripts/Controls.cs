using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class Controls : MonoBehaviour
{
    public float TrailRenderTime = 2.0f;

    public TrailRenderer[] TrailRenderers;
    private List<TrailRenderer> avatar0Trails = new List<TrailRenderer>();
    private List<TrailRenderer> avatar1Trails = new List<TrailRenderer>();

    public ParticleSystem[] ParticleSystems;

    //camera control
    public GameObject mainCam;
    public GameObject subCam;


    //public SkinnedMeshRenderer BodyRenderer;
    private GameObject[] mesh ;
    private bool[] renderOn;

    private GameObject parent;

    //for trailmesh into stars
    private GameObject trailMesh;
    public VisualEffect trailStars;
    public bool trailStarsEnabled = false;
    public Material starMtl;

    //UI elements
    public GameObject dataSource;
    private DataSubscription data;
    public GameObject eventManager;
    public Dropdown dropDown;
    public InputField thresholdInput;
    public Toggle trailToMeshTogg;



    // Start is called before the first frame update
    private void Start()
    {
        //get all trail renderers
        TrailRenderers = FindObjectsOfType<TrailRenderer>();
        foreach (TrailRenderer trail in TrailRenderers){
            if (trail.transform.IsChildOf(GameObject.Find("Ch36_nonPBR").transform)){ avatar0Trails.Add(trail); }
            else { avatar1Trails.Add(trail); }
        }

        mesh = GameObject.FindGameObjectsWithTag("CH36");
        renderOn = new bool[] { mesh[0].GetComponent<SkinnedMeshRenderer>().enabled, mesh[1].GetComponent<SkinnedMeshRenderer>().enabled };
        //Debug.Log("Mesh list has " + mesh.Length + "Elements");


        //get all particle systems
        ParticleSystems = FindObjectsOfType<ParticleSystem>();

        //create parent transform for bake trail renderers
        parent = new GameObject();
        parent.transform.parent = this.gameObject.transform;
        parent.name = "Parent";
        parent.transform.localPosition = Vector3.zero;

        //create parent transform for bake trail renderers into stars
        trailMesh = new GameObject();
        trailMesh.transform.parent = this.gameObject.transform;
        trailMesh.name = "trailMesh";
        trailMesh.transform.localPosition = Vector3.zero;

        trailToMeshTogg.isOn = false;


        //get DataSubscriber
        data = dataSource.GetComponent<DataSubscription>();


    }

    private float _timeIncrement = 0.01f;
    private bool _trailRendererEnabled = true;

    /*/
    Tab: Camera Switch
    Plan View camera:
        Q:Zoom in
        E:Zoom out
    Perspective camera
        WSAD: Move front/back/left/right
        Q:Up
        E:Down
        Click Right Mouse Button: Move mouse to rotate camera
        
    Up/Down arrow: Increase/Decrease Trace Length
    V: turn off Avatar0 body renderer
    B: turn off Avatar1 body renderer
    Space: Bake Trail Renders
    X: Delete Baked Trail Renderers
    O: Avatar0 Toggle Particles/Trail Renderers
    P: Avatar1 Toggle Particles/Trail Renderers
    Dropdown menu = Select body parts to trigger colorburst
    Input Field = Set speed threshold to trigger colorburst
    Slider: Set Swirl star track length
    
    //*/

    // Update is called once per frame
    private void Update()
    {
        //camera control
        if (Input.GetKeyDown(KeyCode.Tab)) cameraSwitch();
        if(mainCam.activeInHierarchy == true && subCam.activeInHierarchy == false)
        {
            if (Input.GetKey(KeyCode.Q)) { mainCam.transform.Translate(5 * Vector3.up * Time.deltaTime, Space.World); }
            if (Input.GetKey(KeyCode.E)) { mainCam.transform.Translate(5 * Vector3.down * Time.deltaTime, Space.World); }
        }
        else if(mainCam.activeInHierarchy == false && subCam.activeInHierarchy == true)
        {
            if (Input.GetMouseButton(1)) {
                subCam.transform.Rotate(0f, Input.GetAxis("Mouse X"), 0f,Space.World);
                subCam.transform.Rotate(Input.GetAxis("Mouse Y"), 0f, 0f, Space.World);
                Vector3 rotation = new Vector3 (subCam.transform.localEulerAngles.x,subCam.transform.localEulerAngles.y,0f);
                subCam.transform.rotation = Quaternion.Euler(rotation);}
            if (Input.GetKey(KeyCode.Q)) { subCam.transform.Translate(3 * Vector3.up * Time.deltaTime ); }
            if (Input.GetKey(KeyCode.E)) { subCam.transform.Translate(3 * Vector3.down * Time.deltaTime); }
            if (Input.GetKey(KeyCode.W)) { subCam.transform.Translate(3 * Vector3.forward * Time.deltaTime); }
            if (Input.GetKey(KeyCode.S)) { subCam.transform.Translate(3 * Vector3.back * Time.deltaTime); }
            if (Input.GetKey(KeyCode.A)) { subCam.transform.Translate(3 * Vector3.left * Time.deltaTime); }
            if (Input.GetKey(KeyCode.D)) { subCam.transform.Translate(3 * Vector3.right * Time.deltaTime); }
        }



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
        //if (Input.GetKeyDown(KeyCode.B) && BodyRenderer != null) { BodyRenderer.enabled = !BodyRenderer.enabled; }

        if (Input.GetKeyDown(KeyCode.V)) { mesh[0].GetComponent<SkinnedMeshRenderer>().enabled = !renderOn[0]; renderOn[0] = !renderOn[0]; }
        if (Input.GetKeyDown(KeyCode.B)) { mesh[1].GetComponent<SkinnedMeshRenderer>().enabled = !renderOn[1]; renderOn[1] = !renderOn[1]; }


        //bake trail renderer as mesh
        if (Input.GetKeyDown(KeyCode.Space)) { BakeTrailRenderers(); }

        //delete drawing
        if (Input.GetKeyDown(KeyCode.X)) { DeleteBakeTrailRenderers(); }

        //toggle trail renderer and particle systems
        if (Input.GetKeyDown(KeyCode.O)) { ToggleTrailRendererParticleSystem(avatar0Trails); }
        if (Input.GetKeyDown(KeyCode.P)) { ToggleTrailRendererParticleSystem(avatar1Trails); }


        //rotate parent
        float rot = 20;
        parent.transform.Rotate(this.transform.up * rot * Time.deltaTime);
        parent.transform.Rotate(this.transform.forward * rot * Time.deltaTime);
        parent.transform.Rotate(this.transform.right * rot * Time.deltaTime);

        //update numbers from UI
        data.rigNumber = dropDown.value;
        if(thresholdInput.text != "0")
        { eventManager.GetComponent<VisualEventManager>().threshold = float.Parse(thresholdInput.text); }
        else
        { eventManager.GetComponent<VisualEventManager>().threshold = 20; }

        //Debug.Log("0");

        //turn trailmesh into stars
        trailStarsEnabled = trailToMeshTogg.isOn;
        if (trailStarsEnabled)
        {
            trailStars.gameObject.SetActive(true);
            /*if (trailMesh.GetComponent<MeshFilter>() != null && trailMesh.GetComponent<MeshRenderer>() != null)
            {
                Destroy(trailMesh.GetComponent<MeshFilter>()); Destroy(trailMesh.GetComponent<MeshRenderer>());
            }*/
            //trailMesh.GetComponent<MeshFilter>().mesh.Clear();
            BakeStarTrailRenderers();
            Debug.Log(trailMesh.GetComponentsInChildren<MeshFilter>().Length);
            MeshFilter[] meshFilters = trailMesh.GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            int i = 1;
            while (i < meshFilters.Length)
            {
                combine[i].mesh = meshFilters[i].mesh;
                //combine[i].transform = Matrix4x4.TRS(meshFilters[i].transform.localPosition, meshFilters[i].transform.localRotation, meshFilters[i].transform.localScale);
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                meshFilters[i].gameObject.SetActive(false);
                i++;
            }
            trailMesh.SetActive(true);
            if (trailMesh.GetComponent<MeshFilter>() == null)
            {
                trailMesh.AddComponent<MeshFilter>();
               //trailMesh.AddComponent<MeshRenderer>();
            }
            trailMesh.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
            //trailMesh.GetComponent<MeshRenderer>().material = starMtl;
            foreach(Transform child in trailMesh.transform)
            {
                child.GetComponent<MeshFilter>().mesh.Clear();
                GameObject.Destroy(child.gameObject); }
            trailStars.SetMesh("_spawnMesh", trailMesh.GetComponent<MeshFilter>().mesh);

            /*trailMesh.transform.GetComponent<MeshFilter>().mesh = new Mesh();
            trailMesh.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
            trailMesh.transform.gameObject.SetActive(true);
            Debug.Log(trailMesh.GetComponent<MeshFilter>().mesh.name);*/
            //trailStars.SetMesh("_spawnMesh", trailMesh.GetComponent<MeshFilter>().mesh);
        }
        

    }

    /*private void ToggleTrailRendererParticleSystem()
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
            //trail.enabled = !trail.enabled;
            trail.enabled = trailOn;
        }

        //toggle particle systems
        foreach (ParticleSystem system in ParticleSystems)
        {
            system.enableEmission = !system.enableEmission;
        }
    }*/

    private void cameraSwitch()
    {
        mainCam.SetActive(!mainCam.activeInHierarchy);
        subCam.SetActive(!subCam.activeInHierarchy);
    }
    private void ToggleTrailRendererParticleSystem(List<TrailRenderer> trails)
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
        foreach (TrailRenderer trail in trails)
        {
            //trail.enabled = !trail.enabled;
            trail.enabled = trailOn;
            Debug.Log("Trail status is " + trailOn);
        }

        //toggle particle systems
        /*foreach (ParticleSystem system in ParticleSystems)
        {
            system.enableEmission = !system.enableEmission;
        }*/
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

    private void BakeStarTrailRenderers()
    {
        Debug.Log("Method start");
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
            go.transform.parent = trailMesh.transform;
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