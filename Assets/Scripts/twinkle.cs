using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class twinkle : MonoBehaviour
{
    public GameObject spike;
    public GameObject star;
    public Color spikeColor;
    public float scaleTimer;
    public float period;
    public bool _scale;
    // Start is called before the first frame update
    void Start()
    {
        spikeColor = Color.white;
        scaleTimer = -1f;
        period = 1f;
        _scale = false;
    }

    // Update is called once per frame
    void Update()
    {
        spikeColor.a = pingPongTwo(-1f, 0.5f,period);
        spike.GetComponent<SpriteRenderer>().color = spikeColor;
        if (_scale == true) {
            scaleDown(0.4f,0.2f,1f);
        }
        starShine();
        //spike.transform.localScale = Vector3.one * spikeScale;
        //star.transform.localScale = Vector3.one * starScale;
    }

    public void scaleDown(float initialScale, float newScale, float timer)
    {
        if(scaleTimer == -1f) {
            scaleTimer = timer;
        }

        float currentScale = Mathf.Lerp( newScale, initialScale, scaleTimer / timer);
        gameObject.transform.localScale = Vector3.one * currentScale;
        if(scaleTimer > 0f) {
            scaleTimer -= Time.deltaTime;
        }
        else { scaleTimer = 0f; _scale=false; }

    }

    public void starShine()
    {
        float power = pingPongTwo(-2f, 2f, 1f);
        star.GetComponent<MeshRenderer>().material.SetFloat("Vector1_e80ff47c9e534fdeb215fa99a5d9fba8", power);
    }

    public float pingPongTwo(float a, float b, float T)
    {
        float d = (a + b) / 2;
        float range = d - a;
        float data = Mathf.PingPong(Time.time * T, range)+d;
        return data;
    }

}
