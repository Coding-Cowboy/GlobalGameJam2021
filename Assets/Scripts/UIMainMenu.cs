using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    public CanvasGroup darkOverlay;
    public float fadeInSpeed = 0.25f;
    public float fadeOutSpeed = 0.5f;
    bool fadingIn = false;
    bool fadingOut = false;

    public Button beginButton;
    public Button exitButton;

    // Start is called before the first frame update
    void Start()
    {
        // Enable overlay and start fade in
        darkOverlay.gameObject.SetActive(true);

        // Rig buttons to work
        beginButton.onClick.AddListener(BeginClicked);
        exitButton.onClick.AddListener(ExitClicked);

        EventSystem.current.SetSelectedGameObject(beginButton.gameObject.transform.parent.gameObject); // set focus on title panel

        // Slight delay to allow scene to load for smoother experience
        Invoke("StartFadeIn", 0.3f);
    }

    // Update is called once per frame
    void Update()
    {
        // Fade overlay in
        if (fadingIn)
        {
            darkOverlay.alpha = Mathf.Clamp01(darkOverlay.alpha - (fadeInSpeed * Time.deltaTime));
            if (darkOverlay.alpha <= 0)
                fadingIn = false;
        }
        // Fade overlay out
        else if (fadingOut)
        {
            darkOverlay.alpha = Mathf.Clamp01(darkOverlay.alpha + (fadeOutSpeed * Time.deltaTime));
            if (darkOverlay.alpha >= 1)
            {
                fadingOut = false;
                // Start the game!
                StartGame();
            }
        }
    }

    void StartFadeIn()
    {
        // Start fading in from black
        fadingIn = true;
    }

    void StartGame()
    {
        // Load main game level
        SceneManager.LoadScene(1);
    }

    void BeginClicked()
    {
        // Start fading out to black
        fadingOut = true;
    }

    void ExitClicked()
    {
        // Quit game (or editor)
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
