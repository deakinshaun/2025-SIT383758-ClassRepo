using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Works.Week_8.Scripts
{
    public class CategorySelector : MonoBehaviour
    {
        public WhisperSpeech speechRecognizer;
        public GameManager boardManager;
        public QuestionPopup questionPopup;
        public TMP_Text outputText;
        public void StartListening()
        {
            speechRecognizer.OnSpeechRecognized += OnSpeech;
            speechRecognizer.processAudio();
        }

        private void OnSpeech(string input)
        {
            speechRecognizer.OnSpeechRecognized -= OnSpeech;
            string normalized = StringUtils.NormalizeString(input);
            string matchedCategory = FindClosestCategory(normalized);
            string matchedValue = FindDollarAmount(normalized);

            Debug.Log($"matched:{matchedCategory}, matchedValue:{matchedValue}");
            if (matchedCategory != null && matchedValue != null &&
                boardManager.questionsByCategory.TryGetValue(matchedCategory, out var valueMap) &&
                valueMap.TryGetValue(matchedValue, out var question))
            {
                questionPopup.ShowQuestion(question);
            }
            else
            {
                Debug.LogWarning("No matching category and value found in voice input.");
                // // Optionally prompt to retry
                // outputText.SetText("Could not catch that, please speak again");
                // StartListening();
            }
        }


        private string NormalizeCategory(string input)
        {
            return input
                .ToLowerInvariant()
                .Replace("&", "and")
                .Replace("’", "")
                .Replace("'", "")
                .Replace("  ", " ")
                .Trim();
        }

        private float WordSetSimilarity(string[] aWords, string[] bWords)
        {
            int matchCount = 0;

            foreach (string aw in aWords)
            {
                foreach (string bw in bWords)
                {
                    float score = StringUtils.Similarity(aw, bw);
                    if (score >= 0.85f) // individual word similarity threshold
                    {
                        matchCount++;
                        break;
                    }
                }
            }

            return (float)matchCount / bWords.Length;
        }

        private string FindClosestCategory(string input)
        {
            string[] inputWords = NormalizeCategory(input).Split(' ');

            float bestScore = 0f;
            string bestMatch = null;

            foreach (string rawCategory in boardManager.questionsByCategory.Keys)
            {
                string normalizedCategory = NormalizeCategory(rawCategory);
                string[] categoryWords = normalizedCategory.Split(' ');

                float matchScore = WordSetSimilarity(inputWords, categoryWords);

                if (matchScore > bestScore)
                {
                    bestScore = matchScore;
                    bestMatch = rawCategory;
                }
            }

            return bestScore >= 0.5f ? bestMatch : null; // Adjust threshold as needed
        }


        private string FindDollarAmount(string input)
        {
            string[] inputWords = NormalizeCategory(input).Split(' ');
            return boardManager.valueTiers.OrderByDescending(v =>
                inputWords.Max(w => StringUtils.Similarity(w, v.Replace("$","")))).FirstOrDefault();
        }
    }
}