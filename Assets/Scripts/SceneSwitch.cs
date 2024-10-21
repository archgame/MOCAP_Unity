using Klak.Spout;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
//using static UnityEngine.InputManagerEntry;

public class SceneSwitch : MonoBehaviour
{
    public Volume postProcessVolume;
    private float camExp;

    public GameObject[] cams;
    public SpoutSender[] spoutSenders;
    //Creating data structure
    [SerializeField]
    private Toggle[] Scene0Avatar, Scene1Trace, Scene2Bake, Scene3SharedWorld, Scene4Circular, Scene5Alien,
        Scene6Moon, Scene7Constellation, Scene8BuildGalaxy, Scene9Sprial, Scene10Halo, Scene11Swirl, Scene12GalaxyFull, SceneFinaleZoom,SceneFinaleText, Scene13SharedWorld2;
    private Toggle[][] allScenes;

    //UI
    private Dropdown sceneSwitch;

    //Getting methods from Control
    [SerializeField]
    private GameObject control;
    private Controls mainControl;

    //Specific Control
    //Constellation Auto
    [SerializeField]
    private GameObject LineDraw;
    private ConstellationDrawer CD;
    //Change Bake rate
    [SerializeField]
    private InputField BakeRateInput;
    private float bakeRate;

    private DataSubscription data;

    private int activeIndex;

    //Circular GridSpecific
    [SerializeField]
    private GameObject[] char0, char1;

    private Vector3 camDefaultPos;
    private Vector3 camHighPos;
    private Vector3 camLowPos;

    private SkinnedMeshRenderer[] skins;
    private Material[] skinMtls;
    private GameObject[] sensors;
    private Material[] sensorMtls;
    [SerializeField]
    private Material blackMtl;

    private float finalTimer;
    private float fadeRate;

    private bool allExcpetTextTurnedOff = false;



    private enum Scenes
    {
        Shared_World = 0, 
        Shared_World_Front = 1, 
        Constellation = 2, 
        Avatar= 3, 
        Trace = 4, 
        Moon = 5, 
        Circular_1 = 6, 
        Swirl = 7,
        Galaxy_Full = 8, 
        FinaleZoom = 9,
        FinaleText = 10,
        Spiral = 11,
        Bake = 12,
        Circular_2 = 13,
        Build_Galaxy = 14, 
        Halo = 15
    }


