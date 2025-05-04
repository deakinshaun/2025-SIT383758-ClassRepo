using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class CameraManager : MonoBehaviour
{
    private WebCamTexture wcTexture;
    public Material boxMaterial;
    public Material coldMaterial;
    public Material warmMaterial;
    private Renderer rend;
    private Material[] currentMaterials;
    public Material offMaterial;
    public TextMeshProUGUI textBox;
    public Button offSwitch;
    public Button WarmButton;
    public Button ColdButton;
    public TextMeshProUGUI switchText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rend = GetComponent<Renderer>();
      
        wcTexture = new WebCamTexture();
       
        
            offSwitch.onClick.AddListener(() =>
        {
            if (wcTexture.isPlaying)
            {
                wcTexture.Stop();
                switchText.text = "Turn On";
                boxMaterial.mainTexture = offMaterial.mainTexture;
            }
            else
            {
                //boxMaterial.mainTexture = wcTexture;
                wcTexture.Play();
                boxMaterial.mainTexture = wcTexture;
    
                rend.material = boxMaterial;
                
                
                WarmButton.onClick.AddListener(() =>
                {
                    rend.material.shader = Shader.Find("Universal Render Pipeline/Unlit");
                    rend.material.color = warmMaterial.color;
                    rend.material.mainTexture = wcTexture;
                });

                ColdButton.onClick.AddListener(() =>
                {
                    rend.material.shader = Shader.Find("Universal Render Pipeline/Unlit");
                    rend.material.color = coldMaterial.color;
                    rend.material.mainTexture = wcTexture;
                });
                switchText.text = "Turn Off";
            }
        });
        
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
