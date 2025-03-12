using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class PhysicalCameraTexture : MonoBehaviour
{
    public Material camTexMaterial;
    private WebCamTexture webcamTexture;
    public TextMeshProUGUI outputText;
    private int currentCamera = 0;
    
    private void showCameras()
    {
        outputText.text = "";
        foreach (WebCamDevice d in WebCamTexture.devices)
        {
            outputText.text += "*" + d.name + (d.name == webcamTexture?.deviceName? "" : "") + "\n";
        }
    }

    public void nextCamera()
    {
        currentCamera = (currentCamera + 1) % WebCamTexture.devices.Length;
        // Change camera only works if the camera is stopped.
        webcamTexture.Stop();
        webcamTexture.deviceName = WebCamTexture.devices[currentCamera].name;
        webcamTexture.Play();
        showCameras();
    }
    
    void Update()
{
        showCameras();
        if (webcamTexture == null)
        {
            webcamTexture = new WebCamTexture();
            #if UNITY_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                webcamTexture = null;
            }
            #endif
        }
        if (!webcamTexture.isPlaying)
        {
            camTexMaterial.mainTexture = webcamTexture;
            webcamTexture.Play();
        }
    }
}