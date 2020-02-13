using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
    GameObject test;
    // Use this for initialization
    void Start () {
        test = GameObject.FindGameObjectWithTag("Finish");
    }
	
	// Update is called once per frame
	void Update() {
        Debug.DrawRay(test.transform.position, test.transform.forward * 1000, Color.green);
    }
}
