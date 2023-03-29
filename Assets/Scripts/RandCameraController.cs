using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class RandCameraController : MonoBehaviour
{
    private Randomizer_controller randomizer;
    private Vector3 cameraTarget;
    private bool randFlag;
    [Header("Z distance of the camera from the subject. (min, max)")]
    [Space(5)]
    [Tooltip("x: min, y: max")]
    public Vector2 distance = new Vector2 (5.0f, 10.0f);
    [Space(10)]

    [Header("Camera pitch in degrees (wrt the target point). (min, max)")]
    [Space(5)]
    [Tooltip("x: min, y: max")]
    public Vector2 pitch = new Vector2 (0.0f, 30.0f);
    [Space(10)]

    [Header("Camera yaw in degrees (wrt the target point). (min, max)")]
    [Space(5)]
    [Tooltip("x: min, y: max")]
    public Vector2 yaw = new Vector2 (-30.0f, 30.0f);
    [Space(10)]

    [Header("Saved frames resolution. (width, height)")]
    [Space(5)]
    [Tooltip("x: width, y: height")]
    public Vector2Int resolution = new Vector2Int (800, 450);

    // Start is called before the first frame update
    void Start()
    {
        GameObject temp_randomizer = GameObject.Find("Randomizer");
        randomizer = temp_randomizer.GetComponent<Randomizer_controller>();
        randFlag = false;  
        cameraTarget = Vector3.zero;

        setCameraRes(new Vector2Int(resolution[0], resolution[1]));

        Camera Cam = GetComponent<Camera>();
        RenderTexture mRt = new RenderTexture(Cam.targetTexture.width, Cam.targetTexture.height, Cam.targetTexture.depth, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        mRt.antiAliasing = Cam.targetTexture.antiAliasing;
        Cam.targetTexture = mRt;
    }

    void Update()
    {
        Vector3 modelPosition = Vector3.zero;
        if(randFlag)
        {
            if(randomizer.getModel() != null)
                modelPosition = randomizer.getModel().transform.position;
                cameraTarget = modelPosition;

            transform.position = new Vector3(0.0f, 0.0f, Random.Range(distance[0], distance[1])) + modelPosition;
            transform.RotateAround(cameraTarget, Vector3.up, Random.Range(yaw[0], yaw[1]));
            transform.RotateAround(cameraTarget, Vector3.left, Random.Range(pitch[0], pitch[1]));
            Debug.Log("New model loaded");
            randFlag = false;
        }
        transform.LookAt(cameraTarget);
    }

    public bool getRandFlag()
    {
        return randFlag;
    }

    public void setRandFlag(bool newState)
    {
        randFlag = newState;
    }

    private void setCameraRes(Vector2Int newResolution)
    {
        Camera Cam = GetComponent<Camera>();
        RenderTexture rt = new RenderTexture(new RenderTextureDescriptor(newResolution[0], newResolution[1]));
        Cam.targetTexture = rt;
    }

    public Vector2Int getResolution()
    {
        return resolution;
    }

    public Vector2 getZDistance()
    {
        return distance;
    }

    public Vector2 getPitch()
    {
        return pitch;
    }

    public Vector2 getYaw()
    {
        return yaw;
    }

    public void setWidth(int width)
    {
        resolution[0] = width;
        setCameraRes(new Vector2Int(resolution[0], resolution[1]));
    }

    public void setHeight(int height)
    {
        resolution[1] = height;
        setCameraRes(new Vector2Int(resolution[0], resolution[1]));
    }

    public void setZMin(float min)
    {
        distance[0] = min;
    }

    public void setZMax(float max)
    {
        distance[1] = max;
    }

    public void setPitchMin(float min)
    {
        pitch[0] = min;
    }

    public void setPitchMax(float max)
    {
        pitch[1] = max;
    }

    public void setYawMin(float min)
    {
        yaw[0] = min;
    }

    public void setYawMax(float max)
    {
        yaw[1] = max;
    }

    public byte[] CamCapture()
    {
        Camera Cam = GetComponent<Camera>();
 
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = Cam.targetTexture;

        Cam.Render();
 
        Texture2D Image = new Texture2D(Cam.targetTexture.width, Cam.targetTexture.height, TextureFormat.RGBA64, false);
        Image.ReadPixels(new Rect(0, 0, Cam.targetTexture.width, Cam.targetTexture.height), 0, 0);
        RenderTexture.active = currentRT;
 
        var Bytes = Image.EncodeToPNG();
        DestroyImmediate(Image);
 
        return Bytes;
    }
}
