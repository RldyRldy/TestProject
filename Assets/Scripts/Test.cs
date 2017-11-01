using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JGame;

public class Test : MonoBehaviour {

    public Text txt;
    public CameraInput camInput = null;

	// Use this for initialization
	void Start () {
		if( camInput == null )
        {
            camInput = Camera.main.GetComponent<CameraInput>();
        }

        txt = GetComponent<Text>();

        var inf = GetComponent<InputField>();

        if( inf != null )
        {
            inf.onEndEdit.AddListener( val => FixSpeed(val) );
        }
	}
	
	// Update is called once per frame
	void Update () {
        txt.text = "Input : " + JInputManager.instance + "\n";
        txt.text += "MoveFactor : " + JInput.buttonDeltaFactor + "\n";
        txt.text += "ScaleFactor : " + JInput.resizeFactor + "\n";
    }

    void FixSpeed( string val )
    {
    }
}
