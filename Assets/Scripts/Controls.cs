using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using HSVPicker;

public class Controls : MonoBehaviour
{
    public float TrailRenderTime = 2.0f;

    public TrailRenderer[] TrailRenderers;
    private List<TrailRenderer> avatar0Trails = new List<TrailRenderer>();
    private List<TrailRenderer> avatar1Trails = new List<TrailRenderer>();

    public ParticleSystem[] ParticleSystems;

    //set trail color gradient for both avatars
    public ColorPicker ava0TrailHead;
    public ColorPicker ava0TrailTail;
    public ColorPicker ava1TrailHead;
    public ColorPicker ava1TrailTail;
    //set blending color according to dist
    private int countdownTime = 5;
    private float initialDist;
    private float currentDist;

    //camera control
    public GameObject mainCam;
    public GameObject subCam;


    //public SkinnedMeshRenderer BodyRenderer;
    private GameObject[] mesh ;
    private bool[] renderOn;

    private GameObject[] parent = new GameObject[2];
    private GameObject[] hips = new GameObject[2];

    //for trailmesh into stars
    private GameObject[] trailMesh = new GameObject[2];
    public VisualEffect[] trailStars;
    private bool trailStarsEnabled = false;
    public Material starMtl;

    private VisualEventManager vem;

    //UI elements
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

        hips = GameObject.FindGameObjectsWithTag("hip");

        //set trail color gradients
        ava0TrailHead.onValueChanged.AddListener(color => { foreach (TrailRenderer trail in avatar0Trails) { trail.material.SetColor("_startColor", color); } });
        ava0TrailTail.onValueChanged.AddListener(color => { foreach (TrailRenderer trail in avatar0Trails) { trail.material.SetColor("_endColor", color); } });
        ava1TrailHead.onValueChanged.AddListener(color => { foreach (TrailRenderer trail in avatar1Trails) { trail.material.SetColor("_startColor", color); } });
        ava1TrailTail.onValueChanged.AddListener(color => { foreach (TrailRenderer trail in avatar1Trails) { trail.material.SetColor("_endColor", color); } });
        //Find intial distance
        StartCoroutine("SetUpColorInitialize");

        mesh = GameObject.FindGameObjectsWithTag("CH36");
        renderOn = new bool[] { mesh[0].GetComponent<SkinnedMeshRenderer>().enabled, mesh[1].GetComponent<SkinnedMeshRenderer>().enabled };
        //Debug.Log("Mesh list has " + mesh.Length + "Elements");


        //get all particle systems
        ParticleSystems = FindObjectsOfType<ParticleSystem>();



        //create parent transform for bake trail renderers
        for (int i = 0; i < parent.Length; i++) {
            parent[i] = new GameObject();
            parent[i].transform.parent = this.gameObject.transform;
            parent[i].name = "Parent" + i;
            parent[i].transform.localPosition = Vector3.zero;
        }


        //create parent transform for bake trail renderers into stars
        for (int i = 0; i < trailMesh.Length; i++) {
            trailMesh[i] = new GameObject();
            trailMesh[i].transform.parent = this.gameObject.transform;
            trailMesh[i].name = "trailMesh"+i;
            trailMesh[i].transform.localPosition = Vector3.zero;
        }

        trailToMeshTogg.isOn = false;


        //get VisualEventManager Sciprt
        vem = GameObject.Find("VfxEventManager").GetComponent<VisualEventManager>();


        foreach(TrailRenderer trail in TrailRenderers) {
            trail.enabled = false;
        }

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

        //blend color according to distance
        currentDist = Vector3.Distance(hips[0].transform.position, hips[1].transform.position);
        currentDist = math.min(currentDist, initialDist);
        currentDist = math.max(currentDist, 0.25f * initialDist);
        float lerp = math.remap(0.25f, 1f,0.5f,1f, currentDist/initialDist);
        foreach (TrailRenderer trail in avatar0Trails) {
            trail.material.SetColor("_startColor", Color.Lerp(ava1TrailHead.CurrentColor, ava0TrailHead.CurrentColor, lerp));
            trail.material.SetColor("_endColor", Color.Lerp(ava1TrailTail.CurrentColor, ava0TrailTail.CurrentColor, lerp));
        }
        foreach (TrailRenderer trail in avatar1Trails) {
            trail.material.SetColor("_startColor", Color.Lerp(ava0TrailHead.CurrentColor, ava1TrailHead.CurrentColor, lerp));
            trail.material.SetColor("_endColor", Color.Lerp(ava0TrailTail.CurrentColor, ava1TrailTail.CurrentColor, lerp));
        }

        //quit application
        if (Input.GetKeyDown(KeyCode.Escape)) { Debug.Log("Quit Application"); Application.Quit(); }

        //increase trail render time
        float tempTime = TrailRenderTime;
        if (Input.GetKey(KeyCode.UpArrow)) { tempTime += _timeIncrement; Debug.Log("Up"); }
        if (Input.GetKey(KeyCode.DownArrow)) { tempTime -= _timeIncrement; Debug.Log("Down"); }
        if (TrailRenderTime > 30) { tempTime = 30; }
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
        if (Input.GetKeyDown(KeyCode.Space)) { BakeTrailRenderersByAvatar(avatar0Trails,0); BakeTrailRenderersByAvatar(avatar1Trails, 1);}

