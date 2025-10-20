using UnityEngine;

public class BallDeath : MonoBehaviour
{
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
        if(collision.gameObject.name == "Player")
        {
            Debug.Log("Game Over");
            GameObject.Find("GameManager").GetComponent<GameManager>().BallDestroyed();
        }
    }
}
