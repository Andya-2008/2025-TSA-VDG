using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Ghost[] ghosts;
    [SerializeField] private Pacman pacman;
    [SerializeField] private Transform pellets;
    [SerializeField] private Text gameOverText;
    [SerializeField] private Text successText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text livesText;

    public bool tutorial;

    [SerializeField] GameObject tutorial1Thing;
    [SerializeField] GameObject tutorial2Thing;

    public int score { get; private set; } = 0;
    public int lives { get; private set; } = 3;

    private int ghostMultiplier = 1;

    // 🎵 Audio slowdown settings
    [Header("Record Stop Audio")]
    [SerializeField] private float recordStopDuration = 1.2f;
    [SerializeField] private float resumeDuration = 0.8f;

    private AudioSource musicSource;
    private Coroutine pitchRoutine;

    [SerializeField] PlayableDirector TitleText;

    

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
        }

        // Find music source named "1"
        GameObject musicObj = GameObject.Find("1");
        if (musicObj != null)
        {
            musicSource = musicObj.GetComponent<AudioSource>();
        }

        if (!tutorial)
        {
            Time.timeScale = 1;
            foreach (GlitchFlickerController fC in FindObjectsByType<GlitchFlickerController>(FindObjectsSortMode.None))
            {
                fC.CallGlitch(.5f);
            }

            PlayerPrefs.SetInt("PacTutorial", 1);
        }
        else
        {
            if (PlayerPrefs.GetInt("PacTutorial") != 1)
                GameObject.Find("SkipButton").SetActive(false);
            StartCoroutine(TitleTextPlay());
        }
    }
    public IEnumerator TitleTextPlay()
    {
        if(MusicManager.Instance)
        MusicManager.Instance.PlayNewTrack(3);
        yield return new WaitForSeconds(2);
        TitleText.Play();
        GameObject.Find("CineCamera").GetComponent<PacCameraFollow>().ReturnToPlayer();
        if (MusicManager.Instance)
            MusicManager.Instance.PlayNewTrack(1);
        yield return new WaitForSeconds(2.5f);
        GameObject.Find("Player").GetComponent<Movement>().canMove = true;
        GameObject.Find("Ghost_Blinky").GetComponent<Movement>().canMove = true;

    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        NewGame();
    }

    private void Update()
    {
        //DEV
        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            pacman.gameObject.SetActive(false);
            GameObject.Find("WhiteFade").GetComponent<PlayableDirector>().Play();
            PlayerPrefs.SetInt("level", 2);
            StartCoroutine(BackToArcade());
        }
        if (lives <= 0 && Input.GetKeyDown(KeyCode.Space))
        {
            RecordResume();

            GameObject.Find("Unfade").GetComponent<PlayableDirector>().Play();
            if(SFXManager.Instance)
            SFXManager.Instance.PlaySFX(7);

            NewGame();
        }
    }
    public IEnumerator BackToArcade()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(1);
    }
    private void NewGame()
    {
        SetScore(0);
        SetLives(3);
        NewRound();
    }

    private void NewRound()
    {
        gameOverText.enabled = false;

        foreach (Transform pellet in pellets)
        {
            pellet.gameObject.SetActive(true);
        }

        ResetState();
    }

    private void ResetState()
    {
        for (int i = 0; i < ghosts.Length; i++)
        {
            ghosts[i].ResetState();
        }

        pacman.ResetState();
    }

    private void GameOver()
    {
        gameOverText.enabled = true;
        GameObject.Find("BlackFade").GetComponent<PlayableDirector>().Play();

        RecordStop(); // 🎵 EXPONENTIAL VINYL STOP

        for (int i = 0; i < ghosts.Length; i++)
        {
            ghosts[i].gameObject.SetActive(false);
        }
    }

    private void SetLives(int lives)
    {
        this.lives = lives;
        livesText.text = "x" + lives.ToString();
    }

    private void SetScore(int score)
    {
        this.score = score;
        scoreText.text = score.ToString().PadLeft(2, '0');
    }

    public void PacmanEaten()
    {
        if (SFXManager.Instance)
            SFXManager.Instance.PlaySFX(6);
        pacman.DeathSequence();
        SetLives(lives - 1);

        if (lives > 0 || tutorial)
        {
            Invoke(nameof(ResetState), 3f);
        }
        else
        {
            GameOver();
        }

        foreach (Ghost ghost in FindObjectsByType<Ghost>(FindObjectsSortMode.None))
        {
            if (!tutorial)
            {
                ghost.GetComponent<GhostScatter>().enabled = true;
            }
        }
    }

    public void BallDestroyed()
    {

        if (SFXManager.Instance)
            SFXManager.Instance.PlaySFX(6);
        GameObject.Find("SplitScreenManager").GetComponent<SplitScreenManager>().SwitchPacManBall(1, true);

        pacman.DeathSequence();
        SetLives(lives - 1);

        if (lives > 0)
        {
            Invoke(nameof(ResetState), 3f);
        }
        else
        {
            GameOver();
        }
    }

    public void GhostEaten(Ghost ghost)
    {
        int points = ghost.points * ghostMultiplier;
        SetScore(score + points);
        ghostMultiplier++;
    }

    public void PelletEaten(Pellet pellet)
    {
        pellet.gameObject.SetActive(false);
        SetScore(score + pellet.points);

        if (!HasRemainingPellets())
        {
            if (!tutorial)
            {
                pacman.gameObject.SetActive(false);
                GameObject.Find("WhiteFade").GetComponent<PlayableDirector>().Play();
                PlayerPrefs.SetInt("level", 2);
                StartCoroutine(BackToArcade());
            }
            else
            {
                tutorial1Thing.SetActive(false);
                tutorial2Thing.SetActive(true);
                GameObject.Find("CineCamera").GetComponent<PacCameraFollow>().ReturnToPlayer();
            }
        }
    }

    public void PowerPelletEaten(PowerPellet pellet)
    {
        for (int i = 0; i < ghosts.Length; i++)
        {
            ghosts[i].frightened.Enable(pellet.duration);
        }

        PelletEaten(pellet);
        CancelInvoke(nameof(ResetGhostMultiplier));
        Invoke(nameof(ResetGhostMultiplier), pellet.duration);
    }

    public bool HasRemainingPellets()
    {
        foreach (Transform pellet in pellets)
        {
            if (pellet.gameObject.activeSelf)
                return true;
        }
        return false;
    }

    private void ResetGhostMultiplier()
    {
        ghostMultiplier = 1;
    }

    // ============================
    // 🎵 RECORD STOP AUDIO EFFECT
    // ============================

    public void RecordStop()
    {
        if (musicSource == null) return;

        if (pitchRoutine != null)
            StopCoroutine(pitchRoutine);

        pitchRoutine = StartCoroutine(ExponentialPitchFade(0.01f, recordStopDuration, true));
    }

    public void RecordResume()
    {
        if (musicSource == null) return;

        musicSource.volume = .5f; // restore volume BEFORE fade-in
        musicSource.UnPause();

        if (pitchRoutine != null)
            StopCoroutine(pitchRoutine);

        pitchRoutine = StartCoroutine(ExponentialPitchFade(1f, resumeDuration, false));
    }

    private IEnumerator ExponentialPitchFade(float targetPitch, float duration, bool pauseAtEnd)
    {
        float startPitch = musicSource.pitch;
        float startVolume = musicSource.volume;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;

            // Exponential curve (heavy slowdown near end)
            float curve = Mathf.Pow(t, 3.5f);

            musicSource.pitch = Mathf.Lerp(startPitch, targetPitch, curve);

            // 🔥 Fade volume ONLY near the very end
            if (pauseAtEnd)
            {
                float volumeFade = Mathf.Clamp01((curve - 0.85f) / 0.15f);
                musicSource.volume = Mathf.Lerp(startVolume, 0f, volumeFade);
            }

            yield return null;
        }

        musicSource.pitch = targetPitch;

        if (pauseAtEnd)
        {
            musicSource.volume = 0f;   // ensure silence
            musicSource.Pause();      // now safe
        }
    }
}
