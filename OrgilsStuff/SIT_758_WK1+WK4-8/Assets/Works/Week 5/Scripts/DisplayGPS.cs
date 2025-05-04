using TMPro;
using UnityEngine;

public class DisplayGPS : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    void Start()
    {
    }

    void Update()
    {
        GPSTracking.RetrieveLocation(out var lat, out var lon, out var alt);
        text.text = $"Latitude:{lat} \nLongitude:{lon} \nAltidude: {alt}";
    }
}