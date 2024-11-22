using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneSwitch : MonoBehaviour
{

    public GameObject[] cams;

    //Creating data structure
    [SerializeField]
    private Toggle[] Scene0Avatar, Scene1Trace, Scene2Bake, Scene3SharedWorld, Scene4Circular, Scene5Alien,
        Scene6Moon, Scene7Constellation, Scene8BuildGalaxy, Scene9Sprial, Scene10Halo, Scene11Swirl, Scene12GalaxyFull, SceneFinale, Scene13SharedWorld2;
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


    private int activeIndex;

    //Circular GridSpecific
    [SerializeField]
    private GameObject[] char0, char1, char2;

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



        //Record Cam position
        camDefaultPos = cams[1].transform.position;
        Vector3 zoomInVec = new Vector3(0f, 4f, 0f);
        camHighPos = camDefaultPos + zoomInVec;
        camLowPos = camDefaultPos - zoomInVec;


        //layer
        int layerDefault = LayerMask.NameToLayer("Default");
        int layer1Only = LayerMask.NameToLayer("Camera 1 Only"); 
        int layer2Only = LayerMask.NameToLayer("Camera 2 Only");
        sceneSwitch = gameObject.GetComponent<Dropdown>();
        Debug.Log("Current value is" + sceneSwitch.value);
        sceneSwitch.onValueChanged.AddListener(Value =>
        {
            CancelInvoke();
            mainControl.DeleteBakeTrailRenderersByAvatar(0);
            mainControl.DeleteBakeTrailRenderersByAvatar(1);
            mainControl.DeleteBakeTrailRenderersByAvatar(2);
            SwitchScene(Value);
            if (Value < 7) { CD.resetDrawing(); cams[1].transform.position = camDefaultPos; cams[0].transform.position = camDefaultPos; }
            if (Value == 7) { cams[1].transform.position = camHighPos; cams[0].transform.position = camHighPos; }
            if (Value > 7) { CD.resetDrawing(); cams[1].transform.position = camHighPos; cams[0].transform.position = camHighPos; }
            if(Value == 1 || Value == 2 || Value==8) { cams[1].transform.position = camLowPos; cams[0].transform.position = camLowPos; }
            if (Value == 5 || Value == 4) { AvatarsData.gridStretchTime = 2f; }
            if (Value == 4) { AvatarsData.avatar0.jumpCount = 1; AvatarsData.avatar1.jumpCount = 1; AvatarsData.avatar2.jumpCount = 1; }
            if (Value == 3) { foreach (var meshes in char0) { SimpleChildrenLayerChange(meshes, layer1Only) ; } foreach (var meshes in char1) { SimpleChildrenLayerChange(meshes, layer2Only); } }
            if (Value != 3) { foreach (var meshes in char0) { SimpleChildrenLayerChange(meshes, layerDefault) ; } foreach (var meshes in char1) { SimpleChildrenLayerChange(meshes, layerDefault); } }
            if(Value == 13) { finalTimer = 20f; }    
        }
        
        );
        BakeRateInput.onValueChanged.AddListener(Value =>
        {
            CancelInvoke();
            if (sceneSwitch.value == 2) { InvokeRepeating("CallBakeTrail", 0f, float.Parse(Value)); }
        });
        allScenes = new Toggle[][] { Scene0Avatar, Scene1Trace, Scene2Bake, Scene3SharedWorld, Scene4Circular, Scene5Alien,
        Scene6Moon, Scene7Constellation, Scene8BuildGalaxy, Scene9Sprial, Scene10Halo, Scene11Swirl, Scene12GalaxyFull,SceneFinale, Scene13SharedWorld2 };
        mainControl = control.GetComponent<Controls>();
        CD = LineDraw.GetComponent<ConstellationDrawer>();
    }

    // Update is called once per frame
    void Update()
    {
        activeIndex = sceneSwitch.value;
        if (Input.GetKeyDown(KeyCode.RightArrow)) { activeIndex++; activeIndex %= 15; }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) { activeIndex--; activeIndex %= 15; }
        if (sceneSwitch.value != activeIndex) { sceneSwitch.value = activeIndex; }

        //final scene
        if(sceneSwitch.value == 13 && finalTimer>0f) { finalTimer -= Time.deltaTime; cams[0].transform.Translate(0, Time.deltaTime / 20f * 10f, 0f, Space.World); cams[1].transform.Translate(0, Time.deltaTime / 20f * 10f, 0f, Space.World); }
    }

    public void SwitchScene(int i)
    {
        switch (i) {
            //Avatar
            case 0:
                TurnOnCamera(0,3); TurnOffVisualGroupsExcept(0); TurnOnVisualGroup(0); break;
            //Trace
            case 1:
                TurnOnCamera(0,3); TurnOffVisualGroupsExcept(2); TurnOnVisualGroup(2); break;
            //Bake 
            case 2:
                TurnOnCamera(0,3); TurnOffVisualGroupsExcept(1, 2); TurnOnVisualGroup(2); InvokeRepeating("CallBakeTrail", 0f, 10f); break;
            //SharedWorld
            case 3:
                TurnOnCamera(0,1); TurnOffVisualGroupsExcept(3); TurnOnVisualGroup(3); break;
            //Circle Grid
            case 4:
                TurnOnCamera(0,1); TurnOffVisualGroupsExcept(4); TurnOnVisualGroup(4); break;
            //Circle Grid
            case 5:
                TurnOnCamera(2,3); TurnOffVisualGroupsExcept(4, 5); TurnOnVisualGroup(5); break;
            //Moon 2
            case 6:
                TurnOnCamera(4,5); TurnOffVisualGroupsExcept(6); TurnOnVisualGroup(6); break;
            //Constellation
            case 7:
                TurnOnCamera(0,1); TurnOffVisualGroupsExcept(7); CD.resetDrawing(); TurnOnVisualGroup(7); break;
            //Build the Sky
            case 8:
                TurnOnCamera(0,1); TurnOffVisualGroupsExcept(8); TurnOnVisualGroup(8); break;
            //Spiral
            case 9:
                TurnOnCamera(0,1); TurnOffVisualGroupsExcept(9); TurnOnVisualGroup(9); break;
            //Halo
            case 10:
                TurnOnCamera(0,1); TurnOffVisualGroupsExcept(10); TurnOnVisualGroup(10); break;
            //Swirl
            case 11:
                TurnOnCamera(0,1); TurnOffVisualGroupsExcept(11); TurnOnVisualGroup(11); break;
            //Combined Galaxy
            case 12:
                TurnOnCamera(0, 1); TurnOffVisualGroupsExcept(8, 9, 10, 11); TurnOnVisualGroup(12); break;
            case 14:
                TurnOnCamera(2, 3); TurnOffVisualGroupsExcept(14); TurnOnVisualGroup(14); break;

            //Defult
            default: break;
        }
    }
    //archived switches
    /*public void SwitchScene(int i)
    {
        switch (i) {
            //Avatar
            case 0:
                mainControl.cameraSelect(0); TurnOffVisualGroupsExcept(0); TurnOnVisualGroup(0); break;
            //Moon 1
            case 1:
                mainControl.cameraSelect(1); TurnOffVisualGroupsExcept(0,1); TurnOnVisualGroup(1); break;
            //Trace
            case 2:
                mainControl.cameraSelect(0); TurnOffVisualGroupsExcept(2); TurnOnVisualGroup(2); break;
            //Bake 
            case 3:
                mainControl.cameraSelect(0); TurnOffVisualGroupsExcept(2,3); TurnOnVisualGroup(3); InvokeRepeating("CallBakeTrail", 0f, 10f); break;
            //Circle Grid
            case 4:
                mainControl.cameraSelect(0); TurnOffVisualGroupsExcept(4); TurnOnVisualGroup(4); break;
            //Alien Morph
            case 5:
                 TurnOffVisualGroupsExcept(4,5); TurnOnVisualGroup(5); StartCoroutine(AlienMorphDelaySwitch()); mainControl.TrailBlack(); break;
            //Moon 2
            case 6:
                mainControl.cameraSelect(0); TurnOffVisualGroupsExcept(6); TurnOnVisualGroup(6); break;
            //Constellation
            case 7:
                mainControl.cameraSelect(0); TurnOffVisualGroupsExcept(7); CD.resetDrawing(); TurnOnVisualGroup(7); break;
            //Star Trace
            case 8:
                mainControl.cameraSelect(1); TurnOffVisualGroupsExcept( 8); TurnOnVisualGroup(8); break;
            //Galaxy
            case 9:
                mainControl.cameraSelect(0); TurnOffVisualGroupsExcept(8,9);  TurnOnVisualGroup(9); mainControl.TrailBlack(); break;
            //Halo
            case 10:
                mainControl.cameraSelect(0); TurnOffVisualGroupsExcept( 8, 9,10);  TurnOnVisualGroup(10); break;
            //Swirl
            case 11:
                mainControl.cameraSelect(0); TurnOffVisualGroupsExcept( 8, 9, 10,11); TurnOnVisualGroup(11); break;
            //Defult
            default: break;
        }
    }*/


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
        foreach (var ind in index) {
            cams[ind].SetActive(true);
        }
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
