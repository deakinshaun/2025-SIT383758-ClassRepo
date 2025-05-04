using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class Item
{
    public string name;
    public Texture2D image;
}
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public List<Item> collectedItems = new();
    private ARTrackedImageManager _arTrackedImageManager;
    public event Action OnItemsUpdated;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _arTrackedImageManager.trackablesChanged.AddListener(OnTrackedImagesChanged);
    }

    private void OnTrackedImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        foreach (var trackedImage in args.added)
        {
            AddItem(new Item{name = trackedImage.name,image = trackedImage.referenceImage.texture});
        }
    }

    public void AddItem(Item item)
    {
        if (!collectedItems.Contains(item))
        {
            collectedItems.Add(item);
            OnItemsUpdated?.Invoke();
            Debug.Log($"Item collected: {item.name}");
        }
    }
    

}
