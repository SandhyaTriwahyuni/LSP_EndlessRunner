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

    // Start is called before the first frame update
    void Start()
    {
        GameOver = false;
        Time.timeScale = 1; // untuk replay
        IsGameStarted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameOver)
        {
            StartCoroutine(GameOverSequence());
        }

        if (ScoreSystem != null && ScoreSystem.HasPlayerWon())
        {
            StartCoroutine(WinSequence()); // Panggil WinSequence jika skor sudah mencapai max
        }

        // Detect Enter key to start the game
        if (Input.GetKeyDown(KeyCode.Return) && !IsGameStarted)
        {
            playerControl.enabled = true;
            IsGameStarted = true;
            Destroy(StartingText);
        }
    }

    IEnumerator GameOverSequence()
    {
        PlayerAnimator.SetTrigger("Die");
        yield return new WaitForSecondsRealtime(4f); 

        Time.timeScale = 0;
        HUD.SetActive(false);
        GameOverPanel.SetActive(true);
        if (ScoreSystem != null)
        {
            ScoreSystem.DisplayGameOverScore(); 
        }
    }

    IEnumerator WinSequence()
    {
        //PlayerAnimator.SetTrigger("Win"); 
        yield return new WaitForSecondsRealtime(0f); 
        Time.timeScale = 0;
        HUD.SetActive(false);
        WinPanel.SetActive(true); 
        if (ScoreSystem != null)
        {
            ScoreSystem.DisplayGameOverScore(); 
        }
    }
}
