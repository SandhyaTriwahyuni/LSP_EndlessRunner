using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static bool GameOver;
    public GameObject GameOverPanel;
    public GameObject WinPanel; // Panel untuk kondisi menang
    public GameObject HTPButton;
    public GameObject HUD;
    public ScoreSystem ScoreSystem;
    public HealthManager HealthManager; // Referensi ke HealthManager

    // Variabel untuk StartGame
    public static bool IsGameStarted;
    public GameObject StartingText;

    public PlayerControl playerControl;
    public Animator PlayerAnimator;

    [SerializeField] private CutsceneManager cutsceneManager;
    private bool hasWon = false;

    // Start is called before the first frame update
    void Start()
    {
        GameOver = false;
        Time.timeScale = 1; // untuk replay
        IsGameStarted = false;
        HUD.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (ScoreSystem != null && ScoreSystem.HasPlayerWon() && !hasWon)
        {
            hasWon = true;  // Set flag agar tidak berulang
            StartCoroutine(WinSequence());
        }

        if (GameOver && !hasWon)
        {
            StartCoroutine(GameOverSequence());
        }

        // Detect Enter key to start the game
        if (Input.GetKeyDown(KeyCode.Return) && !IsGameStarted)
        {
            playerControl.enabled = true;
            IsGameStarted = true;
            Destroy(StartingText);
            HUD.SetActive(true);
        }
    }

    IEnumerator GameOverSequence()
    {
        PlayerAnimator.SetTrigger("Die");
        yield return new WaitForSecondsRealtime(4f);

        Time.timeScale = 0;
        HUD.SetActive(false);
        GameOverPanel.SetActive(true);
        SoundManager.Instance.PlaySound3D("Game Over", transform.position);

        if (ScoreSystem != null)
        {
            ScoreSystem.DisplayGameOverScore(); 
        }
    }

    IEnumerator WinSequence()
    {
        if (cutsceneManager != null)
        {
            cutsceneManager.PlayCutscene();
        }

        yield return null;
    }
    public void ShowWinPanel()
    {
        HUD.SetActive(false);
        WinPanel.SetActive(true);
        GameOverPanel.SetActive(false);  

        if (ScoreSystem != null)
        {
            ScoreSystem.DisplayGameOverScore();
        }

        Time.timeScale = 0;  
    }
}
