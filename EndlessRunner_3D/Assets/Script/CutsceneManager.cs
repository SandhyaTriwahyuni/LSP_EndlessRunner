using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Events;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage displayScreen;
    [SerializeField] private GameObject cutscenePanel;
    [SerializeField] private PlayerManager playerManager;

    [Header("Audio Settings")]
    [SerializeField] private float musicFadeDuration = 0.5f;

    private bool isCutscenePlaying = false;

    private void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.playOnAwake = false;
            videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            videoPlayer.targetTexture = new RenderTexture((int)videoPlayer.clip.width, (int)videoPlayer.clip.height, 24);
            videoPlayer.loopPointReached += OnVideoComplete;
            videoPlayer.started += OnVideoStarted;

            if (displayScreen != null)
            {
                displayScreen.texture = videoPlayer.targetTexture;
            }
        }

        if (cutscenePanel != null)
        {
            cutscenePanel.SetActive(false);
        }
    }

    public void PlayCutscene()
    {
        if (videoPlayer != null && cutscenePanel != null && !isCutscenePlaying)
        {
            isCutscenePlaying = true;
            StartCoroutine(StartCutsceneSequence());
        }
    }

    private System.Collections.IEnumerator StartCutsceneSequence()
    {
        // Fade out musik
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.FadeOutMusic(musicFadeDuration);
        }

        // Tunggu musik fade out
        yield return new WaitForSeconds(musicFadeDuration);

        // Pause musik setelah fade out
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PauseMusic();
        }

        // Mulai cutscene
        cutscenePanel.SetActive(true);
        displayScreen.gameObject.SetActive(true);
        videoPlayer.Play();

        Time.timeScale = 1f;
    }

    private void OnVideoStarted(VideoPlayer vp)
    {
        Debug.Log("Cutscene Started");
    }

    private void OnVideoComplete(VideoPlayer vp)
    {
        Debug.Log("Cutscene Complete");
        isCutscenePlaying = false;

        StartCoroutine(EndCutsceneSequence());
    }

    private System.Collections.IEnumerator EndCutsceneSequence()
    {
        cutscenePanel.SetActive(false);
        displayScreen.gameObject.SetActive(false);

        // Resume dan fade in musik
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.ResumeMusic();
            MusicManager.Instance.FadeInMusic(musicFadeDuration);
        }

        // Tunggu musik fade in
        yield return new WaitForSeconds(musicFadeDuration);

        // Tampilkan win panel
        if (playerManager != null)
        {
            playerManager.ShowWinPanel();
        }
    }

    private void OnDisable()
    {
        // Pastikan musik kembali normal jika scene/object di-disable
        if (MusicManager.Instance != null && isCutscenePlaying)
        {
            MusicManager.Instance.ResumeMusic();
            MusicManager.Instance.FadeInMusic(musicFadeDuration);
        }
    }
}