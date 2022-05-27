using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchShader : MonoBehaviour
{

    // public Color defaultColor = new Color(0.1f, 0.1f, 0.1f, 1.0f);
    // public Color colorWhenTouched = Color.cyan;

    public Material matDefault;
    public Material matTouched;
    // public Transform hand1;
    // public Transform hand2;
    GameObject[] hands;
    GameObject[] handsFiltered;

    public Color emitColor = new Color(0, 0, 0);

    float maxDistThreshold = 0.4f;
    public float minDist = 0;
    public float distHand1;
    public float distHand2;
    public float closerHandDist;

    float[] distances;

    float t;
    float startTime;
    float stayOnDuration = 7;
    float fadeDuration = 3;


    Renderer rend;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time - stayOnDuration - fadeDuration*2; // Initiate start time with value out of bounds

        matDefault = Resources.Load("PBR_Tex_Touch_Default", typeof(Material)) as Material;
        matTouched = Resources.Load("PBR_Tex_TouchED", typeof(Material)) as Material;

        hands = GameObject.FindGameObjectsWithTag("Hand");
        int countHands = 0;

        if(hands.Length != 0) {
            for(int i=0; i<hands.Length; i++) {
                if(hands[i].transform.GetChild(0).gameObject.tag != gameObject.tag) {
                    countHands++;
                }
            }
        }

        Debug.Log(countHands);

        handsFiltered = new GameObject[countHands];

        countHands = 0;

        if(hands.Length != 0) {
            for(int i=0; i<hands.Length; i++) {
                if(hands[i].transform.GetChild(0).gameObject.tag != gameObject.tag) {
                    handsFiltered[countHands] = hands[i];
                    countHands++;
                }
            }
        }
        
        //Debug.Log(hands.Length);

        rend = GetComponent<Renderer> ();
        //rend.material.color = defaultColor;
        rend.material = matDefault;

        if(handsFiltered.Length != 0) {
            distances = new float[handsFiltered.Length];
            for(int i=0; i<handsFiltered.Length; i++) {
                distances[i] = 0f;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        t = Time.time;

        if(t >= startTime + stayOnDuration) {
            if( t < startTime + stayOnDuration + fadeDuration) {
                float lerp = map(t, startTime+stayOnDuration, startTime+stayOnDuration+fadeDuration, 0, 1);
                rend.material.Lerp(matTouched, matDefault, lerp); 
            } else {
                rend.material = matDefault;
            }
        }


        if(handsFiltered.Length != 0) {
            for(int i=0; i<handsFiltered.Length; i++) {
                distances[i] = Vector3.Distance(handsFiltered[i].transform.position, transform.position);
            }
        }

        int indexCloserHand = GetIndexOfLowestValue(distances);
        closerHandDist = distances[indexCloserHand];

        // distHand1 = Vector3.Distance(hand1.position, transform.position);
        // distHand2 = Vector3.Distance(hand2.position, transform.position);
        // float closerDist;
        // if(distHand1 > distHand2) {
        //     closerHandDist = distHand2;
        // } else {
        //     closerHandDist = distHand1;
        // }

        if(closerHandDist < maxDistThreshold) {
            emitColor.r = map(closerHandDist, minDist, maxDistThreshold, 1f, 0f);
        } else {
            emitColor.r = 0f;
        }
        
        //Debug.Log(emitColor.r );
        // matDefault.SetColor("_EmitPower", emitColor);
        // rend.material = matDefault;
        rend.material.SetColor("_EmitPower", emitColor);
        
    }

    private void OnTriggerEnter(Collider other)
    {
        startTime = Time.time;
        
        if(other.tag != gameObject.tag) {
            //Debug.Log("Something touched me!");
            //rend.material.color = colorWhenTouched;
            rend.material = matTouched;
        }
    }

    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s-a1)*(b2-b1)/(a2-a1);
    }

    int GetIndexOfLowestValue(float[] arr)
    {
        float value = float.PositiveInfinity;
        int index = -1;
        for(int i = 0; i < arr.Length; i++)
        {
            if(arr[i] < value)
            {
                index = i;
                value = arr[i];
            }
        }
        return index;
    }
}
