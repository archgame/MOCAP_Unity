using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneSwitch : MonoBehaviour
{
    [SerializeField]
    private Toggle[] Scene1, Scene2, Scene3, Scene4, Scene5, Scene6, Scene7, Scene8, Scene9, Scene10, Scene11, Scene12;
    private Toggle[][] allScenes;
    private Dropdown sceneSwitch;

    // Start is called before the first frame update
    void Start()
    {
        sceneSwitch = GetComponent<Dropdown>();
        sceneSwitch.onValueChanged.AddListener(Value => SwitchScene(Value+1));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchScene(int i)
    {
        switch (i) {
            //Avatar
            case 1:  
                ToggleVisualGroup(Scene1,true);  break;
            //Moon 1
            case 2:  
                ToggleVisualGroup(Scene2, true);  break;
            //Trace
            case 3:  
                ToggleVisualGroup(Scene3, true);  break;
            //Bake
            case 4:  
                ToggleVisualGroup(Scene4, true);  break;
            //Circle Grid
            case 5:  
                ToggleVisualGroup(Scene5, true); break;
            //Alien Morph
            case 6:  
                ToggleVisualGroup(Scene6, true); break;
            //Moon 2
            case 7:  
                ToggleVisualGroup(Scene7, true); break;
            //Constellation
            case 8:  
                ToggleVisualGroup(Scene8, true); break;
            //Star Trace
            case 9:  
                ToggleVisualGroup(Scene9, true); break;
            //Galaxy
            case 10:  
                ToggleVisualGroup(Scene10, true); break;
            //Halo
            case 11:  
                ToggleVisualGroup(Scene11, true); break;
            //Swirl
            case 12:  
                ToggleVisualGroup(Scene12, true) ; break;
            //Defult
            default: break;
        }
    }

    private void ToggleVisualGroup(Toggle[] togGrp, bool stat)
    {
        if (togGrp != null) { foreach (Toggle toggle in togGrp) { if (toggle.isOn = !stat) { toggle.isOn = stat; } } }
        else return;
    }
}
