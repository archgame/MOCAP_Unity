using HSVPicker;
using UnityEngine;
namespace HSVPickerExamples
{
    public class ColorPickerTester : MonoBehaviour 
    {

        public ColorPicker picker;
        public TrailRenderer trail;

        public Color Color = Color.red;
        public bool SetColorOnStart = false;

	    // Use this for initialization
	    void Start () 
        {
            picker.onValueChanged.AddListener(color =>
            {
                trail.material.SetColor("_startColor", color);
                Color = color;
            });
            trail.material.SetColor("_startColor", picker.CurrentColor);

            if(SetColorOnStart) 
            {
                picker.CurrentColor = Color;
            }
        }
	
	    // Update is called once per frame
	    void Update () {
	
	    }
    }
}