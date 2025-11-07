using System.Collections;
using UnityEngine;

public class RacingGameController : MonoBehaviour
{
    private int lapsCompleted;
    private int lapsToFinish = 3;
    [SerializeField] private KartController kart;
    public GameObject[] blockSpawnPoints;
    public GameObject[] blockPrefabs;
    public int currentBlockIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lapsCompleted = 0;
        StartCoroutine(spawnFirstBlock());
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

    IEnumerator spawnFirstBlock()
    {
        yield return new WaitForSeconds(2);
        spawnBlockAtRandomPositionInSpawnPointCollider(blockSpawnPoints[0]);
    }

    public void spawnBlockAtRandomPositionInSpawnPointCollider(GameObject spawnPoint)
    {
        Collider spawnCollider = spawnPoint.GetComponent<Collider>();
        Bounds bounds = spawnCollider.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = 100f; // Fixed height for spawning blocks
        float z = Random.Range(bounds.min.z, bounds.max.z);

        GameObject blockPrefab = blockPrefabs[Random.Range(0, blockPrefabs.Length)];

        Vector3 spawnPosition = new Vector3(x, y, z);
        Destroy(Instantiate(blockPrefab, spawnPosition, spawnPoint.transform.rotation), 10f);

        currentBlockIndex = (currentBlockIndex + 1) % blockSpawnPoints.Length; ;
    }
}
