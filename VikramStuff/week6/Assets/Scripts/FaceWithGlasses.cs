using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

public class FaceWithGlasses : MonoBehaviour
{
    public ARFaceManager faceManager;
    public Material transparentFaceMaterial; 
    public GameObject glassesPrefab;
    public TMP_Text debug; 
    private Dictionary<TrackableId, GameObject> spawnedGlasses = new Dictionary<TrackableId, GameObject>();

    void OnEnable()
    {
        faceManager.facesChanged += OnFacesChanged;
    }

    void OnDisable()
    {
        faceManager.facesChanged -= OnFacesChanged;
    }

    void OnFacesChanged(ARFacesChangedEventArgs args)
    {
        foreach (var face in args.added)
        {
            Debug.Log("Face detected - spawning glasses");

            var meshRenderer = face.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.material = transparentFaceMaterial;
                Debug.Log("Face mesh material changed");
            }

            GameObject glasses = Instantiate(glassesPrefab, face.transform);

          
            Vector3 offset = new Vector3(0.04f, 0.015f, -0.03f);
            glasses.transform.localPosition = offset;
          

            
            glasses.transform.localScale = new Vector3(0.045f, 0.045f, 0.045f);

            Debug.Log("Glasses spawned at position: " + offset);

            spawnedGlasses[face.trackableId] = glasses;
        }

        foreach (var face in args.removed)
        {
            if (spawnedGlasses.TryGetValue(face.trackableId, out GameObject glasses))
            {
                Destroy(glasses);
                spawnedGlasses.Remove(face.trackableId);
                Debug.Log("Glasses destroyed");
            }
        }
    }

}
