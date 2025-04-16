using UnityEngine;
using System.Net;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System;

public class MapTile : MonoBehaviour
{
    public Material mapMat;
    public MeshFilter objectMesh;
    private static bool TrustCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors ssl)
    {
        return true;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ServicePointManager.ServerCertificateValidationCallback = TrustCertificate;
        // string url = "http://a.tile.thunderforest.com/transport/" + z + "/" + x + "/" + y + ".png";
        // string url = "https://s3.amazonaws.com/elevation-tiles-prod/terrarium/" + z + "/" + x + "/" + y + ".png";

        Texture2D textureElevation = RetrieveTile("https://s3.amazonaws.com/elevation-tiles-prod/terrarium/",64, 42, 7);
        Texture2D textureColour = RetrieveTile("http://a.tile.thunderforest.com/transport/", 64, 42, 7);
        mapMat.mainTexture = textureColour;
        CreateMesh(128, 128,textureElevation);
    }
    private int indexOf(int ix, int iy, int width) 
    {
        return ix + iy * width;
    }
    private int faceIndexOf(int ix, int iy, int width)
    {
        return ix + iy * (width-1);
    }

    private float GetElevation(float u, float v, Texture2D tex)
    {
        /*Color c = tex.GetPixel((int)u * tex.width, (int)v * tex.height);
        float height = ((c.r*255.0f*256.0f)+(c.g*255.0f)+(c.b*255.0f/256.0f))-32768.0f;
        Debug.Log(height);
        return height;
        Because u and v are in [0,1] range, 
        and it is multiplying them directly without Mathf.Floor or rounding — but most importantly,
        u * tex.width can give us values equal to tex.width, which is out of bounds.
        we need to clamp or floor the pixel position because texture coordinates can be exactly 1.0, 
        which is outside the index range (0 -> width-1).
        */

        int px = Mathf.Clamp((int)(u * (tex.width - 1)), 0, tex.width - 1);
        int py = Mathf.Clamp((int)(v * (tex.height - 1)), 0, tex.height - 1);
        Color c = tex.GetPixel(px, py);
        float height = ((c.r * 255f * 256f) + (c.g * 255f) + (c.b * 255f / 256f)) - 32768f;
        return height; // Scale down to Unity units if needed
    }
    private void CreateMesh(int x, int y,Texture2D texture)
    {
        float Scale = .1f;
        float vScale = .005f;
        Vector3[] vertices = new Vector3[x*y];
        int[] faces = new int[(x - 1) * (y - 1) * 6];
        Vector2[] uvs = new Vector2[x * y];

        for(int ix = 0; ix < x; ix++)
        {
            for(int iy =0; iy < y; iy++)
            {
                float u = (float)ix / (x - 1);
                float v = (float)iy / (y - 1);
                vertices[indexOf(ix, iy,x)] = new Vector3(ix * Scale, vScale*GetElevation(u,v,texture), iy*Scale);

                uvs[indexOf(ix, iy, x)] = new Vector2(u,v);
                if ((ix != x - 1)&& (iy != y - 1))
                {
                    faces[faceIndexOf(ix, iy, x) * 6 + 0] = indexOf(ix, iy, x);
                    faces[faceIndexOf(ix, iy, x) * 6 + 1] = indexOf(ix, iy, x)+x;
                    faces[faceIndexOf(ix, iy, x) * 6 + 2] = indexOf(ix, iy, x) + 1;

                    faces[faceIndexOf(ix, iy, x) * 6 + 3] = indexOf(ix, iy, x)+1;
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

    public Texture2D RetrieveTile(string urlBase,int x, int y, int z)
    {

        string url = urlBase+z+"/"+x+"/" + y + ".png";
        WebRequest www =  WebRequest.Create(url);
        ((HttpWebRequest)www).UserAgent = "MapTile";
        var response = www.GetResponse();
        Texture2D tex = new Texture2D(2, 2);
        ImageConversion.LoadImage(tex, new BinaryReader(response.GetResponseStream()).ReadBytes(100000));
        return tex;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
