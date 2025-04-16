using UnityEngine;

public class Gold : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject gameObject = GameObject.FindGameObjectWithTag("Castel");

        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer rend in renderers)
        {
            foreach (Material mat in rend.materials)
            {
                if (mat.HasProperty("_EmissionColor"))
                {
                    mat.EnableKeyword("_EMISSION");
                    
                    mat.SetColor("_EmissionColor", new Color(1.0f, 0.843f, 0.0f)); 
                }
            }
        }

        Invoke("DestroyObject", 2f);
    }

    void DestroyObject()
    {
        Destroy(gameObject);
    }


}
