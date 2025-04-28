using UnityEngine;
using Unity.Sentis;
using System;

public class DepthCameraManager : MonoBehaviour
{
    public ModelAsset estimationModel;
    public Worker depthCameraWorker;
    private WebCamTexture webcamTexture;
   
    private Tensor<float> inputTensor;
    private RenderTexture depthTexture;

    private WebCamTexture webCamTexture;
    public Material camMaterial;
    public Material depthMaterial;
    private void LoadModel()
    {
        var model = ModelLoader.Load(estimationModel);
      
        var graph = new FunctionalGraph();
        var inputs = graph.AddInputs(model);
        FunctionalTensor[] outputs = Functional.Forward(model, inputs);
        var output = outputs[0];
        FunctionalTensor max0 = Functional.ReduceMax(output, new int[] { 0, 1, 2 }, false);
        FunctionalTensor min0 = Functional.ReduceMin(output, new int[] { 0, 1, 2 }, false);
        FunctionalTensor maxmmin = Functional.Sub(max0, min0);
        FunctionalTensor outputmmin = Functional.Sub(output, min0);
        FunctionalTensor output2 = Functional.Div(outputmmin, maxmmin);
        model = graph.Compile(output2);

        depthCameraWorker = new Worker(model, BackendType.GPUCompute);
        depthTexture = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGBFloat);
        inputTensor = new Tensor<float>(new TensorShape(1, 3, 256, 256), true);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadModel();
    }
    private void GetDepth()
    {
        TextureConverter.ToTensor(webCamTexture, inputTensor, new TextureTransform());
        depthCameraWorker.Schedule(inputTensor);
        var output = depthCameraWorker.PeekOutput() as Tensor<float>;
        output.Reshape(output.shape.Unsqueeze(0));
        TextureConverter.RenderToTexture(output as Tensor<float>, depthTexture, new TextureTransform().SetCoordOrigin(CoordOrigin.TopLeft));
        depthMaterial.mainTexture = depthTexture;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateWebCam();

        GetDepth();
    }

    private void UpdateWebCam()
    {
        if (webCamTexture == null)
        {
            webCamTexture = new WebCamTexture();
            webCamTexture.Play();
            camMaterial.mainTexture = webCamTexture;
        }
       
    }


    private void OnDestroy()
    {
        depthCameraWorker.Dispose();
        inputTensor.Dispose();
        depthTexture.Release();
    }

    public Texture getColorTexture()
    {
        return webCamTexture;
    }

    public Texture getDepthTexture()
    {
        return depthTexture;
    }
}
