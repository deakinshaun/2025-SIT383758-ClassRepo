using UnityEngine;
using TMPro;

public class GPSDisplay : MonoBehaviour
{
    public TextMeshProUGUI textElement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float lat;
        float lon;
        float alt;

        GetComponent<GPSTracking>().retrieveLocation(out lat, out lon, out alt);
        textElement.text = "Latitude: " + lat + ", longitude: " + lon + ", altitude: " + alt;
    }
}
