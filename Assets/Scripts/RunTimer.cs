using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HSVPicker;

public class RunTimer : MonoBehaviour
{
    [SerializeField]
    private Toggle[] togGroup1, togGroup2, togGroup3, togGroup4, togGroup5,togGroup6;
    [SerializeField]
    private InputField input1, input2, input3, input4, input5, input6,input7;

    [SerializeField]
    private InputField BakeWait, BakeRate;
    private float bakeWaiteTime, bakeRate;
    [SerializeField]
    private Button button;
    private float timeCount;
    private float time1, time2, time3, time4, time5, time6, time7;
    [SerializeField]
    private bool clicked;


    [SerializeField]
    private GameObject LineDraw;
    private ConstellationDrawer CD;

    private Controls mainControl;
    [SerializeField]
    private GameObject controlGameOBJ;
    private bool isInvoked;

    private bool isCoroutine;
 
    // Start is called before the first frame update
    void Start()
    {
        isCoroutine = false;
        isInvoked = false;
        mainControl = controlGameOBJ.GetComponent<Controls>();
        CD = LineDraw.GetComponent<ConstellationDrawer>();
        clicked = false;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            time1 = float.Parse(input1.text);
            time2 = float.Parse(input2.text);
            time3 = float.Parse(input3.text);
            time4 = float.Parse(input4.text);
            time5 = float.Parse(input5.text);
            time6 = float.Parse(input6.text);
            time7 = float.Parse(input7.text);
            clicked = true;
            Debug.Log("CLicked");
        });


    }

    // Update is called once per frame
    void Update()
    {
        if (clicked == true) {
            //Scene 1
            if (time1 >= 0f) { time1 -= Time.deltaTime; }
            //Scene 2.1 & 2.2
            else if (time1 < 0f & time2 >= 0f) {
                ToggleGroup(togGroup1, true); time2 -= Time.deltaTime;
                bakeWaiteTime = float.Parse(BakeWait.text);
                bakeRate = float.Parse(BakeRate.text);
                if (isInvoked == false) {
                    InvokeRepeating("BakeTrail", bakeWaiteTime, bakeRate);
                    isInvoked = true;
                }
            }
            //Scene 3
            else if (time1 < 0f & time2 < 0f && time3 >= 0f) { ToggleGroup(togGroup1, false); ToggleGroup(togGroup2, true); time3 -= Time.deltaTime; 
                CancelInvoke();
                mainControl.DeleteBakeTrailRenderersByAvatar(0);
                mainControl.DeleteBakeTrailRenderersByAvatar(1);
            }
            //Scene 4 Constellation
            else if (time1 < 0f & time2 < 0f && time3 < 0f && time4 >= 0f) { 
                ToggleGroup(togGroup2, false); 
                ToggleGroup(togGroup3, true); 
                time4 -= Time.deltaTime;
                if (CD.isDrawActive == false &isCoroutine==false) { StartCoroutine(AutoAnotherRounds()); }
            }
            //Scene 5.1
            else if (time1 < 0f & time2 < 0f && time3 < 0f && time4 < 0f && time5 >= 0f && CD.isDrawActive == false) { //ToggleGroup(togGroup3, false);
                ToggleGroup(togGroup1, true);
                ToggleGroup(togGroup4, true); time5 -= Time.deltaTime;
                ColorPicker[] pickers = GameObject.FindObjectsOfType<ColorPicker>(); Debug.Log("pickers: " + pickers.Length);
                foreach (var picker in pickers) {
                    picker.AssignColor(Color.black);
                }
            }
            //Scene 5.2
            else if (time1 < 0f & time2 < 0f && time3 < 0f && time4 < 0f && time5 < 0f && time6 >= 0f) { //ToggleGroup(togGroup4, false); 
                ToggleGroup(togGroup5, true); time6 -= Time.deltaTime;
            }
            //Scene 5.3
            else if (time1 < 0f & time2 < 0f && time3 < 0f && time4 < 0f && time5 < 0f && time6 < 0f && time7 >= 0f) { //ToggleGroup(togGroup5, false); 
                ToggleGroup(togGroup6, true); time7 -= Time.deltaTime;
            }
            
            input1.text = Mathf.Round(time1).ToString();
            input2.text = Mathf.Round(time2).ToString();
            input3.text = Mathf.Round(time3).ToString();
            input4.text = Mathf.Round(time4).ToString();
            input5.text = Mathf.Round(time5).ToString();
            input6.text = Mathf.Round(time6).ToString();
            input7.text = Mathf.Round(time7).ToString();
        }


        
    }

    private void ToggleGroup(Toggle[] togGrp, bool stat)
    {
        foreach(Toggle toggle in togGrp) { toggle.isOn = stat;}
    }

    private void BakeTrail()
    {
        mainControl.BakeTrailRenderersByAvatar(mainControl.avatar0Trails, 0);
        mainControl.BakeTrailRenderersByAvatar(mainControl.avatar1Trails, 1);
    }

    IEnumerator AutoAnotherRounds()
    {
        isCoroutine = true;
        yield return new WaitForSeconds(4);
        CD.anotherRound();
        isCoroutine = false;
    }
}
