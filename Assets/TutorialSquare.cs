using UnityEngine;

public class TutorialSquare : MonoBehaviour
{
    public int square;
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
        if(square == 0)
        GameObject.Find("Ghost_Blinky").GetComponent<Ghost>().ActivateInTutorial();
    }
}
