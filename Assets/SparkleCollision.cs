using UnityEngine;
using UnityEngine.Playables;

public class SparkleCollision : MonoBehaviour
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
            GameObject.Find("AnimationController").GetComponent<SceneAnimationController1>().GlitchChange();
            GameObject.Find("Ghost_Blinky").GetComponent<Movement>().enabled = false;
        }
    }

}
