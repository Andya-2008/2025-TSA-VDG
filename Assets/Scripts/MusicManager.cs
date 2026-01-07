using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager _instance;

    public static MusicManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<MusicManager>();
            return _instance;
        }
    }

    [SerializeField] private List<AudioSource> music = new List<AudioSource>();
    [SerializeField] private float fadeTime = 1.5f;
    bool testSwitch;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            TestAudioSwitch();
        }
    }

    // ---------------------------------------------------------
    // NORMAL CROSSFADE (used inside same level)
    // ---------------------------------------------------------
    public void SwitchTracks(int musicIndex)
    {
        for (int i = 0; i < music.Count; i++)
        {
            if (i == musicIndex)
            {
                // Fade IN the desired song
                if (!music[i].isPlaying)
                {
                    Debug.Log("New music playing");
                    music[i].Play();
                }

                StartCoroutine(FadeMusic(music[i], 1)); // fade in
            }
            else
            {
                // Fade OUT the rest
                StartCoroutine(FadeMusic(music[i], 0)); // fade out
            }
        }
    }

    // ---------------------------------------------------------
    // LEVEL MUSIC: Fade out EVERYTHING, start new track fresh
    // ---------------------------------------------------------
    public void PlayNewTrack(int musicIndex)
    {
        for (int i = 0; i < music.Count; i++)
        {
            if (i != musicIndex)
            {
                StartCoroutine(FadeOutAndStop(music[i]));
            }
        }

        // Start new track at volume 0 and fade in
        AudioSource newTrack = music[musicIndex];
        newTrack.Stop();
        newTrack.volume = 0f;
        newTrack.Play();

        StartCoroutine(FadeMusic(newTrack, 1)); // fade in new level music
    }

    // ---------------------------------------------------------
    // Fade IN or OUT a single track (does NOT stop audio)
    // ---------------------------------------------------------
    public IEnumerator FadeMusic(AudioSource audio, int fadeDirection)
    {
        float startVol = audio.volume;
        float targetVol = (fadeDirection == 1) ? audio.GetComponent<Audio>().volume : 0f;
        float timer = 0f;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            float t = timer / fadeTime;
            audio.volume = Mathf.Lerp(startVol, targetVol, t);
            yield return null;
        }

        audio.volume = targetVol;
    }

    // ---------------------------------------------------------
    // Fade out fully THEN stop audio
    // ---------------------------------------------------------
    public IEnumerator FadeOutAndStop(AudioSource audio)
    {
        float startVol = audio.volume;
        float timer = 0f;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            float t = timer / fadeTime;
            audio.volume = Mathf.Lerp(startVol, 0f, t);
            yield return null;
        }

        audio.volume = 0f;
        audio.Stop();
    }

    void TestAudioSwitch()
    {
        if(testSwitch)
        {
            SwitchTracks(0);
            testSwitch = false;
        }
        else
        {
            SwitchTracks(1);
            testSwitch = true;
        }
    }
}
