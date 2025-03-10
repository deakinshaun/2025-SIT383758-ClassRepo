using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CameraAR : MonoBehaviour
{
    public Material boxMaterial;
    public Material offMaterial;
    public TextMeshProUGUI textBox;
    public Button offSwitch;
    public TextMeshProUGUI switchText;

    private WebCamTexture wcTexture;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        wcTexture = new WebCamTexture();
        boxMaterial.mainTexture = wcTexture;


        textBox.text = "Welcome to AR!";
        offSwitch.onClick.AddListener(() =>
        {
            if (wcTexture.isPlaying)
            {
                wcTexture.Stop();
                switchText.text = "On";
                boxMaterial.mainTexture = offMaterial.mainTexture;
            }
            else
            {
                boxMaterial.mainTexture = wcTexture;
                wcTexture.Play();
                switchText.text = "Off";
            }
        });
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
