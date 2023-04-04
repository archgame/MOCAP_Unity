using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using HSVPicker;
using UnityEngine.SceneManagement;
using static CharacterManager;

public class Controls : MonoBehaviour
{
    //UI elements
    public Canvas gameUI;

    public GameObject eventManager;
    public Dropdown dropDown;
    public Dropdown traceLineType;
    public InputField thresholdInput;
    //trailToMeshTogg;

    //trailine material
    public Material[] trailLineMat;

    public float TrailRenderTime = 2.0f;

    private TrailRenderer[] TrailRenderers;
    public List<TrailRenderer> avatar0Trails = new List<TrailRenderer>();
    public List<TrailRenderer> avatar1Trails = new List<TrailRenderer>();
    public List<TrailRenderer> avatar2Trails = new List<TrailRenderer>();

    public ParticleSystem[] ParticleSystems;

    //set trail color with button
    public GameObject[] colorButtons;


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

    private GameObject[] sensors;

    private GameObject[] parent = new GameObject[3];

    private GameObject[] hips = new GameObject[3];

    private VisualEventManager vem;

    //reset Position
    public GameObject[] parentObjs;

    public bool sensor0Toggle;
    public bool sensor1Toggle;
    public bool sensor2Toggle;

    // Start is called before the first frame update
    private void Start()
    {
        sensor0Toggle = true;
        sensor1Toggle = true;
        sensor2Toggle = true;


        sensors = GameObject.FindGameObjectsWithTag("Sensor");
        //Debug.Log("Sensors length is " + sensors.Length);

        //get all trail renderers
        TrailRenderers = FindObjectsOfType<TrailRenderer>();
        foreach (TrailRenderer trail in TrailRenderers)
        {
            if (trail.transform.IsChildOf(GameObject.Find("Ch36_nonPBR").transform)) { avatar0Trails.Add(trail); }
            else if (trail.transform.IsChildOf(GameObject.Find("Ch36_nonPBR (1)").transform)) { avatar1Trails.Add(trail); }
            else if (trail.transform.IsChildOf(GameObject.Find("Ch36_nonPBR (2)").transform)) { avatar2Trails.Add(trail); }
        }

        hips = GameObject.FindGameObjectsWithTag("hip");

        //set trailine linetype
        traceLineType.onValueChanged.AddListener(Value =>
        {
            if (Value == 0) { foreach (var trail in TrailRenderers) { trail.material = trailLineMat[0]; } }
            else if (Value == 1) { foreach (var trail in TrailRenderers) { trail.material = trailLineMat[1]; } }
            else if (Value == 2) { foreach (var trail in TrailRenderers) { trail.material = trailLineMat[2]; } }
        });


        //set trail color with Button
        foreach (var trail in TrailRenderers)
        {
            trail.material.SetColor("_startColor", Color.white);
            trail.material.SetColor("_endColor", Color.white);
            trail.enabled = false;
        }

        //Color buttons functioni 
        Button[][] buttonlists = new Button[colorButtons.Length][];
        for(int i = 0; i < colorButtons.Length; i++) {
            buttonlists[i] = colorButtons[i].GetComponentsInChildren<Button>();
            List<TrailRenderer> trails = i switch
            {
                0 => avatar0Trails,
                1 => avatar1Trails,
                2 => avatar2Trails,
                _ => null,
            };
            string gridParam = i switch
            {
                0 => "_gradientOut0",
                1 => "_gradientOut1",
                2 => "_gradientOut2",
                _ => null,
            };
            ColorButtonFunction(buttonlists[i], trails, gridParam);

        }
        /*
        foreach (var colBott in buttonList1)
        {
            colBott.onClick.AddListener(() => { foreach (TrailRenderer trail in avatar0Trails) { trail.material.SetColor("_endColor", colBott.colors.normalColor); } });
            colBott.onClick.AddListener(() => { for (int i = 0; i < buttonList1.Length; i++) { buttonList1[i].GetComponent<Outline>().enabled = false; } colBott.GetComponent<Outline>().enabled = true; });
            colBott.onClick.AddListener(() => { grids[0].GetComponent<MeshRenderer>().material.SetColor("_gradientOut0", colBott.colors.normalColor); });
        }
        foreach (var colBott in buttonList3)
        {
            colBott.onClick.AddListener(() => { foreach (TrailRenderer trail in avatar1Trails) { trail.material.SetColor("_endColor", colBott.colors.normalColor); } });
            colBott.onClick.AddListener(() => { for (int i = 0; i < buttonList3.Length; i++) { buttonList3[i].GetComponent<Outline>().enabled = false; } colBott.GetComponent<Outline>().enabled = true; });
            colBott.onClick.AddListener(() => { grids[0].GetComponent<MeshRenderer>().material.SetColor("_gradientOut1", colBott.colors.normalColor); });
        }
        foreach (var colBott in buttonList5) {
            colBott.onClick.AddListener(() => { foreach (TrailRenderer trail in avatar2Trails) { trail.material.SetColor("_endColor", colBott.colors.normalColor); } });
            colBott.onClick.AddListener(() => { for (int i = 0; i < buttonList5.Length; i++) { buttonList3[i].GetComponent<Outline>().enabled = false; } colBott.GetComponent<Outline>().enabled = true; });
            colBott.onClick.AddListener(() => { grids[0].GetComponent<MeshRenderer>().material.SetColor("_gradientOut1", colBott.colors.normalColor); });
        }
        */

        //Find intial distance
        //StartCoroutine("SetUpColorInitialize");

        //mesh = GameObject.FindGameObjectsWithTag("CH36");
        //renderOn = new bool[] { mesh[0].GetComponent<SkinnedMeshRenderer>().enabled, mesh[1].GetComponent<SkinnedMeshRenderer>().enabled };
        //Debug.Log("Mesh list has " + mesh.Length + "Elements");

        //get all particle systems
        ParticleSystems = FindObjectsOfType<ParticleSystem>();

        //create parent transform for bake trail renderers
        for (int i = 0; i < parent.Length; i++)
        {
            parent[i] = new GameObject();
            parent[i].transform.parent = this.gameObject.transform;
            parent[i].name = "Parent" + i;
            parent[i].transform.localPosition = Vector3.zero;
        }

        //get VisualEventManager Sciprt
        vem = GameObject.Find("VfxEventManager").GetComponent<VisualEventManager>();

        foreach (TrailRenderer trail in TrailRenderers)
        {
            trail.enabled = false;
        }
    }

