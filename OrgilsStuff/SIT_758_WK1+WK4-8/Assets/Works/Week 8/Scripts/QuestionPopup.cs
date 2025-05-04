using System;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace Works.Week_8.Scripts
{
    public class QuestionPopup : MonoBehaviour
    {
        public GameObject panel;
        public TMP_Text questionText;
        public TMP_Text answerText;
        public ScoreManager scoreManager;
        
        [Header("Audio")] public AudioSource audioSource;
        public AudioClip correctSound;
        public AudioClip incorrectSound;

        [Header("Speech")] public WhisperSpeech speechRecognizer;

        private JeopardyQuestion currentQuestion;

        private void Start()
        {
            panel.SetActive(false);
            answerText.gameObject.SetActive(false);
        }

        public void ShowQuestion(JeopardyQuestion q)
        {
            panel.SetActive(true);
            currentQuestion = q;

            questionText.text = q.Question;
            answerText.text = $"Answer: {q.Answer}";
            speechRecognizer.OnSpeechRecognized += OnAnswerReceived;
            speechRecognizer.processAudio();
        }

        private void OnAnswerReceived(string input)
        {
            speechRecognizer.OnSpeechRecognized -= OnAnswerReceived;
            bool correct = IsAnswerCorrect(input, currentQuestion.Answer);
            audioSource.PlayOneShot(correct ? correctSound : incorrectSound);

            if (correct)
            {
                scoreManager.AddPoints(int.Parse(currentQuestion.Value.Replace("$",""),NumberStyles.Any));
            }
            else
            {
                scoreManager.SubtractPoints(int.Parse(currentQuestion.Value.Replace("$",""),NumberStyles.Any));
            }

            ShowAnswer();
            Invoke(nameof(Hide), 3f);
        }

        private string RemovePhrases(string input, string[] phrases)
        {
            foreach (var p in phrases)
                input = input.Replace(p, "").Trim();
            return input;
        }

        public void ShowAnswer()
        {
            answerText.gameObject.SetActive(true);
        }

        public void Hide()
        {
            answerText.gameObject.SetActive(false);
            panel.SetActive(false);
        }

        private bool IsAnswerCorrect(string input, string expected)
        {
            string user = StringUtils.NormalizeString(Normalize(input));
            string correct = StringUtils.NormalizeString(Normalize(expected));
            float similarity =  StringUtils.Similarity(user, correct);
            // Accept answer if 80%+ similar
            return similarity >= 0.7f;
        }

        private string Normalize(string s)
        {
            s = s.ToLowerInvariant().Trim();
            s = RemovePhrases(s, new[] { "what is", "who is", "where is", "the", "a", "an" });
            return s;
        }
    }
}