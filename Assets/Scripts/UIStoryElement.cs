using UnityEngine;

public class UIStoryElement : MonoBehaviour
{
    public float showDelay = 1.4f;
    public float hideDelay = 1.0f;
    public float fadeSpeed = 0.6f;
    bool fadingIn = false;
    bool fadingOut = false;
    CanvasGroup element;

    // Start is called before the first frame update
    void Start()
    {
        element = GetComponent<CanvasGroup>();
        element.alpha = 0;
        element.interactable = false;
        Invoke("FadeIn", showDelay);
    }

    // Update is called once per frame
    void Update()
    {
        // Fade in
        if (fadingIn)
        {
            element.alpha = Mathf.Clamp01(element.alpha + (fadeSpeed * Time.deltaTime));
            if (element.alpha >= 1)
            {
                fadingIn = false;
                Invoke("FadeOut", hideDelay);
            }
        }
        // Fade out
        else if (fadingOut)
        {
            element.alpha = Mathf.Clamp01(element.alpha - (fadeSpeed * Time.deltaTime));
            if (element.alpha <= 0)
            {
                fadingOut = false;
                gameObject.SetActive(false);
            }
        }
    }

    void FadeIn()
    {
        fadingIn = true;
    }

    void FadeOut()
    {
        fadingOut = true;
    }
}
