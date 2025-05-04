using Unity.Sentis;
using UnityEngine;


public class DepthCameraManager : MonoBehaviour
{
    public Material colorImageMaterial;
    public Material depthImageMaterial;
    public ModelAsset estimationModel;
    public event System.Action<Texture> onDepthTextureCreated;
    [SerializeField] private Texture _webCamTexture;
    private Worker depthCameraWorker;
    private Tensor<float> inputTensor;
    private RenderTexture depthTexture;

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

    private void GetDepth()
    {
        TextureConverter.ToTensor(_webCamTexture, inputTensor, new TextureTransform());
        depthCameraWorker.Schedule(inputTensor);
        var output = depthCameraWorker.PeekOutput() as Tensor<float>;
        output.Reshape(output.shape.Unsqueeze(0));
        TextureConverter.RenderToTexture(output as Tensor<float>, depthTexture,
            new TextureTransform().SetCoordOrigin(CoordOrigin.TopLeft));
        if (depthImageMaterial != null)
        {
            depthImageMaterial.mainTexture = depthTexture;
        }
        
        onDepthTextureCreated?.Invoke(DepthTexture);
    }

    void Start()
    {
        LoadModel();
        GetDepth();
    }


    private void OnDestroy()
    {
        depthCameraWorker.Dispose();
        inputTensor.Dispose();
        depthTexture.Release();
    }

    public Texture ColourTexture => _webCamTexture;


    public Texture DepthTexture => depthTexture;
}