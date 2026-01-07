using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] AudioSource PlaySFX;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPressPlay()
    {
        //Load the 1st cutscene
        //SceneManager.LoadScene("PinballTutorial", LoadSceneMode.Single);
        GameObject.Find("TitleScreen").GetComponent<UIWalkIn>().StartWalking();
        GameObject.Find("NoCover").GetComponent<PlayableDirector>().Play();
        GameObject.Find("FadeBlack").GetComponent<PlayableDirector>().Play();
        foreach(UIFootstepBob bob in FindObjectsByType<UIFootstepBob>(FindObjectsSortMode.None))
        {
            bob.enabled = true;
        }
        SFXManager.Instance.PlaySFX(4, 2500f, 500f);
        StartCoroutine(DoorCreak());
    }

    public IEnumerator DoorCreak()
    {
        yield return new WaitForSeconds(3.25f);

        foreach (UIFootstepBob bob in FindObjectsByType<UIFootstepBob>(FindObjectsSortMode.None))
        {
            bob.enabled = false;
        }
        SFXManager.Instance.PlaySFX(5, 1500f, 500f);
        yield return new WaitForSeconds(2);

        SceneManager.LoadScene("PinballTutorial", LoadSceneMode.Single);
    }
}
