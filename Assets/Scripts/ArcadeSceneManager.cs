using UnityEngine;
using UnityEngine.SceneManagement;

public class ArcadeSceneManager : MonoBehaviour
{
    [SerializeField] private PlayerMovement player;
    [SerializeField] private GameObject firstArcadeMachine;
    // [SerializeField] private GameObject secondArcadeMachine;
    // [SerializeField] private GameObject thirdArcadeMachine;

    // Update is called once per frame
    void Update()
    {
        if (player.getArcadeMachineNumber() == 1)
        {
            firstArcadeMachine.GetComponent<SpriteRenderer>().enabled = true;
            // secondArcadeMachine.GetComponent<SpriteRenderer>().enabled = false;
            // thirdArcadeMachine.GetComponent<SpriteRenderer>().enabled = false;
        }
        else if (player.getArcadeMachineNumber() == 2)
        {
            firstArcadeMachine.GetComponent<SpriteRenderer>().enabled = false;
            // secondArcadeMachine.GetComponent<SpriteRenderer>().enabled = true;
            // thirdArcadeMachine.GetComponent<SpriteRenderer>().enabled = false;
        }
        else if (player.getArcadeMachineNumber() == 3)
        {
            firstArcadeMachine.GetComponent<SpriteRenderer>().enabled = false;
            // secondArcadeMachine.GetComponent<SpriteRenderer>().enabled = false;
            // thirdArcadeMachine.GetComponent<SpriteRenderer>().enabled = true;
        }
        else
        {
            firstArcadeMachine.GetComponent<SpriteRenderer>().enabled = false;
            // secondArcadeMachine.GetComponent<SpriteRenderer>().enabled = false;
            // thirdArcadeMachine.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public void LoadFirstArcadeGame()
    {
        Debug.Log("Loading first arcade game scene...");
        SceneManager.LoadScene("PinballTutorial");
    }
}
