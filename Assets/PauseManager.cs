using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public bool paused;
    [SerializeField] float slowSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (!paused)
                Pause();
            else
                Resume();
        }
    }

    private void Resume()
    {
        if (!paused) { return; }
        paused = false;
        StartCoroutine(SpeedTime());
    }

    public void Pause()
    {
        if (paused) { return; }
        paused = true;
        StartCoroutine(SlowTime());

    }

    public IEnumerator SlowTime()
    {
        while (Time.timeScale > .01)
        {
            Debug.Log("time.timescale: " + Time.timeScale);
            Time.timeScale -= slowSpeed;
            yield return null;
        }
        Time.timeScale = 0;
    }
    public IEnumerator SpeedTime()
    {
        while (Time.timeScale < 1f)
        {
            Time.timeScale += slowSpeed * Time.unscaledDeltaTime * 100f; // scale for smoothness
            Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
            Debug.Log("Speeding time: " + Time.timeScale);
            yield return null; // still okay, we use unscaledDeltaTime for movement
        }
    }
}
