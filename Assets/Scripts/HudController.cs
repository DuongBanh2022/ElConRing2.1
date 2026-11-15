using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class HudController : MonoBehaviour
{
    [SerializeField] TMP_Text scoreLabel;
    [SerializeField] TMP_Text bestLabel;
    [SerializeField] GameObject pausePanel;

    [Header("Finish Panel")]
    [SerializeField] GameObject finishPanel;
    [SerializeField] TMP_Text finishScoreLabel;
    [SerializeField] TMP_Text finishBestLabel;

    private bool isPaused = false;
    private bool hasFinished = false;

    void Update()
    {
        if (scoreLabel != null)
            scoreLabel.text = "Dandelions: " + GameSession.Score;

        if (bestLabel != null)
            bestLabel.text = "Dandelions Eaten: " + GameSession.HighScore;

        if (pausePanel != null &&
            !hasFinished &&
            Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        if (pausePanel == null || hasFinished) return;

        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        if (pausePanel == null || hasFinished) return;

        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void ShowFinishPanel()
    {
        if (hasFinished) return;

        hasFinished = true;

        if (finishScoreLabel != null)
            finishScoreLabel.text = "Final Dandelions Eaten: " + GameSession.Score;

        if (finishBestLabel != null)
            finishBestLabel.text = "Dandelions Collect: " + GameSession.HighScore;

        if (finishPanel != null)
            finishPanel.SetActive(true);

        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ReturnToTitleScreen()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScreen");
    }

    public void QuitToDesktop()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }

    void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
