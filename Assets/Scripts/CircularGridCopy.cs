using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularGridCopy : MonoBehaviour
{
    [SerializeField]
    private GameObject floorGrid;
    private Material lowMat;
    private Material highMat;
    // Start is called before the first frame update
    void Start()
    {
        lowMat = floorGrid.GetComponent<MeshRenderer>().material;
        highMat = GetComponent<MeshRenderer>().material;
        gameObject.SetActive(floorGrid.activeInHierarchy);
    }

    // Update is called once per frame
    void Update()
    {
        float jumpCount0, jumpCount1, jumpCount2, spacing;
        Color color0, color1, color2, outColor0, outColor1, outColor2;
        jumpCount0 = lowMat.GetFloat("_jumpCount0");
        jumpCount1 = lowMat.GetFloat("_jumpCount1");
        jumpCount2 = lowMat.GetFloat("_jumpCount2");
        spacing = lowMat.GetFloat("_spacing");
        color0 = lowMat.GetColor("_ava0GridColor");
        color1 = lowMat.GetColor("_ava1GridColor");
        color2 = lowMat.GetColor("_ava2GridColor");
        outColor0 = lowMat.GetColor("_gradientOut0");
        outColor1 = lowMat.GetColor("_gradientOut1");
        outColor2 = lowMat.GetColor("_gradientOut2");

        highMat.SetFloat("_jumpCount0", jumpCount0);
        highMat.SetFloat("_jumpCount1", jumpCount1);
        highMat.SetFloat("_jumpCount2", jumpCount2);
        highMat.SetFloat("_spacing", spacing);
        highMat.SetColor("_ava0GridColor", color0);
        highMat.SetColor("_ava1GridColor", color1);
        highMat.SetColor("_ava2GridColor", color2);
        highMat.SetColor("_gradientOut0", outColor0);
        highMat.SetColor("_gradientOut1", outColor1);
        highMat.SetColor("_gradientOut2", outColor2);



    }
}
