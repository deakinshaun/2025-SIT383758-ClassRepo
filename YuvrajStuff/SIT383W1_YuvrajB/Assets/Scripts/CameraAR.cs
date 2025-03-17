using UnityEngine;
using TMPro; 

public class CameraAR : MonoBehaviour
{
    public GameObject cube; 
    public Material cubeMaterial; 
    public TextMeshProUGUI statusText; 

    void Start()
    {

        if (statusText != null)
        {
            statusText.text = "Everything ok"; 
        }
        else
        {
            Debug.LogWarning("Status Text is not assigned in the Inspector!");
        }

        // Apply the material to the cube
        if (cube != null && cubeMaterial != null)
        {
            ApplyMaterial();
        }
        else
        {
            Debug.LogWarning("Cube or Material is not assigned in the Inspector!");
        }
    }

    void ApplyMaterial()
    {
        Renderer renderer = cube.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = cubeMaterial;
        }
        else
        {
            Debug.LogError("The assigned Cube GameObject does not have a Renderer component.");
        }
    }
}
