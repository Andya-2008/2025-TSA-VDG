using UnityEngine;

public class PelletSpawner : MonoBehaviour
{
    [SerializeField] PowerPellet powerPellet;
    [SerializeField] Transform spawnPos;

    [SerializeField] float spawnTime;

    bool isPellet;

    float startTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPellet && Time.time - startTime > spawnTime) SpawnPellet();
    }

    public void SpawnPellet()
    {
        startTime = Time.time;
        Instantiate(powerPellet.gameObject, spawnPos);
        isPellet = true;
    }

    public void GrabbedPellet()
    {
        isPellet = false;
        startTime = Time.time;
    }
}
