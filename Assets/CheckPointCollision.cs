using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Playables;

public class CheckPointCollision : MonoBehaviour
{
    [SerializeField] Transform myCheckpoint;
    [SerializeField] int checkPointNum;
    [SerializeField] Transform myGhostCheck;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Pinball Ball") { return; }
        myCheckpoint.GetComponent<PlayableDirector>().Play();
        GameObject.Find("Player").GetComponent<TutorialPacBall>().SetCheckpoint(checkPointNum);
        Vector3 myGhostCh = myGhostCheck.position;
        GameObject.Find("Player").GetComponent<TutorialPacBall>().SetGhostCheckpoint(myGhostCheck.position);
        Debug.Log("Checkpoint reached");
        if (checkPointNum == 0)
        {
            //GameObject.Find("Ghost_Blinky").GetComponent<Ghost>().DeactivateInTutorial();
        }
    }
}
