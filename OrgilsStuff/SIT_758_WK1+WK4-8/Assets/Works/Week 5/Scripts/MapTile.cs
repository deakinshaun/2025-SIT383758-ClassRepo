using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

public class MapTile : MonoBehaviour
{
    [Header("Tile Coordinates")] public int tileX;
    public int tileY;
    public int zoom = 7;

    public int tileSize = 256;
    public float heightScale = 10;

    private static bool TrustCertificate(object sender, X509Certificate certificate, X509Chain chain,
        SslPolicyErrors ssl)
    {
        return true;
    }

    private async void Start()
    {
        ServicePointManager.ServerCertificateValidationCallback = TrustCertificate;
        Texture2D heightMap = await RetrieveTile(tileX, tileY, zoom,
            (x, y, z) => $"https://s3.amazonaws.com/elevation-tiles-prod/terrarium/{z}/{x}/{y}.png");
        Texture2D albedo = await RetrieveTile(tileX, tileY, zoom,
            (x, y, z) =>
                $"https://tile.thunderforest.com/transport/{z}/{x}/{y}.png?apikey=f408d0657a7e489a804d738600c35d19");
        
        Color[] heightPixels = heightMap.GetPixels();

        GetComponent<MeshFilter>().mesh = await CreateMeshAsync(heightMap.width, heightMap.height, heightPixels);
        GetComponent<MeshRenderer>().material.mainTexture = albedo;
    }


    private async Task<Mesh> CreateMeshAsync(int width, int height, Color[] heightPixels)
    {
        var vertices = new Vector3[width * height];
        var triangleCount = (width - 1) * (height - 1) * 2;
        var triangles = new int[triangleCount * 3];
        var uvs = new Vector2[width * height];
        var trisIndex = 0;
        await Task.Run(() =>
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var vertIndex = y * width + x;
                    
                    int sampleX = Mathf.FloorToInt(x * (width - 1f) / (width - 1f));
                    int sampleY = Mathf.FloorToInt(y * (height - 1f) / (height - 1f));
                    var c = heightPixels[sampleY * width + sampleX];
                    float elevation = c.r * 255 * 256 + c.g * 255 + c.b * 255 / 256 - 32768;

                    float uX = x / (float)(width - 1);
                    float uY = y / (float)(height - 1);


                    // world position scaled to tileSize
                    float vx = uX * tileSize;
                    float vz = uY * tileSize;

                    vertices[vertIndex] = new Vector3(vx, elevation * heightScale, vz);
                    uvs[vertIndex] = new Vector2(uX, uY);

                    if (x == width - 1 || y == height - 1)
                    {
                        continue;
                    }

                    triangles[trisIndex++] = vertIndex;
                    triangles[trisIndex++] = vertIndex + width;
                    triangles[trisIndex++] = vertIndex + width + 1;
                    triangles[trisIndex++] = vertIndex;
                    triangles[trisIndex++] = vertIndex + width + 1;
                    triangles[trisIndex++] = vertIndex + 1;
                }
            }
        });
        var mesh = new Mesh { vertices = vertices, triangles = triangles, uv = uvs };
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        return mesh;
    }


    private async Task<Texture2D> RetrieveTile(int x, int y, int z, System.Func<int, int, int, string> getUrl)
    {
        var url = getUrl(x, y, z);
        var www = WebRequest.Create(url);
        ((HttpWebRequest)www).UserAgent = "MapTile";
        var response = await www.GetResponseAsync();
        try
        {
            Texture2D tex = new Texture2D(2, 2);

            ImageConversion.LoadImage(tex, new BinaryReader(response.GetResponseStream()).ReadBytes(1_000_000));
            return tex;
        }
        finally
        {
            response.Close();
        }

        return default;
    }
}