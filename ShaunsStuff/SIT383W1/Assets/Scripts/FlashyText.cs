using UnityEngine;
using TMPro;

public class FlashyText : MonoBehaviour
{
    // This script will change the color of the heading text randomly to make it looks cooler
    // code by Jason 
    public TMP_Text texTmp;
    private float timer = 0f;
   

    
    void Start()
    {
        
    }

   
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 1f)
        {
            SiwtchColor();
            timer = 0f;
        }
       
        
    }

    public void SiwtchColor()
    {
        texTmp.color = new Color(Random.value, Random.value, Random.value);
    }
}
