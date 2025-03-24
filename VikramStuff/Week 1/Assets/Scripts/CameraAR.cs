using UnityEngine;
using TMPro;
public class CameraAR : MonoBehaviour
{
    private WebCamTexture wcTexture;
    public Material boxMaterial;
    public TMP_Text textBox;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        wcTexture = new WebCamTexture();
        boxMaterial.mainTexture = wcTexture;
        boxMaterial.mainTextureScale = new Vector2(-1, -1);
        wcTexture.Play();
       
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 20 * Time.deltaTime, 0);
    }
}
