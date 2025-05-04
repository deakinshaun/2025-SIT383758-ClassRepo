using Unity.Sentis;
using UnityEditor;
using UnityEngine;

namespace Works.Week_6.Scripts.Editor
{
    public class DepthToNormalEditorTool : EditorWindow
    {
        public ModelAsset modelAsset;
        public Texture2D inputTexture;
        public Shader depthToNormalShader;
        public float normalIntensity = 1;
        private RenderTexture depthRT;
        private RenderTexture normalRT;
        private Worker worker;
        private Tensor<float> inputTensor;

        [MenuItem("Tools/Depth to Normal Generator")]
        public static void ShowWindow()
        {
            GetWindow<DepthToNormalEditorTool>("Depth to Normal Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Depth → Normal Map Generator", EditorStyles.boldLabel);

            modelAsset = (ModelAsset)EditorGUILayout.ObjectField("Depth Model", modelAsset, typeof(ModelAsset), false);
            inputTexture =
                (Texture2D)EditorGUILayout.ObjectField("Input Texture", inputTexture, typeof(Texture2D), false);
            depthToNormalShader =
                (Shader)EditorGUILayout.ObjectField("Depth→Normal Shader", depthToNormalShader, typeof(Shader), false);

            normalIntensity = EditorGUILayout.FloatField("Normal Intensity", normalIntensity);
            if (GUILayout.Button("Generate Normal Map"))
            {
                Generate();
            }
            if (normalRT != null)
            {
                GUILayout.Label("Preview:");
                Rect previewRect = GUILayoutUtility.GetRect(256, 256);
                EditorGUI.DrawPreviewTexture(previewRect, normalRT);
            }
            
            if (normalRT != null)
            {
                if (GUILayout.Button("Save Normal Map as PNG"))
                {
                    SaveNormalMapAsPNG();
                }
            }
        }

        private void SaveNormalMapAsPNG()
        {
            RenderTexture.active = normalRT;
            Texture2D tex = new Texture2D(normalRT.width, normalRT.height, TextureFormat.RGBA32, false);
            tex.ReadPixels(new Rect(0, 0, normalRT.width, normalRT.height), 0, 0);
            tex.Apply();
            RenderTexture.active = null;

            string path = EditorUtility.SaveFilePanel("Save Normal Map", Application.dataPath, "normal_map", "png");
            if (!string.IsNullOrEmpty(path))
            {
                System.IO.File.WriteAllBytes(path, tex.EncodeToPNG());
                AssetDatabase.Refresh();
                Debug.Log($"Normal map saved to: {path}");
            }
            DestroyImmediate(tex);
            
        }


        void Generate()
        {
            if (modelAsset == null || inputTexture == null || depthToNormalShader == null)
            {
                Debug.LogError("Missing input fields.");
                return;
            }

            // Prepare
            var model = ModelLoader.Load(modelAsset);
            var graph = new FunctionalGraph();
            var inputs = graph.AddInputs(model);
            var output = Functional.Forward(model, inputs)[0];
            var max0 = Functional.ReduceMax(output, new int[] { 0, 1, 2 }, false);
            var min0 = Functional.ReduceMin(output, new int[] { 0, 1, 2 }, false);
            FunctionalTensor maxmmin = Functional.Sub(max0, min0);
            FunctionalTensor outputmmin = Functional.Sub(output, min0);
            FunctionalTensor output2 = Functional.Div(outputmmin, maxmmin);
            model = graph.Compile(output2);

            // Inference
            worker?.Dispose();
            inputTensor?.Dispose();
            worker = new Worker(model, BackendType.GPUCompute);
            inputTensor = new Tensor<float>(new TensorShape(1, 3, 256, 256), true);

            TextureConverter.ToTensor(inputTexture, inputTensor, new TextureTransform());
            worker.Schedule(inputTensor);
            var tensor = worker.PeekOutput() as Tensor<float>;
            tensor.Reshape(tensor.shape.Unsqueeze(0));

            if (depthRT == null)
                depthRT = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGBFloat);
            TextureConverter.RenderToTexture(tensor, depthRT,
                new TextureTransform().SetCoordOrigin(CoordOrigin.TopLeft));

            // Convert to normal map
            if (normalRT == null)
            {
                normalRT = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGB32);
                normalRT.Create();
            }

            Material mat = new Material(depthToNormalShader);
            mat.SetTexture("_MainTex", depthRT);
            mat.SetFloat("_NormalIntensity", normalIntensity);
            mat.SetVector("_TexelSize", new Vector4(1f / 256f, 1f / 256f, 0, 0));
            Graphics.Blit(null, normalRT, mat);
        }

        private void OnDestroy()
        {
            worker?.Dispose();
            inputTensor?.Dispose();
            depthRT?.Release();
            normalRT?.Release();
        }
    }
}