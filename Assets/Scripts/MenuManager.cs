using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] AudioSource PlaySFX;
    int change = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerPrefs.SetInt("audio", 1);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return) && SceneManager.GetActiveScene().buildIndex == 0)
        {
            OnPressPlay();
        }
    }

    public void OnPressPlay()
    {
        GameObject.Find("PlayButton").SetActive(false);
        //Load the 1st cutscene
        //SceneManager.LoadScene("PinballTutorial", LoadSceneMode.Single);
        GameObject.Find("TitleScreen").GetComponent<UIWalkIn>().StartWalking();
        GameObject.Find("NoCover").GetComponent<PlayableDirector>().Play();
        GameObject.Find("FadeBlack").GetComponent<PlayableDirector>().Play();
        foreach(UIFootstepBob bob in FindObjectsByType<UIFootstepBob>(FindObjectsSortMode.None))
        {
            bob.enabled = true;
        }
        if(SFXManager.Instance)
        SFXManager.Instance.PlaySFX(4, 2500f, 500f);
        StartCoroutine(DoorCreak());
    }

    public IEnumerator DoorCreak()
    {
        yield return new WaitForSeconds(2f);

        foreach (UIFootstepBob bob in FindObjectsByType<UIFootstepBob>(FindObjectsSortMode.None))
        {
            bob.enabled = false;
        }
        if (SFXManager.Instance)
            SFXManager.Instance.PlaySFX(5, 1500f, 500f);
        yield return new WaitForSeconds(2);

        SceneManager.LoadScene("IntroSceneArcade", LoadSceneMode.Single);
    }

    public void ChangeSFX()
    {
        if (change == 0)
        {
            PlayerPrefs.SetInt("audio", 0);
            GameObject.Find("soundOff").GetComponent<Image>().enabled = true;
            if(SFXManager.Instance)
            SFXManager.Instance.TurnOffSFX(true);
            if(MusicManager.Instance)
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
