using UnityEngine;

public class CaptureIcon : MonoBehaviour
{
    [SerializeField] Camera myCamera;
    [SerializeField] GameObject myTarget;
    [SerializeField] int imageSize = 256;
    [SerializeField] string fileName = "";

    private GameObject lastTarget;

    public bool capture = false;
    public bool recenter = false;
    public bool changeTarget = false;

    void Start()
    {
        ChangeTarget();
    }

    private void Update()
    {
        if (changeTarget)
        {
            changeTarget = false;
            ChangeTarget();
        }

        if (recenter)
        {
            recenter = false;
            Recenter();
        }

        if (capture)
        {
            capture = false;
            CaptureScreenshot();
        }
    }

    void ChangeTarget()
    {
        if (lastTarget != null)
        {
            lastTarget.SetActive(false);
        }

        if (myTarget != null)
        {
            fileName = myTarget.name;

            myTarget.SetActive(true);
            Recenter();
        }

        lastTarget = myTarget;
    }

    void Recenter()
    {
        if (myTarget == null)
        {
            return;
        }

        Vector3 centerPosition = myTarget.GetComponentInChildren<Renderer>().bounds.center;
        myCamera.transform.position = new Vector3(centerPosition.x, centerPosition.y, myCamera.transform.position.z);
    }

    void CaptureScreenshot()
    {
        // myCamera.clearFlags = CameraClearFlags.SolidColor;
        // myCamera.backgroundColor = new Color(0, 0, 0, 0);
        RenderTexture rt = new RenderTexture(imageSize, imageSize, 32, RenderTextureFormat.ARGB32);
        myCamera.targetTexture = rt;
        myCamera.Render();
        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D(imageSize, imageSize, TextureFormat.RGBA32, false);
        screenShot.ReadPixels(new Rect(0, 0, imageSize, imageSize), 0, 0);
        screenShot.Apply();

        RenderTexture.active = null;
        myCamera.targetTexture = null;
        Destroy(rt);

        // result = Sprite.Create(screenShot, new Rect(Vector2.zero, new Vector2(imageSize, imageSize)), Vector2.zero);

        byte[] bytes = screenShot.EncodeToPNG();
        string filename = Application.dataPath + "/Art/Icons/" + fileName + ".png";
        System.IO.File.WriteAllBytes(filename, bytes);

        Debug.Log(string.Format("Took screenshot to: {0}", filename));
    }
}
