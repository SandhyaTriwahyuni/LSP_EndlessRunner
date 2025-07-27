using UnityEngine;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
    public TextMeshProUGUI ScoreTextHUD; // Untuk UI HUD
    public TextMeshProUGUI ScoreTextGameOver; // Untuk UI Game Over
    public TextMeshProUGUI ScoreTextWin; // Untuk UI Win
    public TextMeshProUGUI ScoreTextPause; // Untuk UI Win

    private float _score;
    private float _currentScore;
    public int maxScore; // Skor maksimum untuk memenangkan permainan

    void Start()
    {
        _score = 0;
        _currentScore = 0; // Inisialisasi _currentScore
        UpdateScoreHUD();
    }

    void Update()
    {
        if (PlayerManager.GameOver)
            return;

        // Update total score with current score
        _score = _currentScore;
        UpdateScoreHUD();

        // Cek apakah skor mencapai 100
        if (_score >= maxScore)
        {
            PlayerManager.GameOver = true; // Set game over jika menang
        }
    }

    public void TerasiHit()
    {
        _currentScore += 1f; // Menambah skor
        UpdateScoreHUD();
    }

    public void ObstacleHit()
    {
        _currentScore -= 5f; // Mengurangi skor
        if (_currentScore < 0)
        {
            _currentScore = 0;
        }
        UpdateScoreHUD();
    }

    void UpdateScoreHUD()
    {
        if (ScoreTextHUD != null)
        {
            ScoreTextHUD.text = Mathf.Round(_score).ToString() + "/100";
        }

        if (ScoreTextPause != null)
        {
            ScoreTextPause.text = Mathf.Round(_score).ToString() + "/100";
        }
    }

    public void DisplayGameOverScore()
    {
        if (ScoreTextGameOver != null)
        {
            ScoreTextGameOver.text = "KAYU BAKAR: " + Mathf.Round(_score).ToString();
        }

        if (ScoreTextWin != null) // Pastikan ini hanya diupdate di kondisi menang
        {
            ScoreTextWin.text = "KAYU BAKAR: " + Mathf.Round(_score).ToString();
        }
    }
    public bool HasPlayerWon()
    {
        return _score >= maxScore;
    }
}
