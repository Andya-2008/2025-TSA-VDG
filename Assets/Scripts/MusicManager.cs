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

    private bool musicMuted = false;
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
            return;
        }

        PlayNewTrack(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            TestAudioSwitch();
        }
    }

    /* ---------------------------------------------------------
     * PUBLIC MUTE CONTROL
     * --------------------------------------------------------- */

    public void TurnOffMusic(bool mute)
    {
        musicMuted = mute;

        if (musicMuted)
        {
            StopAllCoroutines();

            foreach (AudioSource track in music)
            {
                if (track == null) continue;
                track.Stop();
                track.volume = 0f;
            }
        }
    }

    public bool IsMusicMuted()
    {
        return musicMuted;
    }

    /* ---------------------------------------------------------
     * NORMAL CROSSFADE
     * --------------------------------------------------------- */

    public void SwitchTracks(int musicIndex)
    {
        if (musicMuted) return;

        for (int i = 0; i < music.Count; i++)
        {
            if (i == musicIndex)
            {
                if (!music[i].isPlaying)
                    music[i].Play();

                StartCoroutine(FadeMusic(music[i], 1));
            }
            else
            {
                StartCoroutine(FadeMusic(music[i], 0));
            }
        }
    }

    /* ---------------------------------------------------------
     * LEVEL MUSIC
     * --------------------------------------------------------- */

    public void PlayNewTrack(int musicIndex)
    {
        if (musicMuted) return;

        for (int i = 0; i < music.Count; i++)
        {
            if (i != musicIndex)
                StartCoroutine(FadeOutAndStop(music[i]));
        }

        AudioSource newTrack = music[musicIndex];
        newTrack.Stop();
        newTrack.volume = 0f;
        newTrack.Play();

        StartCoroutine(FadeMusic(newTrack, 1));
    }

    /* ---------------------------------------------------------
     * FADES
     * --------------------------------------------------------- */

    public IEnumerator FadeMusic(AudioSource audio, int fadeDirection)
    {
        float startVol = audio.volume;
        float targetVol = fadeDirection == 1
            ? audio.GetComponent<Audio>().volume
            : 0f;

        float timer = 0f;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            audio.volume = Mathf.Lerp(startVol, targetVol, timer / fadeTime);
            yield return null;
        }

        audio.volume = targetVol;
    }

    public IEnumerator FadeOutAndStop(AudioSource audio)
    {
        float startVol = audio.volume;
        float timer = 0f;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            audio.volume = Mathf.Lerp(startVol, 0f, timer / fadeTime);
            yield return null;
        }

        audio.volume = 0f;
        audio.Stop();
    }

    void TestAudioSwitch()
    {
        if (testSwitch)
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
