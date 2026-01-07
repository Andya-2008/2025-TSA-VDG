using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneAnimationController1 : MonoBehaviour
{
    [SerializeField] AudioSource bigGlitch;
    [SerializeField] GameObject glow;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(bigGlitch);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            SceneManager.LoadScene("Pinball");
        }
    }

    public void GlitchChange()
    {
        GameObject.Find("SplitScreenManager").GetComponent<TutorialSplitManager>().TutorialSwitch(false);
        glow.GetComponent<AudioDistanceFade>().enabled = false;
        glow.GetComponent<AudioSource>().enabled = false;
        foreach (GlitchFlickerController fC in FindObjectsByType<GlitchFlickerController>(FindObjectsSortMode.None))
        {
            fC.CallGlitch(1f);
        }
        bigGlitch.Play();
        //GameObject.Find("Ghost_Blinky").GetComponent<Ghost>().DeactivateInTutorial();
    }
}
