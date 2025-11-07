using UnityEngine;

public class BlockSpawnPoint : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RacingGameController racingGameController = FindObjectOfType<RacingGameController>();
            racingGameController.spawnBlockAtRandomPositionInSpawnPointCollider(
                racingGameController.blockSpawnPoints[racingGameController.currentBlockIndex]
            );
        }
    }
}
