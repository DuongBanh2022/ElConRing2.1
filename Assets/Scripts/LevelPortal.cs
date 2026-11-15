using UnityEngine;
using UnityEngine.SceneManagement;

// Goes on Exit (loads a scene when the player touches it)
// and on TitleManager (Button On Click).
public class LevelPortal : MonoBehaviour
{
    [Header("Portal Settings")]
    [SerializeField] private string destinationScene = "TitleScreen";
    [SerializeField] private bool recordOnEnter = false;

    [Header("Portal Sound")]
    [SerializeField] private AudioClip portalEnterSound;

    public void GoToScene(string targetScene)
    {
        if (targetScene == "Game1")
        {
            GameSession.NewGame();
        }

        SceneManager.LoadScene(targetScene);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (recordOnEnter)
        {
            GameSession.SaveHighScore();
        }

        // Play sound when the player enters the portal.
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(portalEnterSound);
        }

        GoToScene(destinationScene);
    }
}
