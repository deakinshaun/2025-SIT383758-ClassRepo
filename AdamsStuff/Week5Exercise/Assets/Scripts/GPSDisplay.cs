using UnityEngine;
using TMPro;

public class GPSDisplay : MonoBehaviour
{
    public TextMeshProUGUI textElement;

    void Start()
    {

    }

    void Update()
    {
        float lat;
        float lon;
        float alt;

        GetComponent<GPSTracking>().retrieveLocation(out lat, out lon, out alt);
        textElement.text = "Latitude: " + lat + ", longitude: " + lon + ", altitude: " + alt;
    }
}