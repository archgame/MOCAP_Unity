using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastAngleBlendShapeMaterial : MonoBehaviour
{
    public GameObject FoxParentGO;
    public GameObject FoxMeshOnly;
    public GameObject TreeMeshOnly;
    public float angle;
    public Material materialFox;
    public Material materialTree;
    public Material materialTrunk;
    public float startAngle = 16;
    public float endAngle = 11;
    public bool showRays = true;
    
    Ray centreLight;
    Ray pointAtAvatar;
    Renderer rend1;
    Renderer rend2;

    // Start is called before the first frame update
    void Start()
    {
        // Set material for Fox
        rend1 = FoxMeshOnly.GetComponent<Renderer> ();
        rend1.material = materialFox;
        rend1.material.SetFloat("_Opacity", 1);

        // Set material for Tree (2 materials)
        rend2 = TreeMeshOnly.GetComponent<Renderer> ();

        rend2.materials[0] = materialTree;
        rend2.materials[0].SetFloat("_Opacity", 0);

        rend2.materials[1] = materialTrunk;
        rend2.materials[1].SetFloat("_Opacity", 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Calculate angle between lantern and avatar
        centreLight.origin = transform.position;
        centreLight.direction = transform.TransformDirection(Vector3.forward);

        pointAtAvatar.origin = transform.position;
        pointAtAvatar.direction = FoxParentGO.transform.position - transform.position;

        if(showRays) {
            Debug.DrawRay(centreLight.origin, centreLight.direction * 20,  Color.blue);
            Debug.DrawRay(pointAtAvatar.origin, pointAtAvatar.direction * 20,  Color.red);
        }
        
        angle = Vector3.Angle((FoxParentGO.transform.position - transform.position), centreLight.direction);

        // Change material accordingly (lerping opacity between the materials)
        if(angle <= startAngle && angle > endAngle) {
            float lerp1 = map(angle, startAngle, endAngle, 1, 0);
            rend1.material.SetFloat("_Opacity", lerp1);
            float lerp2 = map(angle, startAngle, endAngle, 0, 1);
            rend2.materials[0].SetFloat("_Opacity", lerp2);
            rend2.materials[1].SetFloat("_Opacity", lerp2);
        } else if (angle > startAngle) {
            rend1.material.SetFloat("_Opacity", 1);
            rend2.materials[0].SetFloat("_Opacity", 0);
            rend2.materials[1].SetFloat("_Opacity", 0);
        } else if (angle <= endAngle) {
            rend1.material.SetFloat("_Opacity", 0);
            rend2.materials[0].SetFloat("_Opacity", 1);
            rend2.materials[1].SetFloat("_Opacity", 1);
        }

    }

    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s-a1)*(b2-b1)/(a2-a1);
    }
}
