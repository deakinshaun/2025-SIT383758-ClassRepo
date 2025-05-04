using System;
using System.Collections.Generic;
using UnityEngine;

namespace Works.Week_5.Scripts
{
    public class MapTileGenerator : MonoBehaviour
    {
        [Header("References")] public Transform player;
        public GameObject mapTilePrefab; // prefab with MapTile attached

        [Header("Settings")] public int zoom = 7;
        public int tileRadius = 1;
        public float tileSize = 256f;

        private Vector2Int currentTile;
        private Dictionary<Vector2Int, GameObject> tiles = new Dictionary<Vector2Int, GameObject>();
        private int worldSizeInTiles;


        private void Start()
        {
            if (player == null && Camera.main != null)
                player = Camera.main.transform;

            worldSizeInTiles = 1 << zoom;

            currentTile = WorldPosToTile(player.position);
            UpdateTiles();
        }

        private Vector2Int WorldPosToTile(Vector3 pos)
        {
            int x = Mathf.FloorToInt(pos.x / tileSize);
            int y = Mathf.FloorToInt(-pos.z / tileSize);
            return new Vector2Int(x, y);
        }


        private void Update()
        {
            var newTile = WorldPosToTile(player.position);
            if (newTile != currentTile)
            {
                currentTile = newTile;
                UpdateTiles();
            }
        }

        private void UpdateTiles()
        {
            // destroy out-of-range tiles
            var toRemove = new List<Vector2Int>();
            foreach (var kvp in tiles)
            {
                Vector2Int coord = kvp.Key;
                if (Mathf.Abs(coord.x - currentTile.x) > tileRadius || Mathf.Abs(coord.y - currentTile.y) > tileRadius)
                    toRemove.Add(coord);
            }

            foreach (var coord in toRemove)
            {
                var dest = tiles[coord];
                tiles.Remove(coord);
                Destroy(dest);
            }

            // create missing tiles
            for (int dx = -tileRadius; dx <= tileRadius; dx++)
            {
                for (int dy = -tileRadius; dy <= tileRadius; dy++)
                {
                    Vector2Int coord = currentTile + new Vector2Int(dx, dy);
                    if (!tiles.ContainsKey(coord))
                        CreateTile(coord);
                }
            }
        }

        private void CreateTile(Vector2Int coord)
        {
            // wrap X Y so negatives become valid (world repeats horizontally)
            int wx = ((coord.x % worldSizeInTiles) + worldSizeInTiles) % worldSizeInTiles;
            int wy = ((coord.y % worldSizeInTiles) + worldSizeInTiles) % worldSizeInTiles;

            GameObject go = Instantiate(mapTilePrefab, transform);
            go.transform.position = new Vector3(coord.x * tileSize, 0f, -coord.y * tileSize);
            MapTile mt = go.GetComponent<MapTile>();
            mt.tileX = wx;
            mt.tileY = wy;
            mt.zoom = zoom;
            mt.heightScale = 0.001f * zoom;
            tiles[coord] = go;
        }
    }
}