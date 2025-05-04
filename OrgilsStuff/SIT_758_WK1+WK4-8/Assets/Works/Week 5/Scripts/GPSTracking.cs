using UnityEngine;
using UnityEngine.Android;

public static class GPSTracking 
{
    public static void RetrieveLocation(out float latitude, out float longitude, out float altitute)
    {
        latitude = 0.0f;
        longitude = 0.0f;
        altitute = 0.0f;
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location access has not been enabled");
            return;
        }

        if (Input.location.status != LocationServiceStatus.Running)
        {
            Input.location.Start();
        }
        
        latitude = Input.location.lastData.latitude;
        longitude = Input.location.lastData.longitude;
        altitute = Input.location.lastData.altitude;
    }
}
