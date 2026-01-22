using UnityEngine;

public class GameManagerLvl2 : GameManager
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Playing 4th track");
        MusicManager.Instance.PlayNewTrack(4);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
