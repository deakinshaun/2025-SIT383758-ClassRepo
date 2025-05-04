using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Works.Week_4.Scripts
{
    [RequireComponent(typeof(ARTrackedImageManager))]
    public class ShowObjectOnTrackable : MonoBehaviour
    {
        [ Header("The length of this list must match the number of images in Reference Image Library")]
        public List<GameObject> ObjectsToPlace;

        private Dictionary<string, GameObject> allObjects;
        private ARTrackedImageManager arTrackedImageManager;
        private IReferenceImageLibrary refLibrary;
        
        void Awake()
        {
            arTrackedImageManager = GetComponent<ARTrackedImageManager>();
        }

        private void OnEnable()
        {
            arTrackedImageManager.trackedImagesChanged += OnImageChanged;
        }

        private void OnDisable()
        {
            arTrackedImageManager.trackedImagesChanged -= OnImageChanged;
        }


        private void Start()
        {
            refLibrary = arTrackedImageManager.referenceLibrary;

            allObjects = new Dictionary<string, GameObject>();
            for (int i = 0; i < refLibrary.count; i++)

            {
                allObjects.Add(refLibrary[i].name, Instantiate(ObjectsToPlace[i]));
                allObjects[refLibrary[i].name].SetActive(false);
            }
        }

        void ActivateTrackedObject(string _imageName)
        {
            allObjects[_imageName].SetActive(true);
        }


        public void OnImageChanged(ARTrackedImagesChangedEventArgs _args)
        {
            foreach (var addedImage in _args.added)

            {
                allObjects[addedImage.referenceImage.name]?.SetActive(true);
            }

            foreach (var updated in _args.updated)

            {
                allObjects[updated.referenceImage.name].transform.position = updated.transform.position;
                allObjects[updated.referenceImage.name].transform.rotation = updated.transform.rotation;
            }

            foreach (var removedImage in _args.removed)

            {
                allObjects[removedImage.referenceImage.name]?.SetActive(false);
            }
        }
    }
}