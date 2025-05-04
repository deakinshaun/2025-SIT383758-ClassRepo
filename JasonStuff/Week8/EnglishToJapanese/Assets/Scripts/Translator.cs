using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;

public class Translator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

        public TextMeshProUGUI translatedTextDisplay;

  
   // private string apiKey = "";  

    // Function to trigger the translation
    public void TranslateToJapanese(string englishText)
    {
        StartCoroutine(TranslateWithDeepL(englishText));
    }

    // Coroutine to translate text using DeepL API
    IEnumerator TranslateWithDeepL(string englishText)
    {
        // DeepL API endpoint for free translation
        string url = "https://api-free.deepl.com/v2/translate?auth_key=" + apiKey + "&text=" + UnityWebRequest.EscapeURL(englishText) + "&target_lang=JA";

        // Create UnityWebRequest to make the request
        UnityWebRequest request = new UnityWebRequest(url, "GET");
        request.downloadHandler = new DownloadHandlerBuffer();

        // Send the request and wait for the response
        yield return request.SendWebRequest();

        // Check if the request was successful
        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;

            // Log the raw response for debugging (optional)
            Debug.Log("Response: " + jsonResponse);

            // Extract the translation from the response
            string translation = ExtractTranslation(jsonResponse);

            // Update the UI with the translated text
            translatedTextDisplay.text = translation;
            Debug.Log("Translated Text: " + translation);
        }
        else
        {
            Debug.LogError("Translation failed: " + request.error);
        }
    }

    // Helper function to extract the translation from the JSON response
    string ExtractTranslation(string jsonResponse)
    {
        // Find the start and end of the translated text in the JSON response
        string marker = "\"text\":\"";
        int start = jsonResponse.IndexOf(marker) + marker.Length;
        int end = jsonResponse.IndexOf("\"", start);
        return jsonResponse.Substring(start, end - start);
    }
     
}
