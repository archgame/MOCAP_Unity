using HSVPicker;
using UnityEngine;
namespace HSVPickerExamples
{
    public class PickColor : MonoBehaviour
    {

        public ColorPicker startPicker;
        public ColorPicker endPicker;
        private TrailRenderer trail;

        public Color Color1 = Color.red;
        public Color Color2 = Color.red;
        public bool SetColorOnStart = false;

        // Use this for initialization
        void Start()
        {
           
            trail = GetComponent<TrailRenderer>();
            //Gradient start color setup
            startPicker.onValueChanged.AddListener(color =>
            {
                trail.sharedMaterial.SetColor("_startColor", color);
                Color1 = color;
            });
            trail.sharedMaterial.SetColor("_startColor", startPicker.CurrentColor);

            if (SetColorOnStart)
            {
                startPicker.CurrentColor = Color1;
            }

            //Gradient end color setup
            endPicker.onValueChanged.AddListener(color =>
            {
                trail.sharedMaterial.SetColor("_endColor", color);
                Color2 = color;
            });
            trail.sharedMaterial.SetColor("_endColor", endPicker.CurrentColor);

            if (SetColorOnStart)
            {
                endPicker.CurrentColor = Color2;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
