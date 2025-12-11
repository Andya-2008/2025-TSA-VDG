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
        myCheckpoint.GetComponent<PlayableDirector>().Play();
        GameObject.Find("Player").GetComponent<TutorialPacBall>().SetCheckpoint(checkPointNum);
        GameObject.Find("Player").GetComponent<TutorialPacBall>().SetGhostCheckpoint(myGhostCheck);
        Debug.Log("Checkpoint reached");
    }
}