    private float _timeIncrement = 0.2f;
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
        //reset position
        if (Input.GetKeyDown(KeyCode.F))
        {
            Vector3 offSet0 = new Vector3(hips[0].transform.position.x, 0f, hips[0].transform.position.z);
            Vector3 offSet1 = new Vector3(hips[1].transform.position.x, 0f, hips[1].transform.position.z);
            Vector3 offSet2 = new Vector3(hips[2].transform.position.x, 0f, hips[2].transform.position.z);
            parentObjs[0].transform.Translate(offSet0 * (-1f));
            //parentObjs[1].transform.Translate(hips[0].transform.position * (-1f));
            //parentObjs[2].transform.Translate(hips[0].transform.position * (-1f));
            parentObjs[1].transform.position = parentObjs[0].transform.position;
            parentObjs[2].transform.position = parentObjs[0].transform.position;
            parentObjs[3].transform.Translate(offSet1 * (-1f));
            //parentObjs[4].transform.Translate(hips[1].transform.position * (-1f));
            //parentObjs[5].transform.Translate(hips[1].transform.position * (-1f));
            parentObjs[4].transform.position = parentObjs[3].transform.position;
            parentObjs[5].transform.position = parentObjs[3].transform.position;
            parentObjs[6].transform.Translate(offSet2 * (-1f));
            parentObjs[7].transform.position = parentObjs[6].transform.position;
            parentObjs[8].transform.position = parentObjs[6].transform.position;
        }

        //UI On and Off
        if (Input.GetKeyDown(KeyCode.Y)) { gameUI.enabled = !gameUI.enabled; Cursor.visible = gameUI.enabled; }
        //gameUI.gameObject.SetActive(!gameUI.gameObject.activeInHierarchy);

        #region CameraControl
        if (Input.GetKeyDown(KeyCode.Tab)) cameraSwitch();

