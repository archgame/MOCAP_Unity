using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SendEvent : MonoBehaviour
{
    public VisualEffect vfxSS;
    public string event1;
    public VisualEffect vfxSSV1;
    public VisualEffect vfxSSV2;
    public VisualEffect vfxSwirlV1;
    public VisualEffect vfxSwirlV2;
    public float legVeloFactor;
    private Vector3 lastPosition;
    private Vector3 lastHipPosition;
    public VisualEffect vfxColorV1;
    public VisualEffect vfxColorV2;


    // Start is called before the first frame update
    void Start()
    {
        lastPosition = GameObject.Find("mixamorig1:LeftLeg").transform.position;
        lastHipPosition = GameObject.Find("mixamorig1:Hips").transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftAlt) && vfxSS.isActiveAndEnabled)
        {

            vfxSS.SendEvent(event1);
        }

        if (vfxSSV1.isActiveAndEnabled)
        {
            Vector3 Velocity = (GameObject.Find("mixamorig1:Hips").transform.position - lastHipPosition) / Time.deltaTime;
            //Debug.Log("velo = " + Velocity.magnitude);//
            lastHipPosition = GameObject.Find("mixamorig1:Hips").transform.position;
            if (Velocity.magnitude >= 2.5)
            {

                vfxSSV1.SetVector3("moveDir", Velocity.normalized);
                vfxSSV1.SetFloat("handHeight", GameObject.Find("mixamorig1:LeftHand").transform.position.y);
                vfxSSV1.SendEvent(event1);
            }
        }

        if (vfxSSV2.isActiveAndEnabled)
        {
            Vector3 handDist = GameObject.Find("mixamorig1:LeftHand").transform.position - GameObject.Find("mixamorig1:RightHand").transform.position;
            Vector3 legDist = GameObject.Find("mixamorig1:LeftLeg").transform.position - GameObject.Find("mixamorig1:RightLeg").transform.position;
            Debug.Log(handDist);
            Debug.Log(legDist);
            vfxSSV2.SetFloat("spdFactor", legDist.magnitude);
            vfxSSV2.SetFloat("sizeFactor", handDist.magnitude);
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {

                vfxSSV2.SendEvent(event1);
            }
        }


        if (vfxSwirlV1.isActiveAndEnabled)
        {
            Vector3 relativeLoc = GameObject.Find("mixamorig1:LeftHand").transform.position - GameObject.Find("mixamorig1:Hips").transform.position;
            vfxSwirlV1.SetVector3("handHipDist", relativeLoc);
        }

        if (vfxSwirlV2.isActiveAndEnabled)
        {
            Vector3 Velocity = (GameObject.Find("mixamorig1:LeftLeg").transform.position - lastPosition) / Time.deltaTime;
            //Debug.Log("velo = " + Velocity);//
            vfxSwirlV2.SetFloat("swirlForceStrength", Velocity.magnitude * legVeloFactor);
            lastPosition = GameObject.Find("mixamorig1:LeftLeg").transform.position;
        }

        if (vfxColorV1.isActiveAndEnabled)
        {
            Vector3 Velocity = (GameObject.Find("mixamorig1:LeftLeg").transform.position - lastPosition) / Time.deltaTime;
            //Debug.Log("velo = " + Velocity);//
            vfxColorV1.SetFloat("colorRangeMax", Velocity.magnitude * legVeloFactor);
            lastPosition = GameObject.Find("mixamorig1:LeftLeg").transform.position;
        }

        if (vfxColorV2.isActiveAndEnabled)
        {
            Vector3 relativeLoc = GameObject.Find("mixamorig1:LeftHand").transform.position - GameObject.Find("mixamorig1:Hips").transform.position;
            vfxColorV2.SetFloat("handHipDist", relativeLoc.magnitude);
            Vector3 Velocity = (GameObject.Find("mixamorig1:LeftLeg").transform.position - lastPosition) / Time.deltaTime;
            lastPosition = GameObject.Find("mixamorig1:LeftLeg").transform.position;
            //Debug.Log("velo =" + Velocity.magnitude);
            if (Velocity.magnitude >= 5 )
            {

                vfxColorV2.SendEvent("Wave");
            }
        }


    }


}
