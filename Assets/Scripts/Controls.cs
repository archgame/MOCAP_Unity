using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using HSVPicker;
using UnityEngine.SceneManagement;

public class Controls : MonoBehaviour
{

    //UI elements
    public Canvas gameUI;
    public GameObject eventManager;
    public Dropdown dropDown;
    public Dropdown traceLineType;
    public InputField thresholdInput;
    public Toggle trailToMeshTogg;

    //trailine material
    public Material[] trailLineMat;

    public float TrailRenderTime = 2.0f;

    public TrailRenderer[] TrailRenderers;
    public List<TrailRenderer> avatar0Trails = new List<TrailRenderer>();
    public List<TrailRenderer> avatar1Trails = new List<TrailRenderer>();

    public ParticleSystem[] ParticleSystems;

    //set trail color gradient for both avatars
    public ColorPicker ava0TrailHead;
    public ColorPicker ava0TrailTail;
    public ColorPicker ava1TrailHead;
    public ColorPicker ava1TrailTail;

    //set trail color with button
    public GameObject[] colorButtons;
    private Color[] headTailColor = new Color[4];

    //set blending color according to dist
    private int countdownTime = 5;
    private float initialDist;
    private float currentDist;

    //camera control
    public GameObject mainCam;
    public GameObject subCam;

    //grids
    public GameObject[] grids;
    public float timerStart;

    //public SkinnedMeshRenderer BodyRenderer;
    private GameObject[] mesh;
    private bool[] renderOn;

    private GameObject[] parent = new GameObject[2];
    private GameObject[] hips = new GameObject[2];

    //for trailmesh into stars

    private GameObject[] trailMesh = new GameObject[2];

    public VisualEffect[] trailStars;
    private bool trailStarsEnabled = false;
    public Material starMtl;

    private VisualEventManager vem;






    // Start is called before the first frame update
    private void Start()
    {

        //get all trail renderers
        TrailRenderers = FindObjectsOfType<TrailRenderer>();
        foreach (TrailRenderer trail in TrailRenderers) {
            if (trail.transform.IsChildOf(GameObject.Find("Ch36_nonPBR").transform)) { avatar0Trails.Add(trail); }
            else { avatar1Trails.Add(trail); }
        }

        hips = GameObject.FindGameObjectsWithTag("hip");

        //set trailine linetype
        traceLineType.onValueChanged.AddListener(Value =>
        {
            if (Value == 0) { foreach (var trail in TrailRenderers) { trail.material = trailLineMat[0]; } }
            else if (Value == 1) { foreach (var trail in TrailRenderers) { trail.material = trailLineMat[1]; } }
            else if (Value == 2) { foreach (var trail in TrailRenderers) { trail.material = trailLineMat[2]; } }
        });

        //set trail color gradients
        /*
        ava0TrailHead.onValueChanged.AddListener(color => { foreach (TrailRenderer trail in avatar0Trails) { trail.material.SetColor("_startColor", color); } });
        ava0TrailTail.onValueChanged.AddListener(color => { foreach (TrailRenderer trail in avatar0Trails) { trail.material.SetColor("_endColor", color); } });
        ava1TrailHead.onValueChanged.AddListener(color => { foreach (TrailRenderer trail in avatar1Trails) { trail.material.SetColor("_startColor", color); } });
        ava1TrailTail.onValueChanged.AddListener(color => { foreach (TrailRenderer trail in avatar1Trails) { trail.material.SetColor("_endColor", color); } });
        */

        //set trail color with Button
        foreach(var trail in TrailRenderers) {
            trail.material.SetColor("_startColor", Color.white);
            trail.material.SetColor("_endColor", Color.white);
        }
        for(int i =0; i<headTailColor.Length; i++) {
            headTailColor[i] = Color.white;
        }
        Button[] buttonList0 = colorButtons[0].GetComponentsInChildren<Button>();
        Button[] buttonList1 = colorButtons[1].GetComponentsInChildren<Button>();
        Button[] buttonList2 = colorButtons[2].GetComponentsInChildren<Button>();
        Button[] buttonList3 = colorButtons[3].GetComponentsInChildren<Button>();
        Debug.Log("ButtonList has " + buttonList0.Length + " buttons");
        foreach(var colBott in buttonList0) {
            colBott.onClick.AddListener(() => { foreach (TrailRenderer trail in avatar0Trails) { trail.material.SetColor("_startColor", colBott.colors.normalColor); } });
            colBott.onClick.AddListener(() => { for (int i = 0; i < buttonList0.Length; i++) { buttonList0[i].GetComponent<Outline>().enabled = false; } colBott.GetComponent<Outline>().enabled = true ; });
            colBott.onClick.AddListener(() => { headTailColor[0] = colBott.colors.normalColor;  });
        }
        foreach (var colBott in buttonList1) {
            colBott.onClick.AddListener(() => { foreach (TrailRenderer trail in avatar0Trails) { trail.material.SetColor("_endColor", colBott.colors.normalColor);  } });
            colBott.onClick.AddListener(() => { for (int i = 0; i < buttonList1.Length; i++) { buttonList1[i].GetComponent<Outline>().enabled = false; } colBott.GetComponent<Outline>().enabled = true; });
            colBott.onClick.AddListener(() => { headTailColor[1] = colBott.colors.normalColor; });
        }
        foreach (var colBott in buttonList2) {
            colBott.onClick.AddListener(() => { foreach (TrailRenderer trail in avatar1Trails) { trail.material.SetColor("_startColor", colBott.colors.normalColor); } });
            colBott.onClick.AddListener(() => { for (int i = 0; i < buttonList2.Length; i++) { buttonList2[i].GetComponent<Outline>().enabled = false; } colBott.GetComponent<Outline>().enabled = true; });
            colBott.onClick.AddListener(() => { headTailColor[2] = colBott.colors.normalColor; });
        }
        foreach (var colBott in buttonList3) {
            colBott.onClick.AddListener(() => { foreach (TrailRenderer trail in avatar1Trails) { trail.material.SetColor("_endColor", colBott.colors.normalColor); } });
            colBott.onClick.AddListener(() => { for (int i = 0; i < buttonList3.Length; i++) { buttonList3[i].GetComponent<Outline>().enabled = false; } colBott.GetComponent<Outline>().enabled = true; });
            colBott.onClick.AddListener(() => { headTailColor[3] = colBott.colors.normalColor; });
        }



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
            trailMesh[i].name = "trailMesh" + i;
            trailMesh[i].transform.localPosition = Vector3.zero;
        }

