using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BakeTrailShrinker : MonoBehaviour
{
    [SerializeField] private float _shrinkDelay = 60;
    [SerializeField] private float _shrinkSpeed = 2;

    private bool _doShrink;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("doDrinkSet", _shrinkDelay);
    }

    void doDrinkSet()
    {
        _doShrink = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_doShrink == true)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, transform.localScale / 2, Time.deltaTime * _shrinkSpeed);

            if (transform.localScale.x <= 0.1f)
            {
                Destroy(this.gameObject);
            }
        }
           
    }
}
