using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CameraCapture : MonoBehaviour
{
    public KeyCode screenshotKey;
    public string savePath = "Assets/Resources/Screenshots/";
    public Camera captureCamera;
    public enum ImageFormats {PNG, JPEG};
    public ImageFormats imageFormat = ImageFormats.JPEG;

    void Start()
    {
        if(captureCamera == null)
            captureCamera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        //if (Input.GetKeyDown(screenshotKey))
        //{
        //    Capture();
        //}
    }

    public string Capture()
    {
        RenderTexture activeRenderTexture = RenderTexture.active;
        Debug.Log(captureCamera);

        RenderTexture tempTex = new RenderTexture(captureCamera.pixelWidth, captureCamera.pixelHeight, 16);
        captureCamera.targetTexture = tempTex;
        RenderTexture.active = captureCamera.targetTexture;

        captureCamera.Render();

        Texture2D image = new Texture2D(captureCamera.targetTexture.width, captureCamera.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, captureCamera.targetTexture.width, captureCamera.targetTexture.height), 0, 0);
        image.Apply();

        RenderTexture.active = activeRenderTexture;
        byte[] bytes = null;
        switch (imageFormat)
        {
            case ImageFormats.PNG:
                bytes = image.EncodeToPNG();
                break;
            case ImageFormats.JPEG:
                bytes = image.EncodeToJPG();
                break;
        }
        
        Destroy(image);
        //Destroy(tempTex);

        Debug.Log(bytes);
        string imagePath = Path.Combine(savePath, "output.jpeg");
        File.WriteAllBytes(imagePath, bytes);

        return imagePath;
    }
}