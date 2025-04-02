using UnityEngine;
using UnityEngine.Timeline;

public class ShowPosition : MonoBehaviour
{
    [Tooltip ("Enter the radius of the sphere in this field")]
    public float r = 1.0f;

    [Tooltip ("An object that can be used to show position")]
    public GameObject marker;

    public GameObject planet;
    public float rotationSpeed = 30.0f;

    void Start()
    {
        
    }

    void Update()
    {
        planet.transform.rotation *= Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, Vector3.up);

        float lat;
        float lon;
        float alt;

        GetComponent<GPSTracking>().retrieveLocation(out lat, out lon, out alt);
        float p = r * Mathf.Cos(lat * Mathf.Deg2Rad);
        float x = p * Mathf.Cos(lon * Mathf.Deg2Rad);
        float y = r * Mathf.Sin(lat * Mathf.Deg2Rad);
        float z = p * Mathf.Sin(lon * Mathf.Deg2Rad);
        marker.transform.SetParent(planet.transform);
        marker.transform.localPosition = new Vector3(x, y, z);
    }
}
