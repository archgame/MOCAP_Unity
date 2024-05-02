using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class settingSaveManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField trailRenderTime;

    [SerializeField] private TMP_InputField bakedTrailLifeTimeInput;
    [SerializeField] private TMP_InputField bakedTrilShrinkSpeedInput;

    [SerializeField] private GameObject Controls;

    private void Start()
    {
        loadSettings();
    }

    public void loadSettings()
    {
        Controls.GetComponent<Controls>().TrailRenderTime = PlayerPrefs.GetFloat("TrailRenderTime");

        Controls.GetComponent<Controls>()._bakedTrailLifeTime = PlayerPrefs.GetFloat("BakedTrailLifeTime");
        Controls.GetComponent<Controls>()._bakedTrailShrinkSpeed = PlayerPrefs.GetFloat("BakedTrailShrinkSpeed");
    }

    public void saveSettings()
    {
        PlayerPrefs.SetFloat("TrailRenderTime", float.Parse(trailRenderTime.text.ToString()));

        PlayerPrefs.SetFloat("BakedTrailLifeTime", float.Parse(bakedTrailLifeTimeInput.text.ToString()));
        PlayerPrefs.SetFloat("BakedTrailShrinkSpeed", float.Parse(bakedTrilShrinkSpeedInput.text.ToString()));
    }
}