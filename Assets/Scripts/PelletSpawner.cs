using UnityEngine;

public class PelletSpawner : MonoBehaviour
{
    [SerializeField] PowerPellet powerPellet;

    [SerializeField] float maxSpawnTime = 20;
    [SerializeField] float minSpawnTime = 5;
    [SerializeField] float randTime;

    bool isPellet;

    float startTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnPellet();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPellet && Time.time - startTime > randTime) SpawnPellet();
    }

    public void SpawnPellet()
    {
        randTime = Random.Range(minSpawnTime, maxSpawnTime);
        startTime = Time.time;
        GameObject newPellet = Instantiate(powerPellet.gameObject, transform);
        newPellet.GetComponent<PowerPellet>().myParent = this;
        isPellet = true;
    }

    public void GrabbedPellet()
    {
        isPellet = false;
        startTime = Time.time;
    }
}
