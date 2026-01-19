using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArcadeSceneManager : MonoBehaviour
{
    [SerializeField] private PlayerMovement player;
    [SerializeField] private GameObject firstArcadeMachine;
    // [SerializeField] private GameObject secondArcadeMachine;
    // [SerializeField] private GameObject thirdArcadeMachine;
    [SerializeField] private LevelLoader levelLoader;
    [SerializeField] private GameObject instructionText;

    // Update is called once per frame
    void Update()
    {
        if (player.getArcadeMachineNumber() == 1)
        {
            firstArcadeMachine.GetComponent<SpriteRenderer>().enabled = true;
            // secondArcadeMachine.GetComponent<SpriteRenderer>().enabled = false;
            // thirdArcadeMachine.GetComponent<SpriteRenderer>().enabled = false;
            instructionText.SetActive(true);
        }
        else if (player.getArcadeMachineNumber() == 2)
        {
            firstArcadeMachine.GetComponent<SpriteRenderer>().enabled = false;
            // secondArcadeMachine.GetComponent<SpriteRenderer>().enabled = true;
            // thirdArcadeMachine.GetComponent<SpriteRenderer>().enabled = false;
            instructionText.SetActive(false);
        }
        else if (player.getArcadeMachineNumber() == 3)
        {
            firstArcadeMachine.GetComponent<SpriteRenderer>().enabled = false;
            // secondArcadeMachine.GetComponent<SpriteRenderer>().enabled = false;
            // thirdArcadeMachine.GetComponent<SpriteRenderer>().enabled = true;
            instructionText.SetActive(false);
        }
        else
        {
            firstArcadeMachine.GetComponent<SpriteRenderer>().enabled = false;
            // secondArcadeMachine.GetComponent<SpriteRenderer>().enabled = false;
            // thirdArcadeMachine.GetComponent<SpriteRenderer>().enabled = false;
            instructionText.SetActive(false);
        }
    }

    public void LoadFirstArcadeGame()
    {
        Debug.Log("Loading first arcade game scene...");
        StartCoroutine(levelLoader.LoadArcadeGame(1));
    }
}
