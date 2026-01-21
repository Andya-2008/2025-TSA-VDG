using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public bool paused;

    [SerializeField] float slowSpeed = 0.5f;
    [SerializeField] CanvasGroup pauseGroupSerial;
    [SerializeField] CanvasGroup firstCanvaspauseGroupSerial;
    [SerializeField] float maxBlur = 16f;
    int change = 0;

    private bool isTransitioning = false;

    bool initPause;

    public bool tutorial;

    private void Awake()
    {
        if (pauseGroupSerial != null)
            pauseGroupSerial.alpha = 0f;
            pauseGroupSerial.interactable = false;
        if (PlayerPrefs.GetInt("audio") == 0)
        {
            GameObject.Find("soundOff").GetComponent<Image>().enabled = true;
        }
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
        if (!tutorial)
        {
            if (!initPause)
            {
                Pause(true);
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Resume(true);
                }
            }
        }
    }

    public void Pause(bool firstCanvas = false)
    {
        if (paused || isTransitioning) return;   // ⛔ block mid-animation
        paused = true;
        StartCoroutine(SlowTime(firstCanvas));
    }

    public void Resume(bool firstCanvas = false)
    {
        if (!paused || isTransitioning) return;  // ⛔ block mid-animation
        paused = false;

        initPause = true;
        StartCoroutine(SpeedTime(firstCanvas));
    }

    public void ReturnToArcade()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("IntroSceneArcade");
    }

    IEnumerator SlowTime(bool firstCanvas = false)
    {
        CanvasGroup pauseGroup;
        if (!firstCanvas)
        {
            pauseGroup = pauseGroupSerial;
            pauseGroupSerial.interactable = true;
        }
        else
        {
            pauseGroup = firstCanvaspauseGroupSerial;
        }
        isTransitioning = true;
            GameObject.Find("GameManager").GetComponent<GameManager>().RecordStop();
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

    IEnumerator SpeedTime(bool firstCanvas = false)
    {
        CanvasGroup pauseGroup;
        if (!firstCanvas)
        {
            pauseGroup = pauseGroupSerial;
            pauseGroupSerial.interactable = false;
        }
        else
        {
            pauseGroup = firstCanvaspauseGroupSerial;
        }
        isTransitioning = true;

        pauseGroup.blocksRaycasts = false;
        pauseGroup.interactable = false;
        GameObject.Find("GameManager").GetComponent<GameManager>().RecordResume();

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

    public void ChangeSFX()
    {
        if (change == 0)
        {
            PlayerPrefs.SetInt("audio", 0);
            GameObject.Find("soundOff").GetComponent<Image>().enabled = true;
            if (SFXManager.Instance)
                SFXManager.Instance.TurnOffSFX(true);
            if (MusicManager.Instance)
                MusicManager.Instance.TurnOffMusic(true);
            change = 1;
        }
        else if (change == 1)
        {
            PlayerPrefs.SetInt("audio", 1);
            GameObject.Find("soundOff").GetComponent<Image>().enabled = false;
            if (SFXManager.Instance)
                SFXManager.Instance.TurnOffSFX(false);
            if (MusicManager.Instance)
                MusicManager.Instance.TurnOffMusic(false);
            change = 0;
        }
    }
}
