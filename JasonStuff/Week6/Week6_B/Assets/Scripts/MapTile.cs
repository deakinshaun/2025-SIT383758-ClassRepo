using UnityEngine;
using System.IO;
using System.Net.Security;
using System.Net;

public class MapTile : MonoBehaviour
{
    public Material mapMaterial;

    public MeshFilter objectMesh;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       

    
    }

    private int indexOf(int ix, int iy, int width)
    {
        return ix + iy * width;
    }
    private int faceIndexOf(int ix, int iy, int width)
    {
        return ix + iy * (width - 1);
    }

    

    // Update is called once per frame
    void Update()
    {
        Texture textureElevation = GetComponent<DepCamManager>().getDepthTexture();
        RenderTexture.active = (RenderTexture) textureElevation;
        Texture2D localDepth = new Texture2D(textureElevation.width, textureElevation.height, TextureFormat.RGBA32, false);
        localDepth.ReadPixels(new Rect(0, 0, textureElevation.width, textureElevation.height), 0, 0);
        localDepth.Apply();

        Texture textureColour = GetComponent<DepCamManager>().getColourTexture();
        mapMaterial.mainTexture = textureColour;
        createMesh(512, 512, localDepth);
        Destroy(localDepth);
    }

    void createMesh (int x, int y, Texture2D texture)
    {
        // be aware the different measurement of Vertical scale and Horziontal Scale
        float scale = 0.1f;
        float vscale = 0.0001f;
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
                vertices[indexOf(ix, iy, x)] = new Vector3(ix * scale, vscale * getElevation (u, v, texture), iy * scale);
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

    private float getElevation (float u, float v, Texture2D tex)
    {
        Color c = tex.GetPixel((int) (u * tex.width), (int) (v * tex.height));
        float height = ((c.r * 255.0f * 256.0f) + (c.g * 255.0f) + (c.b * 255.0f / 256.0f)) - 32768.0f;
        return height;
    }

    public Texture2D retrieveTile(string urlbase, int x, int y, int z)
    {
        //string url = "http://a.tile.thunderforest.com/transport/" + z + "/" + x + "/" + y + ".png";
        //string url = "https://s3.amazonaws.com/elevation-tiles-prod/terrarium/" + z + "/" + x + "/" + y + ".png";
        string url = urlbase + z + "/" + x + "/" + y + ".png";
        WebRequest www = WebRequest.Create (url);
        ((HttpWebRequest)www).UserAgent = "MapTile";
        var response = www.GetResponse();
        Texture2D tex = new Texture2D(2, 2);
        ImageConversion.LoadImage (tex, new BinaryReader (response.GetResponseStream()).ReadBytes (1000000));
        return tex;
    }
}
