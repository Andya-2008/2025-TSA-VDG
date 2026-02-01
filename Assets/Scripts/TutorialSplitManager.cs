using UnityEngine;

public class TutorialSplitManager : SplitScreenManager
{
    public GameObject Camera1;
    public GameObject Camera2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TutorialSwitch(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TutorialSwitch(bool pac)
    {
        if(pac)
        {
            Camera1.SetActive(true);
            Camera2.SetActive(false);
            SwitchPacManBall(true);
        }
        else
        {
            Camera1.SetActive(false);
            Camera2.SetActive(true);
            SwitchPacManBall(false);
        }
    }
}
