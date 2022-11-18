using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using ZXing;

public class Reposition : MonoBehaviour
{
    [SerializeField]
    private ARCameraManager cameraManager;
    [SerializeField]
    private ARSession ARSession;
    [SerializeField]
    private ARSessionOrigin ARorigin;

    [SerializeField]
    private List<RoomTargets> QRcodeObjects = new List<RoomTargets>();


    private IBarcodeReader reader = new BarcodeReader();
    private Texture2D m_Texture;


    /* 
     * 
     * Test to check in editor if reposition working
     * 
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetUserLocation("P1160");

        }
    }

    */



    void OnEnable()
    {
        cameraManager.frameReceived += OnCameraFrameReceived;
    }

    void OnDisable()
    {
        cameraManager.frameReceived -= OnCameraFrameReceived;
    }

    void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        if (!cameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
            return;

        var conversionParams = new XRCpuImage.ConversionParams
        {
            // Get the entire image.
            inputRect = new RectInt(0, 0, image.width, image.height),

            // Downsample by 2.
            outputDimensions = new Vector2Int(image.width / 2, image.height / 2),

            // Choose RGBA format.
            outputFormat = TextureFormat.RGBA32,

            // Flip across the vertical axis (mirror image).
            transformation = XRCpuImage.Transformation.MirrorY
        };

        // See how many bytes you need to store the final image.
        int size = image.GetConvertedDataSize(conversionParams);

        // Allocate a buffer to store the image.
        var buffer = new NativeArray<byte>(size, Allocator.Temp);

        // Extract the image data
        image.Convert(conversionParams, buffer);

        // The image was converted to RGBA32 format and written into the provided buffer
        // so you can dispose of the XRCpuImage. You must do this or it will leak resources.
        image.Dispose();

        // At this point, you can process the image, pass it to a computer vision algorithm, etc.
        // In this example, you apply it to a texture to visualize it.

        // You've got the data; let's put it into a texture so you can visualize it.
        m_Texture = new Texture2D(
            conversionParams.outputDimensions.x,
            conversionParams.outputDimensions.y,
            conversionParams.outputFormat,
            false);

        m_Texture.LoadRawTextureData(buffer);
        m_Texture.Apply();

        // Done with your temporary data, so you can dispose it.
        buffer.Dispose();

        //Decoding the qr code and storing as result.

        var result = reader.Decode(m_Texture.GetPixels32(), m_Texture.width, m_Texture.height);

        if(result != null)
        {
            SetUserLocation(result.Text);
        }
       
  
    }


    private void SetUserLocation(string QRtext)
    {
        RoomTargets currentQR = QRcodeObjects.Find(x => x.roomname.Equals(QRtext));
        // reset AR session to original location if qr code is scanned

        if(currentQR != null)
        {
            ARSession.Reset();


            ARorigin.transform.position = currentQR.roomobject.transform.position;
            ARorigin.transform.rotation = currentQR.roomobject.transform.rotation;
        }
       
       

    }





}



