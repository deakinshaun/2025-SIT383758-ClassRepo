using UnityEngine;

public class ShowPosition : MonoBehaviour
{
    public float r = 1f;
    public GameObject marker;
    public GameObject planet;
    private float rotationSpeed=30f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        planet.transform.rotation *= Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, Vector3.up);
        float lat;
        float lon;
        float alt;

        GetComponent<GPSTracker>().RetriveLocation(out lat, out lon, out alt);
        float p = r * Mathf.Cos(lat);
        float x = p * Mathf.Cos(lon);
        float y = r * Mathf.Sin(lat);
        float z = p * Mathf.Sin(lon);
        marker.transform.SetParent(planet.transform);
        marker.transform.localPosition = new Vector3(x, y, z);
    }
}