        if (mainCam.activeInHierarchy == true && subCam.activeInHierarchy == false)
        {
            if (Input.GetKey(KeyCode.Q)) { mainCam.transform.Translate(5 * Vector3.up * Time.deltaTime, Space.World); }
            if (Input.GetKey(KeyCode.E)) { mainCam.transform.Translate(5 * Vector3.down * Time.deltaTime, Space.World); }
        }
        else if (mainCam.activeInHierarchy == false && subCam.activeInHierarchy == true)
        {
            if (Input.GetMouseButton(0))
            {
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
        #endregion CameraControl


        //quit application
        if (Input.GetKeyDown(KeyCode.Escape)) { Debug.Log("Quit Application"); Application.Quit(); }

        //resart Scene
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadSceneAsync(
            SceneManager.GetActiveScene().buildIndex);
        }

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

        //resart Scene

        //toggle body renderer on and off
        //if (Input.GetKeyDown(KeyCode.B) && BodyRenderer != null) { BodyRenderer.enabled = !BodyRenderer.enabled; }

        if (Input.GetKeyDown(KeyCode.Alpha1) && CharMGM.Char0Avatars[(CharMGM.Char0Index + 2) % 3] != null)
        {
            SkinnedMeshRenderer[] skins = CharMGM.Char0Avatars[(CharMGM.Char0Index + 2) % 3].GetComponentsInChildren<SkinnedMeshRenderer>();
            //bool notCurrent = !skins[0].enabled;
            sensor0Toggle = !sensor0Toggle;
            foreach (var skin in skins) { skin.enabled = sensor0Toggle; }
            foreach (var sensor in sensors) { if (sensor.transform.IsChildOf(GameObject.Find("Ch36_nonPBR").transform)) sensor.GetComponent<MeshRenderer>().enabled = sensor0Toggle; }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && CharMGM.Char1Avatars[(CharMGM.Char1Index + 2) % 3] != null)
        {
            SkinnedMeshRenderer[] skins = CharMGM.Char1Avatars[(CharMGM.Char1Index + 2) % 3].GetComponentsInChildren<SkinnedMeshRenderer>();
            sensor1Toggle = !sensor1Toggle;
            foreach (var skin in skins) { skin.enabled = sensor1Toggle; }
            foreach (var sensor in sensors) { if (sensor.transform.IsChildOf(GameObject.Find("Ch36_nonPBR (1)").transform)) sensor.GetComponent<MeshRenderer>().enabled = sensor1Toggle; }
        }

        //bake trail renderer as mesh
        if (Input.GetKeyDown(KeyCode.Space)) { 
            BakeTrailRenderersByAvatar(avatar0Trails, 0); 
            BakeTrailRenderersByAvatar(avatar1Trails, 1);
            BakeTrailRenderersByAvatar(avatar2Trails, 2);
        }

        //delete drawing
        if (Input.GetKeyDown(KeyCode.X)) { DeleteBakeTrailRenderersByAvatar(0); DeleteBakeTrailRenderersByAvatar(1);
          DeleteBakeTrailRenderersByAvatar(2);
        }

        //toggle trail renderer and particle systems
        //if (Input.GetKeyDown(KeyCode.O)) { ToggleTrailRendererParticleSystem(avatar0Trails); }
        //if (Input.GetKeyDown(KeyCode.P)) { ToggleTrailRendererParticleSystem(avatar1Trails); }

        //rotate parent
        float rot = 20;
        float trans = 1;

        /*for (int i=0; i<parent.Length; i++) {
            parent[i].transform.Rotate(this.transform.up * rot * Time.deltaTime);
            parent[i].transform.Rotate(this.transform.forward * rot * Time.deltaTime);
            parent[i].transform.Rotate(this.transform.right * rot * Time.deltaTime);
        }*/

        translateBakedTrail(parent[0], hips[1], trans);
        translateBakedTrail(parent[1], hips[2], trans);
        translateBakedTrail(parent[2], hips[0], trans);

        //update numbers from UI
        vem.rigNumber = dropDown.value;

        if (thresholdInput.text != "0") { vem.threshold = float.Parse(thresholdInput.text); }
        else { vem.threshold = 10; }
    }


    private void cameraSwitch()
    {
        mainCam.SetActive(!mainCam.activeInHierarchy);
        subCam.SetActive(!subCam.activeInHierarchy);
    }

    public void cameraSelect(int i)
    {
        if (i == 0) { mainCam.SetActive(true); subCam.SetActive(false); }
        else if (i == 1) { mainCam.SetActive(false); subCam.SetActive(true); }
        else return;
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

    public void BakeTrailRenderersByAvatar(List<TrailRenderer> trails, int avaIndex)
    {
        Debug.Log("Method start");
        //bake trail renderer and add to parent
        foreach (TrailRenderer trail in trails)
        {
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
        BakeTrailRenderersByAvatar(avatar2Trails, 2);
    }

    public void DeleteBakeTrailRenderersByAvatar(int avatarIndex)
    {
        //delete any existing children
        foreach (Transform child in parent[avatarIndex].transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    private void translateBakedTrail(GameObject trailParent, GameObject target, float spd)
    {
        foreach (Transform child in trailParent.transform)
        {
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


    public void TrailBlack()
    {
        foreach (var trail in TrailRenderers)
        {
            trail.material.SetColor("_startColor", Color.black);
            trail.material.SetColor("_endColor", Color.black);
        }
    }

    private void ColorButtonFunction(Button[] buttonlist, List<TrailRenderer> trails, string gridEndColor)
    {
        foreach(var button in buttonlist) {
            button.onClick.AddListener(() =>
            {
                foreach (var trail in trails) { trail.material.SetColor("_endColor", button.colors.normalColor); }
                for (int i = 0; i < buttonlist.Length; i++) { buttonlist[i].GetComponent<Outline>().enabled = false; }
                button.GetComponent<Outline>().enabled = true;
                grids[0].GetComponent<MeshRenderer>().material.SetColor(gridEndColor, button.colors.normalColor);
            });
        }
    }
}