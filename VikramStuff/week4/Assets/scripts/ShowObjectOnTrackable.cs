using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;

public class ShowObjectOnTrackable : MonoBehaviour
{

    public List<GameObject> ObjectsToPlace ;
    private Dictionary<string, GameObject> trackedObjects;
    private ARTrackedImageManager arTrackedImageManager;
    void Awake()
    {
        arTrackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        arTrackedImageManager.trackedImagesChanged += OnImageChange;
    }
    private void OnDisable()
    {
        arTrackedImageManager.trackedImagesChanged -= OnImageChange;
    }
    private void OnImageChange(ARTrackedImagesChangedEventArgs obj)
    {
        Debug.Log("yee");

        foreach(var addedImage in obj.added)
        {
            trackedObjects[addedImage.referenceImage.name].SetActive(true);
        }

        foreach(var updatedImage in obj.updated)
        {
            trackedObjects[updatedImage.referenceImage.name].transform.position = updatedImage.transform.position;
            trackedObjects[updatedImage.referenceImage.name].transform.rotation = updatedImage.transform.rotation;
        }

        foreach (var removedImage in obj.removed)
        {
            trackedObjects[removedImage.referenceImage.name].SetActive(false);
        }
    }

    void Start()
    {
        trackedObjects = new Dictionary<string, GameObject>();
        for(int i = 0; i < arTrackedImageManager.referenceLibrary.count; i++)
        {
            GameObject gameObject = Instantiate(ObjectsToPlace[i]);
            gameObject.SetActive(false);
            trackedObjects[arTrackedImageManager.referenceLibrary[i].name] = gameObject;
        }
    }


    void Update()
    {
        
    }
}
