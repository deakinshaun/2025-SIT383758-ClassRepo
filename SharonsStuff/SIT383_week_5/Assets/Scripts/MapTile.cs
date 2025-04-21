using System.Net;
using UnityEngine;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

public class MapTile : MonoBehaviour
{

    public Material mapMaterial;

    public MeshFilter objectMesh;

    private static bool TrustCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors ssl)
    {
        return true;
    }

    void Start()
    {
        ServicePointManager.ServerCertificateValidationCallback = TrustCertificate;

        Texture2D textureElevation = retrieveTile("https://s3.amazonaws.com/elevation-tiles-prod/terrarium/", 64, 42, 7);
        Texture2D textureColour = retrieveTile("http://a.tile.thunderforest.com/transport/", 64, 42, 7);
        mapMaterial.mainTexture = textureColour;
        createMesh(512, 512, textureElevation);

    }

    private int indexOf(int ix, int iy, int width)
    {
        return ix + iy * width;
    }
    private int faceIndexOf(int ix, int iy, int width)
    {
        return ix + iy * (width - 1);
    }

    private float getElevation(float u, float v, Texture2D tex)
    {
        Color c = tex.GetPixel((int)(u * tex.width), (int)(v * tex.height));
        float height = ((c.r * 255.0f * 256.0f) + (c.g * 255.0f) + (c.b * 255.0f / 256.0f)) - 32768.0f;
        return height;
    }

    void createMesh(int x, int y, Texture2D texture)
    {
        float scale = 0.1f;
        float vscale = 0.01f;
        Vector3[] vertices = new Vector3[x * y];
        int[] faces = new int[(x - 1) * (y - 1) * 6];
        Vector2[] uvs = new Vector2[x * y];


        for (int ix = 0; ix < x; ix++)
        {
            for (int iy = 0; iy < y; iy++)
            {
                // create a vertex at ix, iy
                float u = (float)ix / (x - 1);
                float v = (float)iy / (y - 1);
                vertices[indexOf(ix, iy, x)] = new Vector3(ix * scale, vscale * getElevation(u, v, texture), iy * scale);
                uvs[indexOf(ix, iy, x)] = new Vector2(u, v);
                if ((ix != x - 1) && (iy != y - 1))
                {
                    faces[faceIndexOf(ix, iy, x) * 6 + 0] = indexOf(ix, iy, x);
                    faces[faceIndexOf(ix, iy, x) * 6 + 1] = indexOf(ix, iy, x) + x;
                    faces[faceIndexOf(ix, iy, x) * 6 + 2] = indexOf(ix, iy, x) + 1;

                    faces[faceIndexOf(ix, iy, x) * 6 + 3] = indexOf(ix, iy, x) + 1;
                    faces[faceIndexOf(ix, iy, x) * 6 + 4] = indexOf(ix, iy, x) + x;
                    faces[faceIndexOf(ix, iy, x) * 6 + 5] = indexOf(ix, iy, x) + 1 + x;
                }
            }
        }

        Mesh m = new Mesh();
        m.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        m.vertices = vertices;
        m.triangles = faces;
        m.uv = uvs;
        m.RecalculateNormals();
        objectMesh.mesh = m;
    }

    public Texture2D retrieveTile(string urlbase, int x, int y, int z)
    {
        //string url = "http://a.tile.thunderforest.com/transport/" + z + "/" + x + "/" + y + ".png";
        //string url = "https://s3.amazonaws.com/elevation-tiles-prod/terrarium/" + z + "/" + x + "/" + y + ".png";
        string url = urlbase + z + "/" + x + "/" + y + ".png";
        WebRequest www = WebRequest.Create(url);
        ((HttpWebRequest)www).UserAgent = "MapTile";
        var response = www.GetResponse();
        Texture2D tex = new Texture2D(2, 2);
        ImageConversion.LoadImage(tex, new BinaryReader(response.GetResponseStream()).ReadBytes(1000000));
        return tex;
    }

    void Update()
    {
        
    }
}
