using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JGame;

public class TestBase
{
    public int publicBase = 0;
    protected int protectedBase = 0;
    private int privateBase = 0;
}

namespace GD
{
    public class GD_Test : TestBase
    {
        static public readonly int staticreadonlypublicTest = 0;
        static protected readonly int staticreadonlyprotectedTest = 0;
        static private readonly int staticreadonlyprivateTest = 0;

        public readonly int readonlypublicTest = 0;
        protected readonly int readonlyprotectedTest = 0;
        private readonly int readonlyprivateTest=0;

        public      const int constpublicTest=0;
        protected   const int constprotectedTest=0;
        private     const int constprivateTest=0;

        static public      int staticpublicTest = 0;
        static protected   int staticprotectedTest = 0;
        static private     int staticprivateTest = 0;

        public int publicTest = 0;
        protected int protectedTest = 0;
        private int privateTest = 0;

        public GD_Test()
        {

        }

        public GD_Test(int a, int b)
        {

        }

        private void privateFun()
        {
        }

        protected void protectedFun()
        {
        }

        public void publicFun()
        {
        }
    }
}

namespace UD
{
    public class UD_Test
    {

    }
}

namespace SGD
{
    public class GD_Test
    {

    }
}

namespace SUD
{
    public class UD_Test
    {

    }
}


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
