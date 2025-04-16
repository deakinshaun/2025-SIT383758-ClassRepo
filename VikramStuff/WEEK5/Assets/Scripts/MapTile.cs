using UnityEngine;
using System.Net;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System;

public class MapTile : MonoBehaviour
{
    public Material mapMat;

    private static bool TrustCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors ssl)
    {
        return true;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ServicePointManager.ServerCertificateValidationCallback = TrustCertificate;
        CreateMesh(16, 16);
        Texture2D texture = RetrieveTile(64, 42, 7);
        mapMat.mainTexture = texture;
    }
    private int indexOf(int ix, int iy, int width) 
    {
        return ix + iy * width;
    }
    private void CreateMesh(int x, int y)
    {
        float Scale = .1f;
        Vector3[] vertices = new Vector3[x*y];
        float scale = .1f;
        for(int ix = 0; ix < x; x++)
        {
            for(int iy =0; iy < y; iy++)
            {
                vertices[indexOf(ix, iy,x)] = new Vector3(ix * Scale, 0.0f, iy*Scale);
            }
        }
    }

    public Texture2D RetrieveTile(int x, int y, int z)
    {
       /* string url = "http://a.tile.thunderforest.com/transport/" + z + "/" + x + "/" + y + ".png";*/
        string url = "https://s3.amazonaws.com/elevation-tiles-prod/terrarium/" + z + "/" + x + "/" + y + ".png";
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
