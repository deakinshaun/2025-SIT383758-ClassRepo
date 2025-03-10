using UnityEngine;
using TMPro;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif
public class camera : MonoBehaviour
{
    public Material boxMaterial;
    private WebCamTexture wcTexture;

    public TextMeshProUGUI outputText;

    private int currentCamera = 0;

    private void showCameras()
    {
        outputText.text = "";
        foreach (WebCamDevice d in WebCamTexture.devices)
        {
            outputText.text += d.name + (d.name == wcTexture?.deviceName ? "*" : "") + "\n";
        }
    }

    public void nextCamera()
    {
        currentCamera = (currentCamera + 1) % WebCamTexture.devices.Length;
        wcTexture.Stop();
        wcTexture.deviceName = WebCamTexture.devices[currentCamera].name;
        wcTexture.Play();
        showCameras();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        wcTexture = new WebCamTexture();
        boxMaterial.mainTexture = wcTexture;
        wcTexture.Play();
    }

    // Update is called once per frame
    void Update()
    {
        showCameras();
        if (wcTexture == null)
        {
            wcTexture = new WebCamTexture();
            #if UNITY_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                wcTexture = null;
            }
            #endif

        }
        if (!wcTexture.isPlaying)
        {
            boxMaterial.mainTexture = wcTexture;
            wcTexture.Play();
        }
    }
}
