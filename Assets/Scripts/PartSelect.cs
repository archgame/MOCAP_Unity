using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartSelect : MonoBehaviour
{
    public GameObject dataSource;
    private DataSubscription data;
    private Dropdown dropDown;
    // Start is called before the first frame update
    void Start()
    {
        data = dataSource.GetComponent<DataSubscription>();
        dropDown = GetComponent<Dropdown>();

    }

    // Update is called once per frame
    void Update()
    {
        data.rigNumber = dropDown.value;
    }
}
