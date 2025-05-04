using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.InputSystem;

namespace Works.Week_6.Scripts
{
    public class ObjectScan : MonoBehaviour
    {
        public DepthCameraManager depthCameraManager;
        private Mesh mesh;
        private Vector3[] vertices;
        private Vector2[] uv;

        private int[] triangles;

// Size of Unity mesh is limited, so down sample to fit this limit.
        private const int meshX = 256;
        private const int meshY = 256;
        private double depthScale = 1.0f;
        private float timeDelay = -1.0f;
        private bool timerStarted = false;
        public float timeBeforeCapture = 0.1f;
        private InputSystem_Actions controls;
        private bool touch = false;

        private void touched(InputAction.CallbackContext context)
        {
            touch = true;
        }

        void Start()
        {
            controls = new InputSystem_Actions();
            controls.Player.Attack.performed += context => touched(context);
            controls.Enable();
            CreateMesh(meshX, meshY);
            StartCoroutine(updateMesh());
        }

        void CreateMesh(int width, int height)
        {
            mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            GetComponent<MeshFilter>().mesh = mesh;
            vertices = new Vector3[width * height];
            uv = new Vector2[width * height];
            triangles = new int[6 * ((width - 1) * (height - 1))];
            int triangleIndex = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = (y * width) + x;
                    float fx = (float)x / (float)(width - 1);
                    float fy = (float)y / (float)(height - 1);
                    vertices[index] = new Vector3(fx - 0.5f, 0.5f - fy, 0);
                    uv[index] = new Vector2(((float)x / (float)width), ((float)y / (float)height));
// Skip the last row/col
                    if (x != (width - 1) && y != (height - 1))
                    {
                        int topLeft = index;
                        int topRight = topLeft + 1;
                        int bottomLeft = topLeft + width;
                        int bottomRight = bottomLeft + 1;
                        triangles[triangleIndex++] = topLeft;
                        triangles[triangleIndex++] = topRight;
                        triangles[triangleIndex++] = bottomLeft;
                        triangles[triangleIndex++] = bottomLeft;
                        triangles[triangleIndex++] = topRight;
                        triangles[triangleIndex++] = bottomRight;
                    }
                }
            }

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
        }

        IEnumerator updateMesh()
        {
            yield return new WaitForEndOfFrame();
            gameObject.GetComponent<MeshRenderer>().material.mainTexture = depthCameraManager.ColourTexture;
            RefreshData(depthCameraManager.DepthTexture);
            if (touch)
            {
                touch = false;
                timeDelay = timeBeforeCapture;
                timerStarted = true;
            }

            if (timeDelay > 0.0f)
            {
                timeDelay -= Time.deltaTime;
            }

            if (timerStarted && (timeDelay < 0.0f))
            {
                timerStarted = false;

                // For continuous capture. Recommend timeBeforeCapture of at least 1 s.
                //timeDelay = timeBeforeCapture;
            }


            //timerStarted = true;

            int count = 0;

            string fn = "";
            do
            {
                fn = Application.persistentDataPath + "/scan" + count.ToString("D4");
                count++;
            } while (File.Exists(fn + ".obj"));

            Debug.Log("Exporting: " + fn);
            exportObj(fn);
            // EditorApplication.Beep();


            yield return null; // wait until next frame at least.
        }

        private void RefreshData(Texture depthData)
        {
            RenderTexture.active = (RenderTexture)depthData;
            Texture2D readTex = new Texture2D(depthData.width, depthData.height, TextureFormat.RGBA32, false);
            readTex.ReadPixels(new Rect(0, 0, depthData.width, depthData.height), 0, 0);
            readTex.Apply();

            int xsteps = Mathf.Max(readTex.width, meshX); // deal with cases where mesh > texture or vice versa
            int ysteps = Mathf.Max(readTex.height, meshY);
            int xmstep = Mathf.Max(readTex.width / meshX, 1); // step in the mesh vertices
            int ymstep = Mathf.Max(readTex.height / meshY, 1);
            int xistep = Mathf.Max(meshX / readTex.width, 1); // step in the texture
            int yistep = Mathf.Max(meshY / readTex.height, 1);
            for (int ystep = 0; ystep < ysteps; ystep++)
            {
                for (int xstep = 0; xstep < xsteps; xstep++)
                {
                    int smallIndex = (ystep / ymstep) * meshX + (xstep / xmstep);
                    double avg = 1.0f - GetAvg(readTex, xstep / xistep, ystep / yistep, xistep, yistep);
                    ;
                    avg = avg * depthScale;
                    vertices[smallIndex].z = (float)avg;

                    // Update UV mapping with CDRP
                    uv[smallIndex] = new Vector2((float)(xstep / xmstep) / (float)meshX,
                        (float)(ystep / ymstep) / (float)meshY);
                    smallIndex++;
                }
            }

            Destroy(readTex);

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
        }

        private float GetAvg(Texture2D depthData, int x, int y, int width, int height)
        {
            float sum = 0.0f;
            int count = 0;
            for (int y1 = y; y1 < y + height; y1++)
            {
                for (int x1 = x; x1 < x + width; x1++)
                {
                    if ((x1 < depthData.width) && (y1 < depthData.height))
                    {
                        sum += depthData.GetPixel(x1, y1).r;
                        count++;
                    }
                }
            }

            return sum / count;
        }

        private Texture2D getTexture(Texture tex)
        {
            Texture2D image = new Texture2D(tex.width, tex.height, TextureFormat.RGB24, false);
            RenderTexture renderTexture = new RenderTexture(tex.width, tex.height, 24);
            Graphics.Blit(tex, renderTexture);
            RenderTexture.active = renderTexture;
            image.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            image.Apply();
            Destroy(renderTexture);
            return image;
        }

        void exportObj(string filenameBase)
        {
            Debug.Log("Saving scan");

            // Write obj file
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filenameBase + ".obj"))
            {
                file.WriteLine("mtllib " + filenameBase + ".mtl");
                file.WriteLine("o mesh");
                foreach (Vector3 v in vertices)
                {
                    file.WriteLine("v " + v.x + " " + v.y + " " + v.z);
                }

                foreach (Vector2 v in uv)
                {
                    file.WriteLine("vt " + v.x + " " + v.y);
                }

                foreach (Vector3 v in mesh.normals)
                {
                    file.WriteLine("vn " + v.x + " " + v.y + " " + v.z);
                }

                file.WriteLine("usemtl meshMaterial");
                file.WriteLine("s off");
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    file.WriteLine("f " + (1 + triangles[i + 0]) + "/" + (1 + triangles[i + 0]) + "/" +
                                   (1 + triangles[i + 0]) +
                                   " " +
                                   (1 + triangles[i + 1]) + "/" + (1 + triangles[i + 1]) + "/" +
                                   (1 + triangles[i + 1]) +
                                   " " +
                                   (1 + triangles[i + 2]) + "/" + (1 + triangles[i + 2]) + "/" +
                                   (1 + triangles[i + 2]));
                }
            }

            // Write mtl fie

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filenameBase + ".mtl"))
            {
                file.WriteLine("newmtl meshMaterial");
                file.WriteLine("o mesh");
                file.WriteLine("Ns 96.078431");
                file.WriteLine("Ka 1.000000 1.000000 1.000000");
                file.WriteLine("Kd 0.640000 0.640000 0.640000");
                file.WriteLine("Ks 0.500000 0.500000 0.500000");
                file.WriteLine("Ke 0.000000 0.000000 0.000000");
                file.WriteLine("Ni 1.000000");
                file.WriteLine("d 1.000000");
                file.WriteLine("illum 2");
                file.WriteLine("map_Kd " + filenameBase + ".jpg");
            }

            Texture2D tex = getTexture(depthCameraManager.ColourTexture);
            File.WriteAllBytes(filenameBase + ".jpg", ImageConversion.EncodeToJPG(tex));
            Destroy(tex);
        }
    }
}