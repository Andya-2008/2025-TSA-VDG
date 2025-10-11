using JetBrains.Annotations;
using UnityEngine;

public class RacingGameController : MonoBehaviour
{
    private int lapsCompleted;
    private int lapsToFinish = 3;
    [SerializeField] private KartController kart;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void finishLap()
    {
        lapsCompleted++;
        if (lapsCompleted >= lapsToFinish)
        {
            Debug.Log("You finished the race!");
            kart.RaceFinished();
        }
    }

    public int getLapsCompleted() {
        return lapsCompleted;
    } 
}