    // Start is called before the first frame update
    void Start()
    {


        skins = FindObjectsOfType<SkinnedMeshRenderer>();
        skinMtls = new Material[skins.Length];
        for (int i = 0; i < skins.Length; i++) {
            skinMtls[i] = skins[i].sharedMaterial;
        }
        sensors = GameObject.FindGameObjectsWithTag("Sensor");
        sensorMtls = new Material[sensors.Length];
        for (int i = 0; i < sensors.Length; i++) {
            sensorMtls[i] = sensors[i].GetComponent<MeshRenderer>().sharedMaterial;
        }

        spoutSenders[0].sourceCamera = cams[0].GetComponent<Camera>();
        spoutSenders[1].sourceCamera = cams[2].GetComponent<Camera>();

        //Record Cam position
        camDefaultPos = cams[1].transform.position;
        Vector3 zoomInVec = new Vector3(0f, 4f, 0f);
        camHighPos = camDefaultPos + zoomInVec;
        camLowPos = camDefaultPos - zoomInVec;



        //layer
        int layerDefault = LayerMask.NameToLayer("Default");
        int layer1Only = LayerMask.NameToLayer("Camera 1 Only");
        int layer2Only = LayerMask.NameToLayer("Camera 2 Only");

        data = GameObject.Find("DataSubscribers").GetComponent<DataSubscription>();
        sceneSwitch = gameObject.GetComponent<Dropdown>();
        Debug.Log("Current value is" + sceneSwitch.value);

        int[] transitionGroup = new int[] { 2, 3, 8 };

        sceneSwitch.onValueChanged.AddListener(Value =>
        {
            CancelInvoke();
            mainControl.DeleteBakeTrailRenderersByAvatar(0);
            mainControl.DeleteBakeTrailRenderersByAvatar(1);
            if (transitionGroup.Contains(Value)) {
                StartCoroutine(WaitAndExecute(Value));
            }
            else {
                SwitchScene(Value);
            }

            if (Value < (int)Scenes.Constellation) { CD.resetDrawing(); cams[1].transform.position = camDefaultPos; cams[0].transform.position = camDefaultPos; }
            if (Value == (int)Scenes.Constellation) { cams[1].transform.position = camHighPos; cams[0].transform.position = camHighPos; }
            if (Value > (int)Scenes.Constellation && Value != (int)Scenes.FinaleText) { CD.resetDrawing(); cams[1].transform.position = camHighPos; cams[0].transform.position = camHighPos; }
            if(Value == (int)Scenes.Trace || Value == (int)Scenes.Bake || Value== (int)Scenes.Build_Galaxy) { cams[1].transform.position = camLowPos; cams[0].transform.position = camLowPos; }
            if (Value == (int)Scenes.Circular_1 || Value == (int)Scenes.Circular_2) { data.gridStretchTime = 2f; }
            if (Value == (int)Scenes.Circular_1) { data.avatar0.jumpCount = 1; data.avatar1.jumpCount = 1; }
            if (Value == (int)Scenes.Shared_World) { foreach (var meshes in char0) { SimpleChildrenLayerChange(meshes, layer1Only) ; } foreach (var meshes in char1) { SimpleChildrenLayerChange(meshes, layer2Only); } }
            if (Value != (int)Scenes.Shared_World) { foreach (var meshes in char0) { SimpleChildrenLayerChange(meshes, layerDefault) ; } foreach (var meshes in char1) { SimpleChildrenLayerChange(meshes, layerDefault); } }

        }
        
        );
        BakeRateInput.onValueChanged.AddListener(Value =>
        {
            CancelInvoke();
            if (sceneSwitch.value == (int)Scenes.Bake) { InvokeRepeating("CallBakeTrail", 0f, float.Parse(Value)); }
        });
        allScenes = new Toggle[][] { Scene0Avatar, Scene1Trace, Scene2Bake, Scene3SharedWorld, Scene4Circular, Scene5Alien,
        Scene6Moon, Scene7Constellation, Scene8BuildGalaxy, Scene9Sprial, Scene10Halo, Scene11Swirl, Scene12GalaxyFull,SceneFinaleZoom, SceneFinaleText,Scene13SharedWorld2 };
        mainControl = control.GetComponent<Controls>();
        CD = LineDraw.GetComponent<ConstellationDrawer>();

        SwitchScene((int)Scenes.Shared_World);

        if (sceneSwitch.value == (int)Scenes.Shared_World) {
            foreach (var meshes in char0) { SimpleChildrenLayerChange(meshes, layer1Only); }
            foreach (var meshes in char1) {
                SimpleChildrenLayerChange(meshes, layer2Only);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        activeIndex = sceneSwitch.value;

        if (Input.GetKeyDown(KeyCode.RightArrow)) { activeIndex++; activeIndex %= 16; }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) { activeIndex--; activeIndex %= 16; }
        if (sceneSwitch.value != activeIndex) { sceneSwitch.value = activeIndex; }
        if (sceneSwitch.value != (int)Scenes.FinaleZoom) {
            foreach (var vfx in data.effects) {
                if (vfx.isActiveAndEnabled) { vfx.SetFloat("_fadeRate", 1); }
            }
        }
        //final scene
        if (sceneSwitch.value == (int)Scenes.FinaleZoom) {
            finalTimer -= Time.deltaTime;
            if (finalTimer > 0f) {
                cams[0].transform.Translate(0, Time.deltaTime / 20f * 10f, 0f, Space.World);
                cams[1].transform.Translate(0, Time.deltaTime / 20f * 10f, 0f, Space.World);
            }
        }
        if (sceneSwitch.value == (int)Scenes.FinaleText) {
            fadeRate -= Time.deltaTime;
            foreach (var vfx in data.effects) {
                if (vfx.isActiveAndEnabled) { vfx.SetFloat("_fadeRate", /*Mathf.Max((finalTimer/20),0)*/fadeRate / 10); }
            }
            if (fadeRate < -2f && !allExcpetTextTurnedOff) {
                TurnOffVisualGroupsExcept(14);
                allExcpetTextTurnedOff = true;
            }
        }
    }

    public void SwitchScene(int i)
    {
        switch (i) {
            //Avatar
            case (int)Scenes.Avatar:
                TurnOnCamera(0,3); TurnOffVisualGroupsExcept(0); TurnOnVisualGroup(0); break;
            //Trace
            case (int)Scenes.Trace:
                TurnOnCamera(0,3); TurnOffVisualGroupsExcept(2); TurnOnVisualGroup(2); break;
            //Bake 
            case (int)Scenes.Bake:
                TurnOnCamera(0,3); TurnOffVisualGroupsExcept(1, 2); TurnOnVisualGroup(2); InvokeRepeating("CallBakeTrail", 0f, 10f); break;
            //SharedWorld
            case (int)Scenes.Shared_World:
                TurnOnCamera(0,1); TurnOffVisualGroupsExcept(3); TurnOnVisualGroup(3); break;
            //Circle Grid
            case (int)Scenes.Circular_1:
                TurnOnCamera(0,1); TurnOffVisualGroupsExcept(4); TurnOnVisualGroup(4); break;
            //Circle Grid
            case (int)Scenes.Circular_2:
                TurnOnCamera(2,3); TurnOffVisualGroupsExcept(4, 5); TurnOnVisualGroup(5); break;
            //Moon 2
            case (int)Scenes.Moon:
                TurnOnCamera(4,5); TurnOffVisualGroupsExcept(6); TurnOnVisualGroup(6); break;
            //Constellation
            case (int)Scenes.Constellation:
                TurnOnCamera(0,1); TurnOffVisualGroupsExcept(7); CD.resetDrawing(); TurnOnVisualGroup(7); break;
            //Build the Sky
            case (int)Scenes.Build_Galaxy:
                TurnOnCamera(0,1); TurnOffVisualGroupsExcept(8); TurnOnVisualGroup(8); break;
            //Spiral
            case (int)Scenes.Spiral:
                TurnOnCamera(0,1); TurnOffVisualGroupsExcept(9); TurnOnVisualGroup(9); break;
            //Halo
            case (int)Scenes.Halo:
                TurnOnCamera(0,1); TurnOffVisualGroupsExcept(10); TurnOnVisualGroup(10); break;
            //Swirl
            case (int)Scenes.Swirl:
                TurnOnCamera(0,1); TurnOffVisualGroupsExcept(11); TurnOnVisualGroup(11); break;
            //Combined Galaxy
            case (int)Scenes.Galaxy_Full:
                TurnOnCamera(0, 1); TurnOffVisualGroupsExcept(8, 9, 10, 11); TurnOnVisualGroup(12); break;
            case (int)Scenes.FinaleZoom:
                finalTimer = 15f; /*data.effects[0].gameObject.SetActive(false) ; data.effects[1].gameObject.SetActive(false);*/ TurnOnVisualGroup(13);  break;

            case (int)Scenes.FinaleText:
                fadeRate = 10f;  allExcpetTextTurnedOff = false; TurnOnVisualGroup(14); break;

            case (int)Scenes.Shared_World_Front:
                TurnOnCamera(2, 3); TurnOffVisualGroupsExcept(15); TurnOnVisualGroup(15); break;

            //Defult
            default: break;
        }
    }
 

    private IEnumerator WaitAndExecute(int caseNum)
    {
        bool sceneSwitched=false;
        postProcessVolume.profile.TryGet(out ColorAdjustments colorLayer);
        colorLayer.postExposure.value = 0f;
        float timer = 5f;
        float oscillatingValue = 0;
        while (timer > 0f) {
            timer -= Time.deltaTime;
            oscillatingValue = Mathf.PingPong(timer / 5 * 2f, 1f) * -10f;
            if(oscillatingValue < -9.9f && !sceneSwitched) {
                SwitchScene(caseNum);
                sceneSwitched = !sceneSwitched;
            }
            colorLayer.postExposure.value = oscillatingValue;
            yield return null;
        }
        colorLayer.postExposure.value = 0f;
    }
    private void SimpleChildrenLayerChange(GameObject go, int layerIn)
    {
        foreach(Transform childs in go.transform) {
            childs.gameObject.layer = layerIn;
        }
    }

    private void TurnOnVisualGroup(int sceneNum )
    {
        if (allScenes[sceneNum] != null) {
            foreach (var tog in allScenes[sceneNum]) {
                if (tog.isOn != true) {
                    tog.isOn = true;
                }
            }

        }
    }

    private void TurnOffVisualGroupsExcept(params int[] sceneNums)
    {

        if (sceneNums == null) { return; }
        else {
            for (int i = 0; i < allScenes.Length; i++) {
                bool match = false;
                for (int j = 0; j < sceneNums.Length; j++) {
                    if (i == sceneNums[j]) {match = true;}
                }
                if (!match && allScenes[i]!=null) { TurnOffVisualGroup(i); }
            }
        }
    }

    private void TurnOffVisualGroup(int sceneNum)
    {
        if (allScenes[sceneNum] != null) {
            foreach (var tog in allScenes[sceneNum]) {
                if (tog.isOn != false) {
                    tog.isOn = false;
                }
            }

        }
    }

    private void CallBakeTrail()
    {
        mainControl.BakeTrail();
    }

    IEnumerator AlienMorphDelaySwitch()
    {
        yield return new WaitForSeconds(3f);
        mainControl.cameraSelect(1);
    }

    private void TurnOnCamera(params int[] index)
    {
        foreach(var cam in cams) {
            cam.SetActive(false);
        }
        for(int i = 0; i < index.Length;i++) {
            if (i <= 1) {
                spoutSenders[i].sourceCamera = cams[index[i]].GetComponent<Camera>();
            }
            cams[index[i]].SetActive(true);
        }
        //foreach (var ind in index) {
        //    if (i == 0) { }
        //    if (i == 1) { spoutSenders[1].sourceCamera = cams[ind].GetComponent<Camera>(); }
        //    cams[ind].SetActive(true);
        //}
    }

    public void ChangeSkinMtl(bool set)
    {
        if (set) {
            foreach (var skin in skins) {
                skin.sharedMaterial = blackMtl;
            }
            foreach (var sensor in sensors) {
                sensor.GetComponent<MeshRenderer>().sharedMaterial = blackMtl;
            }
        }
        else {
            for (int i = 0; i < skins.Length; i++) {
                skins[i].sharedMaterial = skinMtls[i];
            }
            for (int i = 0; i < sensors.Length; i++) {
                sensors[i].GetComponent<MeshRenderer>().sharedMaterial = sensorMtls[i];
            }
        }
    }

    public void SensorOff(bool set)
    {
        if (set) {
            foreach(var sensor in sensors) {
                sensor.SetActive(false);
            }
        }
        else {
            foreach (var sensor in sensors) {
                sensor.SetActive(true);
            }
        }
    }


}
