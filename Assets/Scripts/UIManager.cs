using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public CanvasGroup darkOverlay;
    public float fadeInSpeed = 0.5f;
    public float fadeOutSpeed = 0.75f;
    bool fadingIn = true;
    bool fadingOut = false;

    public CanvasGroup pauseOverlay;
    public float pauseFadeSpeed = 2;
    bool pauseActive = false;
    bool pauseFading = false;

    public Button continueButton;
    public Button quitButton;


    // Start is called before the first frame update
    void Start()
    {
        // Enable overlay and start fade in
        darkOverlay.gameObject.SetActive(true);

        // Disable pause panel + pause overlay
        pauseOverlay.alpha = 0; // invisible black
        pauseOverlay.interactable = false;

        // Rig buttons to work
        continueButton.onClick.AddListener(TogglePause);
        quitButton.onClick.AddListener(QuitClicked);
    }

    // Update is called once per frame
    void Update()
    {
        // Fade dark overlay in
        if (fadingIn)
        {
            darkOverlay.alpha = Mathf.Clamp01(darkOverlay.alpha - (fadeInSpeed * Time.deltaTime));
            if (darkOverlay.alpha <= 0)
                fadingIn = false;
        }
        // Fade dark overlay out
        else if (fadingOut)
        {
            darkOverlay.alpha = Mathf.Clamp01(darkOverlay.alpha + (fadeOutSpeed * Time.deltaTime));
            if (darkOverlay.alpha >= 1)
            {
                fadingOut = false;
                // Quit to menu
                QuitToMenu();
            }
        }

        // Toggle PAUSE
        if (Input.GetButtonDown("Cancel"))
        {
            TogglePause();
        }

        // Pause overlay fade
        if (pauseFading)
        {
            if (pauseActive)
            {
                // Fade in
                pauseOverlay.alpha = Mathf.Clamp01(pauseOverlay.alpha + (pauseFadeSpeed * Time.unscaledDeltaTime));
                if (pauseOverlay.alpha >= 1)
                    pauseFading = false;
            }
            else
            {
                // Fade out
                pauseOverlay.alpha = Mathf.Clamp01(pauseOverlay.alpha - (pauseFadeSpeed * Time.unscaledDeltaTime));
                if (pauseOverlay.alpha <= 0)
                    pauseFading = false;
            }
        }
    }

    // Fade dark overlay in/out
    void TogglePause()
    {
        if (pauseActive)
        {
            pauseActive = false;
            pauseOverlay.interactable = false;
            Time.timeScale = 1;
        }
        else
        {
            pauseActive = true;
            pauseOverlay.interactable = true;
            Time.timeScale = 0;
            EventSystem.current.SetSelectedGameObject(continueButton.gameObject.transform.parent.gameObject); // set focus on pause panel
        }
        pauseFading = true;
    }

    void QuitClicked()
    {
        // Start fading out to black
        Time.timeScale = 1;
        fadingOut = true;
    }

    void QuitToMenu()
    {
        // Quit to menu
        SceneManager.LoadScene(0);
    }
}
