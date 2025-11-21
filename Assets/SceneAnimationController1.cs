using UnityEngine;

public class SceneAnimationController1 : MonoBehaviour
{
    [SerializeField] AudioSource bigGlitch;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GlitchChange()
    {
        Debug.Log("Collided with glitch");
        GameObject.Find("SplitScreenManager").GetComponent<TutorialSplitManager>().TutorialSwitch(false);
        bigGlitch.Play();
    }
}
