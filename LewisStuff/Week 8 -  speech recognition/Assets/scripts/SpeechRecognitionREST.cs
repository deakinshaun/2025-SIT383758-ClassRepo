using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SpeechRecognitionREST : MonoBehaviour
{
    // Reference to the UI Text component to display the result
    //had TMP enabled and found link below that fixes issue as it was using the unity text system instead of TMP system on this line
    //https://discussions.unity.com/t/why-can-t-i-put-text-in-the-public-text-variable-spot/252976
    public TextMeshProUGUI outputTextMeshProUGUI;

    //This will need to be replaced with the own key
    private string subscriptionKey = "06eda0a0e323449780212ef76d543bc7";
    private string token;

    //length of any recording sent. 10 s is the current maximum
    private int recordDuration = 5;

    private static bool trustCertificate(
        object sender,
        X509Certificate x509Certificate,
        X509Chain x509Chain,
        SslPolicyErrors sslPolicyErrors)
    {
        // Trust all certificates
        return true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ServicePointManager.ServerCertificateValidationCallback = trustCertificate;
    }

    // These structures contain the relevant fields from
    // the response strings/JSON that is returned by the service.
    [System.Serializable]
    class AssemblyResponse
    {
        public string id;
        public string status;
        public string text;
    };

    [System.Serializable]
    class AssemblyUploadResponse
    {
        public string upload_url;
    };

    // not reallly needed, but useful to test the connection
    //disabled for the API.assemblyai code
    /*public void Authentication()
        {
            // Unity webforms do not handle the certificates required for https servers.

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://westus.api.cognitive.microsoft.com/sts/v1.0/issueToken");
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";
            request.Headers["Ocp-Apim-Subscription-Key"] = subscriptionKey;

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            Debug.Log("Token received: " + responseString);

            token = responseString;
        }*/

    private string uploadData(byte[] wavData)
    {
        string fetchUri = "https://api.assemblyai.com/v2/upload";
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fetchUri + "");
        request.Headers["authorization"] = subscriptionKey;
        request.Method = "POST";
        Stream rs = request.GetRequestStream();
        rs.Write(wavData, 0, wavData.Length);
        rs.Close();

        var response = (HttpWebResponse)request.GetResponse();

        var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
        Debug.Log("Response from service: " + responseString);
        if (outputTextMeshProUGUI != null)
        {
            outputTextMeshProUGUI.text = responseString;
        }
        AssemblyUploadResponse r = JsonUtility.FromJson<AssemblyUploadResponse>(responseString);
        Debug.Log("Got id " + r.upload_url);
        return r.upload_url;
    }


    public void SpeechToText (byte [] wavData)
    {


        //disabled fot the API.assemblyai code
        /*
        // This function sends the audio data to the server and receives the response
        string fetchUrl = "https://westus.stt.speech.microsoft.com/speech/recognition/conversation/cognitiveservices/v1?language=en-US";
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fetchUrl + "https://westus.stt.speech.microsoft.com/speech/recognition/conversation/cognitiveservices/v1?language=en-US");
        request.ContentType = "application/x-www-form-urlencoded";
        request.Method = "POST";
        request.ContentType = "audio/wav; codec=\"audio/pcm\"; samplerate=16000";
        request.Headers["Ocp-Apim-Subscription-Key"] = subscriptionKey;*/

        // Read a wav file off the filesystem and send that to the service. Note that the paths
        // used may not working when packaged for a mobile platform. This is useful for testing
        // under controlled and repeatable conditions (same input file each time).
        //
        // Stream rs = request.GetRequestStream();
        // FileStream fileStream = new FileStream(Application.dataPath + "/SpeechRecognition/Sound/untitled.wav", FileMode.Open, FileAccess.Read);
        // byte[] buffer = new byte[4096];
        // int bytesRead = 0;
        // while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0) {
        // rs.Write(buffer, 0, bytesRead);
        // }
        // fileStream.Close();
        // rs.Close();


        // convert wav to a file on the server.
        string wavURL = uploadData(wavData);

        // This function sends the audio data to the server and receives the response
        string fetchUri = "https://api.assemblyai.com/v2/transcript";

        //fix this to the correct URL as it was fetchUri
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fetchUri + "");
        request.ContentType = "application/application/json";
        request.Headers["authorization"] = subscriptionKey;
        request.Method = "POST";
        byte[] wData = System.Text.Encoding.ASCII.GetBytes("{\"audio_url\": \"" + wavURL + "\"}");
        Stream rs = request.GetRequestStream();
        //was typeo wavdata instead of wData!!!!!
        rs.Write(wData, 0, wData.Length);
        rs.Close();

        var response = (HttpWebResponse)request.GetResponse();

        var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
        Debug.Log("Response from service: " + responseString);
        if (outputTextMeshProUGUI != null)
        {
            outputTextMeshProUGUI.text = responseString;
        }

        AssemblyResponse r = JsonUtility.FromJson <AssemblyResponse> (responseString);
        Debug.Log("Got id " + r.id);

        // Wait for the speech to text to complete.
        StartCoroutine (waitForTranscript (r.id));

    }
    private IEnumerator waitForTranscript (string id)
    {
        AssemblyResponse r = null;
        // We only poll 10 times, at 5 second intervals. Longer portions of
        // speech would require changing this.
        for (int i = 0; i < 10; i++)
        {
            string fetchUri = "https://api.assemblyai.com/v2/transcript";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fetchUri + "/" + id);
            request.ContentType = "application/application/json";
            request.Headers["authorization"] = subscriptionKey;
            request.Method = "GET";
            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            Debug.Log("Response from service: " + responseString);
            if (outputTextMeshProUGUI != null)
            {
                outputTextMeshProUGUI.text = responseString;
            }
            r = JsonUtility.FromJson <AssemblyResponse> (responseString);
            if (r.status == "completed")
            {
                break;
            }
            yield return new WaitForSeconds(5.0f);

        }
        outputTextMeshProUGUI.text = r.text;

    }
    private IEnumerator recordAudio()
    {
        // Set the microphone recording. Service requires 16 kHz sampling.
        AudioClip audio = Microphone.Start(null, false, recordDuration, 16000);
        yield return new WaitForSeconds(recordDuration);
        Microphone.End (null);

        // Play the recording back, to validate it was recorded correctly.
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = audio;
        audioSource.Play();

        // Convert it to a wav file, and upload to the service.
        byte [] wavData = ConvertToWav (audio);
        SpeechToText (wavData);
    }

    public void Trigger()
    {
        ///Authentication ();

        StartCoroutine (recordAudio());
    }

    // Remaining functions adapted from: https://gist.github.com/darktable/2317063

    const int HEADER_SIZE = 44; // WAV header size

    static byte [] ConvertToWav (AudioClip clip)
    {
        var samples = new float[clip.samples];

        clip.GetData(samples, 0);

        Int16[] intData = new Int16[samples.Length];
        //converting in 2float[] steps to Int16[], //then int16 [] to Byte[]

        //Had this line incorrectly as byte[] bytes = new byte[HEADER_SIZE + samples.Length * 2]; Now fixed

        Byte[] bytesData = new Byte[HEADER_SIZE + samples.Length * 2];
        //bytesData array is twice the size of
        //dataSource array because a float converted in Int16 is 2 bytes.

        int rescaleFactor = 32767; //to convert float to Int16

        WriteHeader(bytesData, clip);

        for (int i = 0; i<samples.Length; i++)
        {
            intData[i] = (short) (samples[i] * rescaleFactor);
            Byte[] byteArr = new Byte[2];
            byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, HEADER_SIZE + i * 2);
        }

        return bytesData;
    }

    static void WriteHeader(byte [] bytesData, AudioClip clip)
    {

        var hz = clip.frequency;
        var channels = clip.channels;
        var samples = clip.samples;

        Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
        riff.CopyTo(bytesData, 0);

        Byte[] chunkSize = BitConverter.GetBytes(HEADER_SIZE + clip.samples * 2 - 8);
        chunkSize.CopyTo(bytesData, 4);

        Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
        wave.CopyTo(bytesData, 8);

        Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
        fmt.CopyTo(bytesData, 12);

        Byte[] subChunk1 = BitConverter.GetBytes(16);
        subChunk1.CopyTo(bytesData, 16);

        UInt16 one = 1;

        Byte[] audioFormat = BitConverter.GetBytes(one);
        audioFormat.CopyTo(bytesData, 20);

        Byte[] numChannels = BitConverter.GetBytes(channels);
        numChannels.CopyTo(bytesData, 22);

        Byte[] sampleRate = BitConverter.GetBytes(hz);
        sampleRate.CopyTo(bytesData, 24);

        Byte[] byteRate = BitConverter.GetBytes(hz * channels * 2); // sampleRate * bytesPerSample*number of channels, here 44100*2*2
        byteRate.CopyTo(bytesData, 28);

        UInt16 blockAlign = (ushort) (channels * 2);
        BitConverter.GetBytes(blockAlign).CopyTo(bytesData, 32);

        UInt16 bps = 16;
        Byte[] bitsPerSample = BitConverter.GetBytes(bps);
        bitsPerSample.CopyTo(bytesData, 34);

        Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
        datastring.CopyTo(bytesData, 36);

        Byte[] subChunk2 = BitConverter.GetBytes(samples * channels * 2);
        subChunk2.CopyTo(bytesData, 40);
    }
}
