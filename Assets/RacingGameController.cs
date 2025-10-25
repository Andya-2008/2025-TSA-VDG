using System.Collections;
using UnityEngine;

public class RacingGameController : MonoBehaviour
{
    private int lapsCompleted;
    private int lapsToFinish = 3;
    [SerializeField] private KartController kart;
    [SerializeField] private GameObject[] blockSpawnPoints;
    [SerializeField] private GameObject[] blockPrefabs;
    private int currentBlockIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lapsCompleted = 0;
        spawnBlockAtRandomPositionInSpawnPointCollider(blockPrefabs[currentBlockIndex], blockSpawnPoints[0]);
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

    public int getLapsCompleted()
    {
        return lapsCompleted;
    }

    private void spawnBlockAtRandomPositionInSpawnPointCollider(GameObject blockPrefab, GameObject spawnPoint)
    {
        Collider spawnCollider = spawnPoint.GetComponent<Collider>();
        Bounds bounds = spawnCollider.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        float z = Random.Range(bounds.min.z, bounds.max.z);

        Vector3 spawnPosition = new Vector3(x, y, z);
        Destroy(Instantiate(blockPrefab, spawnPosition, spawnPoint.transform.rotation), 10f);
    }
}
