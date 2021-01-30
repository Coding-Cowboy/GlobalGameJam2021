using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public CanvasGroup pauseOverlay;
    public float fadeSpeed = 1;
    bool overlayActive = false;
    bool overlayFading = false;

    public Button continueButton;
    public Button quitButton;


    // Start is called before the first frame update
    void Start()
    {
        // Disable pause panel + overlay
        pauseOverlay.alpha = 0; // invisible black
        pauseOverlay.interactable = false;

        // Rig buttons to work
        continueButton.onClick.AddListener(TogglePause);
        quitButton.onClick.AddListener(QuitClicked);
    }

    // Update is called once per frame
    void Update()
    {
        // Toggle PAUSE
        if (Input.GetButtonDown("Cancel"))
        {
            TogglePause();
        }

        // Overlay fade
        if (overlayFading)
        {
            if (overlayActive)
            {
                // Fade in
                pauseOverlay.alpha = Mathf.Clamp01(pauseOverlay.alpha + (fadeSpeed * Time.unscaledDeltaTime));
                if (pauseOverlay.alpha >= 1)
                    overlayFading = false;
            }
            else
            {
                // Fade out
                pauseOverlay.alpha = Mathf.Clamp01(pauseOverlay.alpha - (fadeSpeed * Time.unscaledDeltaTime));
                if (pauseOverlay.alpha <= 0)
                    overlayFading = false;
            }
        }
    }

    // Fade dark overlay in/out
    void TogglePause()
    {
        if (overlayActive)
        {
            overlayActive = false;
            pauseOverlay.interactable = false;
            Time.timeScale = 1;
        }
        else
        {
            overlayActive = true;
            pauseOverlay.interactable = true;
            Time.timeScale = 0;
            EventSystem.current.SetSelectedGameObject(continueButton.gameObject.transform.parent.gameObject); // set focus on pause panel
        }
        overlayFading = true;
    }

    void QuitClicked()
    {
        // Quit to menu
        SceneManager.LoadScene(0);
    }
}
