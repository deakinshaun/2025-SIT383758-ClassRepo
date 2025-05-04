using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Works.Week_4.Scripts
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private GameObject itemprefab;

        private void Start()
        {
            PopulateInventoryUI();
            InventoryManager.Instance.OnItemsUpdated += PopulateInventoryUI;
        }

        private void PopulateInventoryUI()
        {
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }

            foreach (var item in InventoryManager.Instance.collectedItems)
            {
                var itemUI = Instantiate(itemprefab, transform);
                itemUI.GetComponentInChildren<TMP_Text>().text = item.name;
                itemUI.GetComponentInChildren<Image>().sprite = Sprite.Create(item.image,
                    new Rect(0.0f, 0.0f, item.image.width, item.image.height), new Vector2(0.5f, 0.5f), 100.0f);
            }
        }
    }
}