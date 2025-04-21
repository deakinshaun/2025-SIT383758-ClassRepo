using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class FoundationStatus : MonoBehaviour
{
    [Tooltip("Provide the AR Session object for this field")]
    public ARInputManager arInputManager;

    public TextMeshProUGUI sessionStatusText;
    public TextMeshProUGUI inputStatusText;

    void Update()
    {
        // Update information from the ARSession object.
        sessionStatusText.text = "State: " + ARSession.state + " - " + ARSession.notTrackingReason;

        // Update information from the ARInputManager object.
        inputStatusText.text = "Input: ";
        if (arInputManager.subsystem != null)
        {
            inputStatusText.text += arInputManager.subsystem.running + " - ";
            List<UnityEngine.XR.InputDevice> devices = new List<UnityEngine.XR.InputDevice>(); arInputManager.subsystem.TryGetInputDevices(devices);
            if (devices.Count > 0)
            {
                Vector3 position;
                Quaternion rotation;
                devices[0].TryGetFeatureValue(UnityEngine.XR.CommonUsages.colorCameraPosition, out position);
                devices[0].TryGetFeatureValue(UnityEngine.XR.CommonUsages.colorCameraRotation, out rotation);
                inputStatusText.text += position.ToString("F4") + ", " + rotation.ToString("F4") + " ";
            }

            // This code is not strictly necessary. However it is handy when encountering
            // a new device, to see what input is available.
            var inputDevices = new List<UnityEngine.XR.InputDevice>();
            UnityEngine.XR.InputDevices.GetDevices(inputDevices);

            foreach (var device in inputDevices)
            {
                inputStatusText.text += string.Format("\nDevice name '{0}' characteristics '{1}': ", device.name, device.characteristics);

                var inputFeatures = new List<UnityEngine.XR.InputFeatureUsage>();
                if (device.TryGetFeatureUsages(inputFeatures))
                {
                    foreach (var feature in inputFeatures)
                    {
                        inputStatusText.text += string.Format("Feature {0} ", feature.name);
                    }
                }
            }

        }
    }
}
