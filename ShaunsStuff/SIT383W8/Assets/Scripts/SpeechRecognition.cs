using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.Collections;
using Unity.Sentis;
using UnityEngine;

public class SpeechRecognition : MonoBehaviour
{
    public int recordDuration = 5;

    public TextMeshProUGUI outputTextDisplay;

    public ModelAsset decoder;
    public ModelAsset decoderWithPast;
    public ModelAsset encoder;
    public ModelAsset logMelSpectro;

    public TextAsset jsonFile;

    public int maxTokens = 100;

    private Worker decoderWorker;
    private Worker decoderWithPastWorker;
    private Worker encoderWorker;
    private Worker argmaxWorker;
    private Worker logMelSpectroWorker;

    private int[] whiteSpaceCharacters = new int[256];
    private Tensor<int> tokensTensor;
    private Tensor<int> lastTokenTensor;
    private NativeArray<int> lastToken;
    private string[] tokens;

    const int END_OF_TEXT = 50257;
    const int START_OF_TRANSCRIPT = 50258;
    const int ENGLISH = 50259;
    const int GERMAN = 50261;
    const int FRENCH = 50265;
    const int TRANSCRIBE = 50359; //for speech-to-text in specified language
    const int TRANSLATE = 50358;  //for speech-to-text then translate to English
    const int NO_TIME_STAMPS = 50363;
    const int START_TIME = 50364;

    void GetTokens()
    {
        var vocab = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, int>>(jsonFile.text);
        tokens = new string[vocab.Count];
        foreach (var item in vocab)
        {
            tokens[item.Value] = item.Key;
        }
    }

    string GetUnicodeText(string text)
    {
        var bytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(ShiftCharacterDown(text));
        return Encoding.UTF8.GetString(bytes);
    }

    string ShiftCharacterDown(string text)
    {
        string outText = "";
        foreach (char letter in text)
        {
            outText += ((int)letter <= 256) ? letter :
                (char)whiteSpaceCharacters[(int)(letter - 256)];
        }
        return outText;
    }

    void SetupWhiteSpaceShifts()
    {
        for (int i = 0, n = 0; i < 256; i++)
        {
            if (IsWhiteSpace((char)i)) whiteSpaceCharacters[n++] = i;
        }
    }

    bool IsWhiteSpace(char c)
    {
        return !(('!' <= c && c <= '~') || ('�' <= c && c <= '�') || ('�' <= c && c <= '�'));
    }


    private void loadModels ()
    {
        SetupWhiteSpaceShifts();
        GetTokens();

        decoderWorker = new Worker(ModelLoader.Load(decoder), BackendType.GPUCompute);
        decoderWithPastWorker = new Worker(ModelLoader.Load(decoderWithPast), BackendType.GPUCompute);

        FunctionalGraph graph = new FunctionalGraph();
        var input = graph.AddInput(DataType.Float, new DynamicTensorShape(1, 1, 51865));
        var amax = Functional.ArgMax(input, -1, false);
        var selectTokenModel = graph.Compile(amax);
        argmaxWorker = new Worker(selectTokenModel, BackendType.GPUCompute);

        encoderWorker = new Worker(ModelLoader.Load(encoder), BackendType.GPUCompute);
        logMelSpectroWorker = new Worker(ModelLoader.Load(logMelSpectro), BackendType.GPUCompute);
    }

    void Start()
    {
        loadModels();
    }

    void Update()
    {
        
    }

    private void OnDestroy()
    {
        decoderWorker.Dispose();
        decoderWithPastWorker.Dispose();
        encoderWorker.Dispose();
        logMelSpectroWorker.Dispose();
        argmaxWorker.Dispose();
    }

