using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneSwitch : MonoBehaviour
{
    //Creating data structure
    [SerializeField]
    private Toggle[] Scene0, Scene1, Scene2, Scene3, Scene4, Scene5, Scene6, Scene7, Scene8, Scene9, Scene10, Scene11;
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


    // Start is called before the first frame update
    void Start()
    {
        data = GameObject.Find("DataSubscribers").GetComponent<DataSubscription>();
        sceneSwitch = gameObject.GetComponent<Dropdown>();
        Debug.Log("Current value is" + sceneSwitch.value);
        sceneSwitch.onValueChanged.AddListener(Value => { 
            CancelInvoke(); 
            mainControl.DeleteBakeTrailRenderersByAvatar(0);
            mainControl.DeleteBakeTrailRenderersByAvatar(1);
            SwitchScene(Value);
            if (Value < 7) { CD.resetDrawing(); }
            if (Value > 7) { CD.stopDrawing(); }
            if (Value == 5 || Value ==4) { data.gridStretchTime = 2f; }
            if (Value == 4) { data.avatar0.jumpCount = 1; data.avatar1.jumpCount = 1; }
        }
        );
        BakeRateInput.onValueChanged.AddListener(Value =>
        {
            CancelInvoke();
            if(sceneSwitch.value == 3) { InvokeRepeating("CallBakeTrail", 0f, float.Parse(Value)); }
        });
        allScenes =new Toggle[][] { Scene0, Scene1, Scene2, Scene3, Scene4, Scene5, Scene6, Scene7, Scene8, Scene9, Scene10, Scene11};
        mainControl = control.GetComponent<Controls>();
        CD = LineDraw.GetComponent<ConstellationDrawer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SwitchScene(int i)
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
                 TurnOffVisualGroupsExcept(4,5); TurnOnVisualGroup(5); StartCoroutine(AlienMorphDelaySwitch()); break;
            //Moon 2
            case 6:
                mainControl.cameraSelect(1); TurnOffVisualGroupsExcept(4, 5,6); TurnOnVisualGroup(6); break;
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


}
