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

    public Button continueButton;
    public Button quitButton;

    public GameObject inGamePanel;

    public CanvasGroup pauseOverlay;
    public float pauseFadeSpeed = 2;
    bool pauseActive = false;
    bool pauseFading = false;

    public CanvasGroup deathOverlay;
    public float deathFadeSpeed = 2;
    bool deathActive = false;
    bool deathFading = false;

    public CanvasGroup endgameOverlay;
    public float endgameFadeSpeed = 0.05f;
    bool endgameActive = false;
    bool endgameFading = false;

    AudioSource music;
    public AudioClip endgameMusic;
    public AudioClip endgameSoundEffect;
    float volumeGoal;
    bool musicOut = false;

    HeartManager heartManager;

    // Start is called before the first frame update
    void Start()
    {
        // Enable overlay and start fade in
        darkOverlay.gameObject.SetActive(true);

        // Disable pause panel + pause overlay
        pauseOverlay.alpha = 0;
        pauseOverlay.interactable = false;
        // Disable death panel + death overlay
        deathOverlay.alpha = 0;
        deathOverlay.interactable = false;
        // Disable endgame overlay
        endgameOverlay.alpha = 0;
        endgameOverlay.interactable = false;

        // Rig buttons to work
        continueButton.onClick.AddListener(TogglePause);
        quitButton.onClick.AddListener(QuitClicked);

        music = GetComponent<AudioSource>();
        volumeGoal = music.volume;
        music.volume = 0.005f;
        music.Play();

        heartManager = FindObjectOfType<HeartManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Fade music in
        if (!musicOut)
            if (music.volume < volumeGoal)
                music.volume += 0.006f;
            else
                musicOut = true;
        // Fade music out
        else if (fadingOut)
            if (music.volume > 0)
                music.volume -= 0.006f;

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
        if (Input.GetButtonDown("Pause") && !deathActive && !endgameActive)
            TogglePause();

        // Check DEATH
        if (heartManager.IsDead() && !deathActive)
            ToggleDeath();
        if (deathActive && Input.GetButtonDown("Pause"))
            ToggleDeath();

        // Check ENDGAME exit
        if (endgameActive && Input.GetButtonDown("Pause"))
            QuitClicked();

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

        // Death overlay fade
        if (deathFading)
        {
            if (deathActive)
            {
                // Fade in
                deathOverlay.alpha = Mathf.Clamp01(deathOverlay.alpha + (deathFadeSpeed * Time.unscaledDeltaTime));
                if (deathOverlay.alpha >= 1)
                    deathFading = false;
            }
            else
            {
                // Fade out
                deathOverlay.alpha = Mathf.Clamp01(deathOverlay.alpha - (deathFadeSpeed * Time.unscaledDeltaTime));
                if (deathOverlay.alpha <= 0)
                    deathFading = false;
            }
        }

        // Endgame overlay fade
        if (endgameFading)
        {
            // Fade in
            endgameOverlay.alpha = Mathf.Clamp01(endgameOverlay.alpha + (endgameFadeSpeed * Time.unscaledDeltaTime));
            if (endgameOverlay.alpha >= 1)
                endgameFading = false;
        }
        // Endgame music fade
        if (endgameActive)
            if (music.clip != endgameMusic)
                if (music.volume > 0)
                    music.volume -= 0.006f;
                else
                {
                    music.clip = endgameMusic;
                    music.Play();
                }
            else if (music.volume < volumeGoal && !fadingOut)
                music.volume += 0.006f;

    }

    // Fade pause overlay in/out
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

    // Fade death overlay in/out
    void ToggleDeath()
    {
        if (deathActive)
        {
            QuitClicked();
        }
        else
        {
            deathActive = true;
            deathOverlay.interactable = true;
            Time.timeScale = 0;
        }
        deathFading = true;
    }

    // Fade in the endgame overlay
    public void Endgame()
    {
        if (!endgameActive)
            SoundEffectScript.PlaySoundEffect(transform, endgameSoundEffect, 0.32f);
        endgameActive = true;
        endgameFading = true;
        inGamePanel.SetActive(false);
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
