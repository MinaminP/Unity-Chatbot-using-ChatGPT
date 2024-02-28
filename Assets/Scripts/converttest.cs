using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class converttest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void convert()
    {
        File.Move("Assets/Resources/DialogflowV2/certificate222.p12", "Assets/Resources/DialogflowV2/certificate222.bytes");
        Debug.Log("Called");
    }
}
