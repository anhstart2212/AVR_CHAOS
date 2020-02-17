using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour {

    public Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();
    private GameObject currentKey;

    [SerializeField] private Text leftHook, rightHook, centertHook;
    // Use this for initialization
    void Start () {
        keys.Add("Left Hook", KeyCode.Q);
        keys.Add("Right Hook", KeyCode.E);
        keys.Add("Center Hook", KeyCode.LeftShift);
    }
	
	// Update is called once per frame
	void Update() {

    }

    private void OnGUI()
    {
        if (currentKey != null)
        {
            Event e = Event.current;
            if (e.isKey)
            {
                Debug.Log(e.keyCode);
                keys[currentKey.name] = e.keyCode;
                currentKey.transform.GetChild(0).GetComponent<Text>().text = e.keyCode.ToString();
                currentKey = null;
            }
        }
    }

    public void ChangeKey(GameObject clicked)
    {
        currentKey = clicked;
    }
}
