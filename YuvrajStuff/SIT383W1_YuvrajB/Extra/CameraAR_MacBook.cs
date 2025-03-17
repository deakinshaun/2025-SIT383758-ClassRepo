// using UnityEngine;

// public class CameraAR : MonoBehaviour
// {
//     // Start is called once before the first execution of Update after the MonoBehaviour is created
//     void Start()
//     {

//     }

//     // Update is called once per frame
//     void Update()
//     {

//     }
// }


using UnityEngine;

public class CameraAR : MonoBehaviour
{
    [SerializeField] private GameObject cube; // Drag and drop the cube GameObject here in the Inspector
    [SerializeField] private Material cubeMaterial; // Drag and drop the material here in the Inspector

    void Start()
    {
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
            Debug.LogError("The assigned GameObject does not have a Renderer component.");
        }
    }
}