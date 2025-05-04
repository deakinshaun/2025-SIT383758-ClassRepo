using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Works.Week_8.Scripts
{
    [System.Serializable]
    public class JeopardyQuestion
    {
        [JsonProperty("category")] public string Category { get; set; }
        [JsonProperty("air_date")] public string AirDate { get; set; }
        [JsonProperty("question")] public string Question { get; set; }
        [JsonProperty("value")] public string Value { get; set; }
        [JsonProperty("answer")] public string Answer { get; set; }
        [JsonProperty("round")] public string Round { get; set; }
        [JsonProperty("show_number")] public string ShowNumber { get; set; }
    }

    public class GameManager : MonoBehaviour
    {
        public TextAsset jeopardyJsonFile;

        public RectTransform questionGrid;
        public GameObject questionButtonPrefab;
        public QuestionPopup questionPopup;
        public CategorySelector categorySelector;
        
        public string showNumber = "4680";
        public Dictionary<string, Dictionary<string, JeopardyQuestion>> questionsByCategory;
        public List<string> valueTiers;

        private void Start()
        {
            LoadAndFilterQuestions();
            GenerateButtons();
            categorySelector.StartListening();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                categorySelector.StartListening();
            }
        }

        private void LoadAndFilterQuestions()
        {
            List<JeopardyQuestion> allQuestions =
                JsonConvert.DeserializeObject<List<JeopardyQuestion>>(jeopardyJsonFile.text);
            var filtered = allQuestions
                .Where(q => q.ShowNumber == showNumber && q.Round == "Jeopardy!")
                .ToList();
            
            const NumberStyles style = NumberStyles.Integer | NumberStyles.AllowThousands;
            questionsByCategory = filtered
                .Where(q=>q.Value!=null)
                .GroupBy(q => q.Category)
                .Where(g=>g.Count()==5)
                .ToDictionary(
                    g => g.Key,
                    g => g
                        .GroupBy(q => q.Value)
                        .ToDictionary(
                            gg => gg.Key,
                            gg => gg.First()));

            valueTiers = filtered
                .Where(q=>q.Value!=null)
                .Select(q => q.Value)
                .Distinct()
                .OrderBy(v => int.Parse(v.Replace("$",""),style))
                .ToList();
        }

        void GenerateButtons()
        {
            foreach (var category in questionsByCategory.Keys)
            {
                GameObject column = new GameObject($"column:{category}");

                column.transform.parent = questionGrid;
                var verticalLayoutGroup = column.AddComponent<VerticalLayoutGroup>();
                verticalLayoutGroup.padding = new RectOffset { left = 5, right = 5, top = 5, bottom = 5 };
                verticalLayoutGroup.spacing = 5;
                verticalLayoutGroup.childControlHeight = true;
                verticalLayoutGroup.childControlWidth = true;
                var categoryBtn = Instantiate(questionButtonPrefab, column.transform);
                categoryBtn.GetComponentInChildren<TMP_Text>().text = category;
                categoryBtn.GetComponentInChildren<TMP_Text>().enableAutoSizing = true;

                foreach (var value in valueTiers)
                {
                    
                    if (questionsByCategory[category].TryGetValue(value, out JeopardyQuestion question))
                    {
                        GameObject btn = Instantiate(questionButtonPrefab, column.transform);
                        btn.GetComponentInChildren<TMP_Text>().text = value;
                        btn.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            // column.GetComponent<VerticalLayoutGroup>().enabled = false;
                            questionPopup.ShowQuestion(question); 
                            // btn.gameObject.SetActive(false);
                        });
                    }
                }
            }
        }
    }
}