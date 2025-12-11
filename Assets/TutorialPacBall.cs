using UnityEngine;
using System.Collections.Generic;

public class TutorialPacBall : MonoBehaviour
{
    [SerializeField] List<Transform> checkPoints = new List<Transform>();
    Transform currentCheckpoint;
    Transform currentGhostCheckpoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentCheckpoint = checkPoints[0];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCheckpoint(int checkPoint)
    {
        currentCheckpoint = checkPoints[checkPoint];
        
        GameObject.Find("Player").GetComponent<Movement>().startingPosition = currentCheckpoint.position;
    }
    public void SetGhostCheckpoint(Transform checkPoint)
    {
        currentGhostCheckpoint = checkPoint;
        Debug.Log("Setting ghost CheckPoint:" + checkPoint.name);
        GameObject.Find("Ghost_Blinky").GetComponent<Movement>().startingPosition = currentGhostCheckpoint.position;
    }
}
