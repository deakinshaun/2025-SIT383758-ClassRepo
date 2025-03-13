using UnityEngine;
using TMPro;

public class CameraAR : MonoBehaviour
{
    public Material boxMaterial;
    public TextMeshProUGUI textBox;

    private WebCamTexture wcTexture;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        wcTexture = new WebCamTexture();
        boxMaterial.mainTexture = wcTexture;
        wcTexture.Play();

        textBox.text = "Everything OK";

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
