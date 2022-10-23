using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour {

    public Text text;

    public void initText(string str)
    {
        text.text = str;
    }
	
	// Update is called once per frame
	void Update () {
        this.text.transform.Translate(transform.up);
	}
}
