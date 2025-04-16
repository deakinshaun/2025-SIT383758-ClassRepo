using UnityEngine;
using TMPro;
public class GPSDisplay : MonoBehaviour
{
    public TMP_Text gpsText;
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

        GetComponent<GPSTracker>().RetriveLocation(out lat, out lon, out alt);
        gpsText.text = "Latitude: " + lat + ", longtitude: " + lon + ", altitude: " + alt;
    }
}
