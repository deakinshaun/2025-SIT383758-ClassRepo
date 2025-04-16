using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;

public class ShowObjectOnTrackable : MonoBehaviour
{
    [Header("Image Tracking Stuff")]
    public List<GameObject> objectsToSpawn; // Prefabs to spawn based on images
    public TMP_Text debugText;              // For displaying messages
    public LayerMask interactableLayer;     // Only highlight objects on this layer

    [Header("Plane Detection + Spawning Castle")]
    public ARPlaneManager planeManager;           // Detects flat surfaces
    public ARRaycastManager raycastManager;       // Used to shoot rays at flat surfaces
    public Button spawnButton;                    // Button to spawn the castle
    public GameObject castlePrefab;               // Castle to place on tap

    private Dictionary<string, GameObject> trackedObjects = new Dictionary<string, GameObject>();
    private ARTrackedImageManager arTrackedImageManager;
    private GameObject lastSelected;              // For selecting/highlighting objects
    private GameObject spawnedCastle;             // Holds our placed castle instance
    private bool planeDetected = false;           // Track if any flat surface is detected
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    public Slider scaleSlider;
    void Awake()
    {
        // Grab image manager on start
        arTrackedImageManager = GetComponent<ARTrackedImageManager>();
        spawnButton.gameObject.SetActive(false); 
        spawnButton.onClick.AddListener(SpawnCastleOnPlane); 
    }

    void Start()
    {
        // Create all image-spawnable objects, disable them initially
        for (int i = 0; i < arTrackedImageManager.referenceLibrary.count; i++)
        {
            GameObject go = Instantiate(objectsToSpawn[i]);
            go.SetActive(false);
            trackedObjects[arTrackedImageManager.referenceLibrary[i].name] = go;
        }

        scaleSlider.onValueChanged.AddListener(OnScaleChanged);
    }

    void OnEnable()
    {
        arTrackedImageManager.trackedImagesChanged += OnImageChange;
    }

    void OnDisable()
    {
        arTrackedImageManager.trackedImagesChanged -= OnImageChange;
    }

    void Update()
    {
        // Detect if player tapped to highlight tracked objects
        if (Touchscreen.current == null || Touchscreen.current.primaryTouch.press.isPressed == false)
            return;

        if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Vector2 touchPos = Touchscreen.current.primaryTouch.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(touchPos);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, interactableLayer))
            {
                GameObject selectedObject = hit.collider.gameObject;

                // Unhighlight previous one
                if (lastSelected != null && lastSelected != selectedObject)
                    HighlightObject(lastSelected, false);

                // Highlight current
                HighlightObject(selectedObject, true);
                lastSelected = selectedObject;

                debugText.text = "Selected: " + lastSelected.name;
            }
        }

        //  Plane Detection Check (enable spawn button once surface is found)
        foreach (var plane in planeManager.trackables)
        {
            if (plane.alignment == PlaneAlignment.HorizontalUp && plane.trackingState == TrackingState.Tracking)
            {
                if (!planeDetected)
                {
                    planeDetected = true;
                    spawnButton.gameObject.SetActive(true);
                    debugText.text = "Surface found. Tap button to spawn castle!";
                }
                break;
            }
        }
    }

    private void OnImageChange(ARTrackedImagesChangedEventArgs args)
    {
        // When new image is found
        foreach (var trackedImage in args.added)
        {
            SpawnObject(trackedImage);
        }

        // When already scanned image is updated (moved/rotated)
        foreach (var updatedImage in args.updated)
        {
            UpdateObjectTransform(updatedImage);
        }

        // When image is removed/lost
        foreach (var removedImage in args.removed)
        {
            RemovedObjects(removedImage);
        }
    }

    private void SpawnObject(ARTrackedImage trackedImage)
    {
        trackedObjects[trackedImage.referenceImage.name].SetActive(true);
        trackedObjects[trackedImage.referenceImage.name].transform.localScale = Vector3.one;
    }

    private void UpdateObjectTransform(ARTrackedImage updatedImage)
    {
        trackedObjects[updatedImage.referenceImage.name].transform.position = updatedImage.transform.position;
        trackedObjects[updatedImage.referenceImage.name].transform.rotation = updatedImage.transform.rotation;
    }

    private void RemovedObjects(ARTrackedImage removedImage)
    {
        trackedObjects[removedImage.referenceImage.name].SetActive(false);
    }

    private void HighlightObject(GameObject obj, bool highlight)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        if (rend)
        {
            rend.material.color = highlight ? Color.yellow : Color.white;
        }
    }

    // When spawn button is pressed
    private void SpawnCastleOnPlane()
    {
        Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);

        // Raycast to a flat surface
        if (raycastManager.Raycast(center, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            // Only spawn once
            if (spawnedCastle == null)
            {
                spawnedCastle = Instantiate(castlePrefab, hitPose.position, hitPose.rotation);
                debugText.text = "Castle spawned!";
                spawnButton.gameObject.SetActive(false); // Hide the button
               
                planeManager.enabled = false;
                foreach (var plane in planeManager.trackables)  // disable all the planes detected 
                {
                    plane.gameObject.SetActive(false);
                }
            }
        }
    }

    public void OnScaleChanged(float newScale)
    {
        if (spawnedCastle != null)
        {
            spawnedCastle.transform.localScale = Vector3.one * newScale;
        }
    }

}
