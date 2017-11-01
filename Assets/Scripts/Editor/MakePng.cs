using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MakePng : MonoBehaviour
{
    public Animator animator;
    public string animName;
    public string className;
    public string filepath;
    public int nFrame = 4;
    public Color alphacolor;
    protected bool isWorking = false;

    // Use this for initialization
    void Start()
    {
        alphacolor = Camera.main.backgroundColor;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (!isWorking)
            {
                StartCoroutine(MakeAnimFiles());
            }
        }
    }

    public IEnumerator MakeAnimFiles()
    {
        if (animator == null)
        {
            yield break;
        }
        isWorking = true;

        yield return null;


        animator.Play(animName, 0, 1.0f);

        float animTurm = 1.0f / nFrame;

        for (int i = 0; i < nFrame; ++i)
        {
            // file name
            var file = Application.dataPath + "/Units/" + className + "/" + className + "_" + filepath + i + ".png";

            // screen shot
            ScreenCapture.CaptureScreenshot(file);

            yield return new WaitForSeconds(animTurm);
        }

        // fix screenshot files
        for (int i = 0; i < nFrame; ++i)
        {
            // file name
            var file = Application.dataPath + "/Units/" + className + "/" + className + "_" + filepath + i + ".png";

            TextureTransAlpha(file, alphacolor);
        }

        isWorking = false;

        yield return null;
    }

    public void TextureTransAlpha(string filePath, Color alpha )
    {
        // load texture file
        Texture2D newTex = null;

        byte[] fileData;

        // read file
        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            newTex = new Texture2D(2, 2);
            newTex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        else
        {
            Debug.LogError("not exist file");
            return;
        }

        // read pixels
        Color[] pix = newTex.GetPixels();

        // fix pixels. background color -> alpha
        for (int pi = 0; pi < pix.Length; ++pi)
        {
            if (pix[pi].r == alpha.r
                && pix[pi].g == alpha.g
                && pix[pi].b == alpha.b)
            {
                pix[pi] = new Color(1, 1, 1, 0);
            }
        }
        newTex.SetPixels(pix);
        newTex.Apply();

        // save again
        byte[] bytes = newTex.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);
    }
}
