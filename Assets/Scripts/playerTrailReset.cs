using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerTrailReset : MonoBehaviour
{
    [SerializeField] private bool _isAvatar0;

    [SerializeField] private GameObject _trailResetKey;

    [SerializeField] private Transform _controls;

    private bool _doResetDelay;

    [SerializeField] private float _resetDalay = 2;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == _trailResetKey)
        {
            _doResetDelay = true;

            Invoke("resetDelay", _resetDalay);
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == _trailResetKey)
        {
            _doResetDelay = false;
        }
    }

    void resetDelay()
    {
        if (_doResetDelay == true)
        {
            print("Reset Delay Done");
            if (_isAvatar0 == true)
            {
                _controls.GetComponent<Controls>().resetTrailsAvatar0();
            }
            else
            {
                _controls.GetComponent<Controls>().resetTrailsAvatar1();
            }
        }

        
    }
}
