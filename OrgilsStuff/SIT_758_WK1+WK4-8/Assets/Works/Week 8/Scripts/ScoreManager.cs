using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int Score { get; private set; } = 0;
    public TMP_Text scoreText;

    void Start()
    {
        UpdateScoreUI();
    }

    public void AddPoints(int points)
    {
        Score += points;
        UpdateScoreUI();
    }

    public void SubtractPoints(int points)
    {
        Score -= points;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {Score}";
    }
}