    private IEnumerator recordAudio ()
    {
        AudioClip audio = Microphone.Start(null, false, recordDuration, 16000);
        yield return new WaitForSeconds(recordDuration);
        Microphone.End(null);

        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = audio;
        audioSource.Play();

        Debug.Log("Preparing audio data");
        const int maxSamples = 30 * 16000;
        var data = new float[maxSamples];
        audio.GetData(data, 0);
        Tensor<float> audioInput = new Tensor<float>(new TensorShape(1, maxSamples), data);
        
        // First model.
        logMelSpectroWorker.Schedule(audioInput);
        var logmel = logMelSpectroWorker.PeekOutput() as Tensor<float>;
        
        // Second model.
        encoderWorker.Schedule(logmel);
        Tensor<float> encodedAudio = encoderWorker.PeekOutput() as Tensor<float>;
        
        // Set up for iterative speech recognition.
        int tokenCount = 0;
        NativeArray<int> outputTokens = new NativeArray<int>(maxTokens, Allocator.Persistent);
        outputTokens[0] = START_OF_TRANSCRIPT;
        outputTokens[1] = ENGLISH;// GERMAN;//FRENCH;//
        outputTokens[2] = TRANSCRIBE; //TRANSLATE;//
        //outputTokens[3] = NO_TIME_STAMPS;// START_TIME;//
        tokenCount = 3;
        
        tokensTensor = new Tensor<int>(new TensorShape(1, maxTokens));
        ComputeTensorData.Pin(tokensTensor);
        tokensTensor.Reshape(new TensorShape(1, tokenCount));
        tokensTensor.dataOnBackend.Upload<int>(outputTokens, tokenCount);
        
        lastToken = new NativeArray<int>(1, Allocator.Persistent);
        lastToken[0] = NO_TIME_STAMPS;
        lastTokenTensor = new Tensor<int>(new TensorShape(1, 1), new[] { NO_TIME_STAMPS });
        
        string outputString = "";
        int index = 0;
        do
            {
            // Third model.
            decoderWorker.SetInput("input_ids", tokensTensor);
            decoderWorker.SetInput("encoder_hidden_states", encodedAudio);
            decoderWorker.Schedule();
            
            // Fourth model.
            var past_key_values_0_decoder_key = decoderWorker.PeekOutput("present.0.decoder.key") as Tensor<float>;
            var past_key_values_0_decoder_value = decoderWorker.PeekOutput("present.0.decoder.value") as Tensor<float>;
            var past_key_values_1_decoder_key = decoderWorker.PeekOutput("present.1.decoder.key") as Tensor<float>;
            var past_key_values_1_decoder_value = decoderWorker.PeekOutput("present.1.decoder.value") as Tensor<float>;
            var past_key_values_2_decoder_key = decoderWorker.PeekOutput("present.2.decoder.key") as Tensor<float>;
            var past_key_values_2_decoder_value = decoderWorker.PeekOutput("present.2.decoder.value") as Tensor<float>;
            var past_key_values_3_decoder_key = decoderWorker.PeekOutput("present.3.decoder.key") as Tensor<float>;
            var past_key_values_3_decoder_value = decoderWorker.PeekOutput("present.3.decoder.value") as Tensor<float>;
            
            var past_key_values_0_encoder_key = decoderWorker.PeekOutput("present.0.encoder.key") as Tensor<float>;
            var past_key_values_0_encoder_value = decoderWorker.PeekOutput("present.0.encoder.value") as Tensor<float>;
            var past_key_values_1_encoder_key = decoderWorker.PeekOutput("present.1.encoder.key") as Tensor<float>;
            var past_key_values_1_encoder_value = decoderWorker.PeekOutput("present.1.encoder.value") as Tensor<float>;
            var past_key_values_2_encoder_key = decoderWorker.PeekOutput("present.2.encoder.key") as Tensor<float>;
            var past_key_values_2_encoder_value = decoderWorker.PeekOutput("present.2.encoder.value") as Tensor<float>;
            var past_key_values_3_encoder_key = decoderWorker.PeekOutput("present.3.encoder.key") as Tensor<float>;
            var past_key_values_3_encoder_value = decoderWorker.PeekOutput("present.3.encoder.value") as Tensor<float>;
            
            decoderWithPastWorker.SetInput("input_ids", lastTokenTensor);
            decoderWithPastWorker.SetInput("past_key_values.0.decoder.key", past_key_values_0_decoder_key);
            decoderWithPastWorker.SetInput("past_key_values.0.decoder.value", past_key_values_0_decoder_value);
            decoderWithPastWorker.SetInput("past_key_values.1.decoder.key", past_key_values_1_decoder_key);
            decoderWithPastWorker.SetInput("past_key_values.1.decoder.value", past_key_values_1_decoder_value);
            decoderWithPastWorker.SetInput("past_key_values.2.decoder.key", past_key_values_2_decoder_key);
            decoderWithPastWorker.SetInput("past_key_values.2.decoder.value", past_key_values_2_decoder_value);
            decoderWithPastWorker.SetInput("past_key_values.3.decoder.key", past_key_values_3_decoder_key);
            decoderWithPastWorker.SetInput("past_key_values.3.decoder.value", past_key_values_3_decoder_value);
            
            decoderWithPastWorker.SetInput("past_key_values.0.encoder.key", past_key_values_0_encoder_key);
            decoderWithPastWorker.SetInput("past_key_values.0.encoder.value", past_key_values_0_encoder_value);
            decoderWithPastWorker.SetInput("past_key_values.1.encoder.key", past_key_values_1_encoder_key);
            decoderWithPastWorker.SetInput("past_key_values.1.encoder.value", past_key_values_1_encoder_value);
            decoderWithPastWorker.SetInput("past_key_values.2.encoder.key", past_key_values_2_encoder_key);
            decoderWithPastWorker.SetInput("past_key_values.2.encoder.value", past_key_values_2_encoder_value);
            decoderWithPastWorker.SetInput("past_key_values.3.encoder.key", past_key_values_3_encoder_key);
            decoderWithPastWorker.SetInput("past_key_values.3.encoder.value", past_key_values_3_encoder_value);
            
            decoderWithPastWorker.Schedule();
            
            // Fifth model.
            var logits = decoderWithPastWorker.PeekOutput("logits") as Tensor<float>;
            argmaxWorker.Schedule(logits);
            
            // Collate the output.
            var t_Token = argmaxWorker.PeekOutput().ReadbackAndClone() as Tensor<int>;
            index = t_Token[0];
            
            outputTokens[tokenCount] = lastToken[0];
            lastToken[0] = index;
            tokenCount++;
            tokensTensor.Reshape(new TensorShape(1, tokenCount));
            tokensTensor.dataOnBackend.Upload<int>(outputTokens, tokenCount);
            lastTokenTensor.dataOnBackend.Upload<int>(lastToken, 1);
            
            if (index < tokens.Length)
                {
                outputString += GetUnicodeText(tokens[index]);
                }
            
            Debug.Log(outputString);
            outputTextDisplay.text = outputString;
            
            past_key_values_0_decoder_key.Dispose();
            past_key_values_0_decoder_value.Dispose();
            past_key_values_1_decoder_key.Dispose();
            past_key_values_1_decoder_value.Dispose();
            past_key_values_2_decoder_key.Dispose();
            past_key_values_2_decoder_value.Dispose();
            past_key_values_3_decoder_key.Dispose();
            past_key_values_3_decoder_value.Dispose();
            
            past_key_values_0_encoder_key.Dispose();
            past_key_values_0_encoder_value.Dispose();
            past_key_values_1_encoder_key.Dispose();
            past_key_values_1_encoder_value.Dispose();
            past_key_values_2_encoder_key.Dispose();
            past_key_values_2_encoder_value.Dispose();
            past_key_values_3_encoder_key.Dispose();
            past_key_values_3_encoder_value.Dispose();
            
            logits.Dispose();
            t_Token.Dispose();
            }
        while (index != END_OF_TEXT) ;
        
        audioInput.Dispose();
        logmel.Dispose();
        encodedAudio.Dispose();
        
        lastTokenTensor.Dispose();
        tokensTensor.Dispose();

        yield return null;
    }

    public void processAudio ()
    {
        Debug.Log("Processing audio");
        StartCoroutine (recordAudio());
    }
}
