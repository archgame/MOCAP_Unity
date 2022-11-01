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
        float jumpCount0, jumpCount1, spacing;
        Color color0, color1, outColor0, outColor1;
        jumpCount0 = lowMat.GetFloat("_jumpCount0");
        jumpCount1 = lowMat.GetFloat("_jumpCount1");
        spacing = lowMat.GetFloat("_spacing");
        color0 = lowMat.GetColor("_ava0GridColor");
        color1 = lowMat.GetColor("_ava1GridColor");
        outColor0 = lowMat.GetColor("_gradientOut0");
        outColor1 = lowMat.GetColor("_gradientOut1");

        highMat.SetFloat("_jumpCount0", jumpCount0);
        highMat.SetFloat("_jumpCount1", jumpCount1);
        highMat.SetFloat("_spacing", spacing);
        highMat.SetColor("_ava0GridColor", color0);
        highMat.SetColor("_ava1GridColor", color1);
        highMat.SetColor("_gradientOut0", outColor0);
        highMat.SetColor("_gradientOut1", outColor1);



    }
}