        trailToMeshTogg.isOn = false;


        //get VisualEventManager Sciprt
        vem = GameObject.Find("VfxEventManager").GetComponent<VisualEventManager>();


        foreach (TrailRenderer trail in TrailRenderers) {
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
        //UI control
        if (Input.GetKeyDown(KeyCode.Y)) { gameUI.enabled = !gameUI.enabled; Cursor.visible = gameUI.enabled; }
        //gameUI.gameObject.SetActive(!gameUI.gameObject.activeInHierarchy);
        //camera control
        if (Input.GetKeyDown(KeyCode.Tab)) cameraSwitch();
        if (mainCam.activeInHierarchy == true && subCam.activeInHierarchy == false) {
            if (Input.GetKey(KeyCode.Q)) { mainCam.transform.Translate(5 * Vector3.up * Time.deltaTime, Space.World); }
            if (Input.GetKey(KeyCode.E)) { mainCam.transform.Translate(5 * Vector3.down * Time.deltaTime, Space.World); }
        }
        else if (mainCam.activeInHierarchy == false && subCam.activeInHierarchy == true) {
            if (Input.GetMouseButton(1)) {
                subCam.transform.Rotate(0f, Input.GetAxis("Mouse X"), 0f, Space.World);
                subCam.transform.Rotate(Input.GetAxis("Mouse Y"), 0f, 0f, Space.World);
                Vector3 rotation = new Vector3(subCam.transform.localEulerAngles.x, subCam.transform.localEulerAngles.y, 0f);
                subCam.transform.rotation = Quaternion.Euler(rotation);
            }
            if (Input.GetKey(KeyCode.Q)) { subCam.transform.Translate(3 * Vector3.up * Time.deltaTime); }
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
        float lerp = math.remap(0.25f, 1f, 0.5f, 1f, currentDist / initialDist);
        foreach (TrailRenderer trail in avatar0Trails) {
            trail.material.SetColor("_startColor", Color.Lerp(headTailColor[2], headTailColor[0], lerp));
            trail.material.SetColor("_endColor", Color.Lerp(headTailColor[3], headTailColor[1], lerp));
        }
        foreach (TrailRenderer trail in avatar1Trails) {
            trail.material.SetColor("_startColor", Color.Lerp(headTailColor[0], headTailColor[2], lerp));
            trail.material.SetColor("_endColor", Color.Lerp(headTailColor[1], headTailColor[3], lerp));
        }

        //quit application
        if (Input.GetKeyDown(KeyCode.Escape)) { Debug.Log("Quit Application"); Application.Quit(); }

        //resart Scene
        if (Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadSceneAsync(
            SceneManager.GetActiveScene().buildIndex);
        }

        //increase trail render time
        float tempTime = TrailRenderTime;
        if (Input.GetKey(KeyCode.UpArrow)) { tempTime += _timeIncrement; Debug.Log("Up"); }
        if (Input.GetKey(KeyCode.DownArrow)) { tempTime -= _timeIncrement; Debug.Log("Down"); }
        if (TrailRenderTime > 30) { tempTime = 30; }
        if (TrailRenderTime < 0) { tempTime = 0; }
        if (tempTime != TrailRenderTime) {
            TrailRenderTime = tempTime;
            UpdateTrailRenderers();
        }

        //resart Scene

        //toggle body renderer on and off
        //if (Input.GetKeyDown(KeyCode.B) && BodyRenderer != null) { BodyRenderer.enabled = !BodyRenderer.enabled; }

        if (Input.GetKeyDown(KeyCode.V)) { mesh[0].GetComponent<SkinnedMeshRenderer>().enabled = !renderOn[0]; renderOn[0] = !renderOn[0]; }
        if (Input.GetKeyDown(KeyCode.B)) { mesh[1].GetComponent<SkinnedMeshRenderer>().enabled = !renderOn[1]; renderOn[1] = !renderOn[1]; }


        //bake trail renderer as mesh
        if (Input.GetKeyDown(KeyCode.Space)) { BakeTrailRenderersByAvatar(avatar0Trails, 0); BakeTrailRenderersByAvatar(avatar1Trails, 1); }

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

        if (thresholdInput.text != "0") { vem.threshold = float.Parse(thresholdInput.text); }
        else { vem.threshold = 10; }

        //Debug.Log("0");

        if (grids[0].activeInHierarchy) {
            timerStart += Time.deltaTime;
            if (timerStart >= 60f && grids[0].transform.position.y <= 3) {
                grids[0].transform.Translate(Vector3.up * 0.5f * Time.deltaTime);
                timerStart = 121f;
            }

        }

        //turn trailmesh into stars
        trailStarsEnabled = trailToMeshTogg.isOn;
        if (trailStarsEnabled) {
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
    private void CameraControl()
    {

    }
    private void ToggleTrailRendererParticleSystem(List<TrailRenderer> trails)
    {
        bool trailOn = true;
        bool particleOn = false;
        _trailRendererEnabled = !_trailRendererEnabled;
        if (_trailRendererEnabled) {
            trailOn = false;
            particleOn = true;
        }

        //toggle trail renderers
        foreach (TrailRenderer trail in trails) {
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
        foreach (TrailRenderer trail in TrailRenderers) {
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
        foreach (TrailRenderer trail in trails) {
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


    public void BakeTrailRenderersByAvatar(List<TrailRenderer> trails, int avaIndex)
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

            // Adding MeshCollider and assigning the bakedMesh.translateBakedTrail
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

    //for calling method from outside
    public void BakeTrail()
    {
        BakeTrailRenderersByAvatar(avatar0Trails, 0);
        BakeTrailRenderersByAvatar(avatar1Trails, 1);
    }

    public void DeleteBakeTrailRenderersByAvatar(int avatarIndex)
    {
        //delete any existing children
        foreach (Transform child in parent[avatarIndex].transform) {
            GameObject.Destroy(child.gameObject);
        }
    }

    private void translateBakedTrail(GameObject trailParent, GameObject target, float spd)
    {
        foreach (Transform child in trailParent.transform) {
            /*if (Vector3.Distance(child.GetComponent<Renderer>().transform.position, target.transform.position) <= 3f) {
                Vector3 direction = target.transform.position - child.GetComponent<Renderer>().bounds.center;
                Vector3 randomForce = new Vector3(UnityEngine.Random.Range(0f, 3f), UnityEngine.Random.Range(0f, .1f), UnityEngine.Random.Range(0f, .1f));
                child.GetComponent<Renderer>().transform.Translate((spd * Time.deltaTime * direction.normalized));
                float intensity = Mathf.PingPong(Time.time, 4f);
                intensity = 2f - intensity;
                child.GetComponent<Renderer>().transform.Translate(Vector3.left * intensity * Time.deltaTime);
            }
            else {*/
                Vector3 direction = target.transform.position - child.GetComponent<Renderer>().bounds.center;
                Vector3 randomForce = new Vector3(UnityEngine.Random.Range(0f, 3f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
                child.GetComponent<Renderer>().transform.Translate((spd * Time.deltaTime * direction.normalized));
                float intensity = Mathf.PingPong(Time.time, 4f);
                intensity = 2f - intensity;
                child.GetComponent<Renderer>().transform.Translate(Vector3.left * intensity * Time.deltaTime);
        }
            //Debug.DrawLine(child.transform.position , target.transform.position, Color.white, 2f);
    }



    IEnumerator SetUpColorInitialize()
    {
        yield return new WaitForSeconds(countdownTime);
        initialDist = Vector3.Distance(hips[0].transform.position, hips[1].transform.position);
        Debug.Log("Initial distance is" + initialDist);

    }

    /*public IEnumerator AutoBake(bool toggle)
    {
        yield return new WaitForSeconds(25f);
        if (toggle) {
            float delta = Mathf.Repeat(Time.time, 11f);
            float timer = 10f - delta;
            if (timer <= 0f) {
                BakeTrailRenderersByAvatar(avatar0Trails, 0);
                BakeTrailRenderersByAvatar(avatar1Trails, 1);
                yield return new WaitForSeconds(1f);
            }
        }
    }*/
    public void TrailBlack()
    {
        foreach (var trail in TrailRenderers) {
            trail.material.SetColor("_startColor", Color.black);
            trail.material.SetColor("_endColor", Color.black);
        }
        for (int i = 0; i < headTailColor.Length; i++) {
            headTailColor[i] = Color.black;
        }
    }
    
}