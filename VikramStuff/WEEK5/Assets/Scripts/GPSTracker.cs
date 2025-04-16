using UnityEngine;
using UnityEngine.Android;

public class GPSTracker : MonoBehaviour
{

    public void RetriveLocation(out float latitude, out float longitude, out float altitude)
    {
        latitude = 0.0f;
        longitude = 0.0f;
        altitude = 0.0f;
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("location access has not been enabled");
        }
        if (Input.location.status != LocationServiceStatus.Running)
        {
            if (Input.location.status == LocationServiceStatus.Stopped)
            {
                Input.location.Start();
            }
        }

        else
        {
            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;
            altitude = Input.location.lastData.altitude;

        }
    }
    
}
