using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public TextMeshProUGUI movesText;
    public TextMeshProUGUI scoreText;

    private int moveCount = 0;
    private int score = 0;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RegisterMove(int diskWeight)
    {
        moveCount++;
        score += diskWeight;

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (movesText != null)
            movesText.text = "Moves: " + moveCount;

        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }
}
