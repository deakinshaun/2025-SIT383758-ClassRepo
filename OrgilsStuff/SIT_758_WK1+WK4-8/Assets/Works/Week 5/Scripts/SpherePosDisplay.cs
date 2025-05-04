using UnityEngine;

public class SpherePosDisplay : MonoBehaviour
{
    [SerializeField] private bool rotate;
    [SerializeField] private Transform markerTransform;
    [SerializeField] private Transform sphereTransform;
    [SerializeField] private float sphereRadius;
    void Update()
    {
        GPSTracking.RetrieveLocation(out float lat,out float lon, out float alt);
        markerTransform.parent = sphereTransform;
        markerTransform.localPosition = SphericalToCartesian(lat, lon);
        markerTransform.up = markerTransform.position - sphereTransform.position;

        if (rotate)
        {
            sphereTransform.Rotate(Vector3.up,10*Time.deltaTime);
        }
    }

    private Vector3 SphericalToCartesian(float lat, float lon)
    {
        lat *= Mathf.Deg2Rad;
        lon *= Mathf.Deg2Rad;
        float x = sphereRadius * Mathf.Cos(lat) * Mathf.Cos(lon);
        float y = sphereRadius * Mathf.Sin(lat);
        float z = sphereRadius * Mathf.Cos(lat) * Mathf.Sin(lon);
        return new Vector3(x, y, z);
    }
}
