using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadePanel : MonoBehaviour
{
    public delegate void FadeCallback();   

    public Image target;
    public float fadeTime = 0.5f;

    [Header("Dynamic")]
    public bool shouldBeVisible;
    public float timestamp;
    public float fadeDeltaT;

    public event FadeCallback OnFadeIn;
    public event FadeCallback OnFadeOut;

    private void Start()
    {
        shouldBeVisible = true;
        fadeDeltaT = 1.0f;
    }

    private void Update()
    {
        if (fadeDeltaT < 1.0f)
        {
            fadeDeltaT += Time.unscaledDeltaTime / fadeTime;
            if (fadeDeltaT >= 1.0f)
            {
                if (shouldBeVisible)
                {
                    OnFadeIn();
                }
                else
                {
                    OnFadeOut();
                }
            }
        }

        target.color = shouldBeVisible ? Color.Lerp(Color.clear, Color.black, fadeDeltaT) : Color.Lerp(Color.black, Color.clear, fadeDeltaT);
    }

    public void FadeIn()
    {
        shouldBeVisible = true;
        fadeDeltaT = 0.0f;
    }

    public void FadeOut()
    {
        shouldBeVisible = false;
        fadeDeltaT = 0.0f;
    }




}
