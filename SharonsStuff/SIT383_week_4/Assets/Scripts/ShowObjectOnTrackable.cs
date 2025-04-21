using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;

public class ShowObjectOnTrackable : MonoBehaviour
{
    public List<GameObject> ObjectsToPlace;

    private ARTrackedImageManager arTrackedManager;
    private Dictionary<string, GameObject> trackedObjects;

    private void Awake()
    {
        arTrackedManager = GetComponent<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        arTrackedManager.trackedImagesChanged += OnImageChanged;
    }

    private void OnDisable()
    {
        arTrackedManager.trackedImagesChanged -= OnImageChanged;
    }

    void Start()
    {
        trackedObjects = new Dictionary<string, GameObject>();
        for (int i = 0; i < arTrackedManager.referenceLibrary.count; i++)
        {
            GameObject go = Instantiate(ObjectsToPlace[i]);
            go.SetActive(false);
            trackedObjects[arTrackedManager.referenceLibrary[i].name] = go;
        }
    }

    public void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {

        foreach (var addedImage in args.added)
        {
            Debug.Log("Tracked image added " + addedImage.referenceImage.name + " - " + trackedObjects[addedImage.referenceImage.name]);
            trackedObjects[addedImage.referenceImage.name].SetActive(true);
        }

        foreach (var updatedImage in args.updated)
        {
            trackedObjects[updatedImage.referenceImage.name].SetActive(true);
            trackedObjects[updatedImage.referenceImage.name].transform.position = updatedImage.transform.position;
            trackedObjects[updatedImage.referenceImage.name].transform.rotation = updatedImage.transform.rotation;
            Debug.Log("Tracked image update " + updatedImage.referenceImage.name + " - " + trackedObjects[updatedImage.referenceImage.name] + " : " + trackedObjects[updatedImage.referenceImage.name].transform.position + " - " + trackedObjects[updatedImage.referenceImage.name].activeSelf);
        }

        foreach (var removedImage in args.added)
        {
            trackedObjects[removedImage.referenceImage.name].SetActive(false);
        }
    }

 

    // Update is called once per frame
    void Update()
    {
        
    }
}