        //delete drawing
        if (Input.GetKeyDown(KeyCode.X)) { DeleteBakeTrailRenderersByAvatar(0); DeleteBakeTrailRenderersByAvatar(1); }

        //toggle trail renderer and particle systems
        if (Input.GetKeyDown(KeyCode.O)) { ToggleTrailRendererParticleSystem(avatar0Trails); }
        if (Input.GetKeyDown(KeyCode.P)) { ToggleTrailRendererParticleSystem(avatar1Trails); }


        //rotate parent
        float rot = 20;
        float trans = 1;

        /*for (int i=0; i<parent.Length; i++) {
            parent[i].transform.Rotate(this.transform.up * rot * Time.deltaTime);
            parent[i].transform.Rotate(this.transform.forward * rot * Time.deltaTime);
            parent[i].transform.Rotate(this.transform.right * rot * Time.deltaTime);
        }*/

        translateBakedTrail(parent[0], hips[1], trans);
        translateBakedTrail(parent[1], hips[0], trans);


        //update numbers from UI
        vem.rigNumber = dropDown.value;

        if(thresholdInput.text != "0")
        { vem.threshold = float.Parse(thresholdInput.text); }
        else
        { vem.threshold = 10; }

        //Debug.Log("0");

        //turn trailmesh into stars
        trailStarsEnabled = trailToMeshTogg.isOn;
        if (trailStarsEnabled)
        {
            trailStars[0].gameObject.SetActive(true);
            trailStars[1].gameObject.SetActive(true);
            BakeStarTrailRenderersByAvatar(avatar0Trails, 0);
            BakeStarTrailRenderersByAvatar(avatar1Trails, 1);
            CombineChildrenMesh(trailMesh[0]);
            CombineChildrenMesh(trailMesh[1]);
            trailStars[0].SetMesh("_spawnMesh", trailMesh[0].GetComponent<MeshFilter>().mesh);
            trailStars[1].SetMesh("_spawnMesh", trailMesh[1].GetComponent<MeshFilter>().mesh);
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

    /*private void BakeTrailRenderers()
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
    }*/

    private void BakeStarTrailRenderersByAvatar(List<TrailRenderer> trails, int index)
    {
        Debug.Log("Method start");
        //bake trail renderer and add to parent
        foreach (TrailRenderer trail in trails)
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
            go.transform.parent = trailMesh[index].transform;
        }
    }

    private void CombineChildrenMesh(GameObject mesh)
    {
        MeshFilter[] meshFilters = mesh.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 1;
        while (i < meshFilters.Length) {
            combine[i].mesh = meshFilters[i].mesh;
            //combine[i].transform = Matrix4x4.TRS(meshFilters[i].transform.localPosition, meshFilters[i].transform.localRotation, meshFilters[i].transform.localScale);
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
            i++;
        }
        mesh.SetActive(true);
        if (mesh.GetComponent<MeshFilter>() == null) {
            mesh.AddComponent<MeshFilter>();
            //trailMesh.AddComponent<MeshRenderer>();
        }
        mesh.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        //trailMesh.GetComponent<MeshRenderer>().material = starMtl;
        foreach (Transform child in mesh.transform) {
            child.GetComponent<MeshFilter>().mesh.Clear();
            GameObject.Destroy(child.gameObject);
        }
    }


    private void BakeTrailRenderersByAvatar(List<TrailRenderer> trails, int avaIndex)
    {
        Debug.Log("Method start");
        //bake trail renderer and add to parent
        foreach (TrailRenderer trail in trails) {
            //bake mesh
            Mesh bakedMesh = new Mesh();
            trail.BakeMesh(bakedMesh);

            // Recalcultate the bounding volume of the mesh from the vertices.
            //bakedMesh.RecalculateBounds();
            //Debug.Log("Baked mesh bounds: " + bakedMesh.bounds.ToString());

            // Adding MeshCollider and assigning the bakedMesh.
            GameObject go = new GameObject();
            MeshFilter meshFilter = go.AddComponent<MeshFilter>();
            meshFilter.mesh = bakedMesh;
            MeshRenderer renderer = go.AddComponent<MeshRenderer>();
            renderer.material = trail.material;
            //Rigidbody rigidBody = go.AddComponent<Rigidbody>();

            //add to parent
            go.transform.TransformPoint(hips[avaIndex].transform.position);
            go.transform.parent = parent[avaIndex].transform;
            
        }
    }

    private void DeleteBakeTrailRenderersByAvatar(int avatarIndex)
    {
        //delete any existing children
        foreach (Transform child in parent[avatarIndex].transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    private void translateBakedTrail(GameObject trailParent, GameObject target, float spd)
    {
        
        foreach (Transform child in trailParent.transform) {
            Vector3 direction = target.transform.position - child.GetComponent<Renderer>().bounds.center;
            
            child.transform.Translate(spd * Time.deltaTime * direction.normalized);
            //Debug.DrawLine(child.transform.position , target.transform.position, Color.white, 2f);
            
            
        }

        

    }

    IEnumerator SetUpColorInitialize()
    {
        yield return new WaitForSeconds(countdownTime);
        initialDist = Vector3.Distance(hips[0].transform.position, hips[1].transform.position);
        Debug.Log("Initial distance is" + initialDist);

    }

}