using UnityEngine;

public class Tree : MonoBehaviour
{
    private void Start()
    {
        GameObject gameObject = GameObject.FindGameObjectWithTag("Castel");
        foreach (Transform child in gameObject.transform)
        {
            if (!child.gameObject.activeSelf)
            {
                child.gameObject.SetActive(true);
            }
            
            
        }

        Invoke("DestroyObject", 2f);

        
    }

    void DestroyObject()
    {
        Destroy(gameObject);
    }
}
