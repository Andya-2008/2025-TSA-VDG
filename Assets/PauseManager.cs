using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PauseManager : MonoBehaviour
{
    public bool paused;

    [SerializeField] float slowSpeed = 0.5f;
    [SerializeField] CanvasGroup pauseGroup;
    [SerializeField] float maxBlur = 16f;

    private bool isTransitioning = false;

    private void Awake()
    {
        if (pauseGroup != null)
            pauseGroup.alpha = 0f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!paused)
                Pause();
            else
                Resume();
        }
    }

    public void Pause()
    {
        if (paused || isTransitioning) return;   // ⛔ block mid-animation
        paused = true;
        StartCoroutine(SlowTime());
    }

    public void Resume()
    {
        if (!paused || isTransitioning) return;  // ⛔ block mid-animation
        paused = false;
        StartCoroutine(SpeedTime());
    }

    IEnumerator SlowTime()
    {
        isTransitioning = true;
        SFXManager.Instance.PlaySFX(2);
        pauseGroup.blocksRaycasts = true;
        pauseGroup.interactable = true;

        while (Time.timeScale > 0.01f)
        {
            float newScale = Time.timeScale - slowSpeed * Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Clamp(newScale, 0.01f, 1f);

            float linear = 1f - Time.timeScale;
            float eased = Mathf.SmoothStep(0f, 1f, linear);

            pauseGroup.alpha = eased;

            yield return null;
        }

        Time.timeScale = 0f;
        pauseGroup.alpha = 1f;

        isTransitioning = false;   // ✔ animation done
    }

    IEnumerator SpeedTime()
    {
        isTransitioning = true;

        SFXManager.Instance.PlaySFX(3);
        pauseGroup.blocksRaycasts = false;
        pauseGroup.interactable = false;

        while (Time.timeScale < 1f)
        {
            float newScale = Time.timeScale + slowSpeed * Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Clamp(newScale, 0f, 1f);

            float linear = 1f - Time.timeScale;
            float eased = Mathf.SmoothStep(0f, 1f, linear);

            pauseGroup.alpha = eased;

            yield return null;
        }

        pauseGroup.alpha = 0f;

        isTransitioning = false;   // ✔ animation done
    }
}
