using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArcadeSceneManager : MonoBehaviour
{
    [SerializeField] private PlayerMovement player;
    [SerializeField] private LevelLoader levelLoader;
    [SerializeField] private GameObject instructionText;
    [SerializeField] private ArcadeMachine[] arcadeMachines;
    [SerializeField] private GameObject controlsText;

    public void Start()
    {
        resetArcadeMachineStates();

        if (MusicManager.Instance)
            MusicManager.Instance.PlayNewTrack(2);
    }
    // Update is called once per frame
    void Update()
    {
        if (PlayerPrefs.GetInt("level") == 1)
        {
            controlsText.SetActive(true);
        }

        else
        {
            controlsText.SetActive(false);
        }
    }

    public void LoadFirstArcadeGame()
    {
        Debug.Log("Loading first arcade game scene...");
        instructionText.GetComponent<TextMeshProUGUI>().enabled = false;
        StartCoroutine(levelLoader.LoadArcadeGame(1));
    }

    public void LoadSecondArcadeGame()
    {
        Debug.Log("Loading second arcade game scene...");
        instructionText.GetComponent<TextMeshProUGUI>().enabled = false;
        StartCoroutine(levelLoader.LoadArcadeGame(2));
    }

    public void resetArcadeMachineStates()
    {
        foreach (ArcadeMachine machine in arcadeMachines)
        {
            // Reset state
            if (machine.getArcadeMachineNumber() == 1)
            {
                machine.SetMachineState(ArcadeMachine.MachineState.UNLOCKED);
            }
            else if (machine.getArcadeMachineNumber() == 2)
            {
                if (PlayerPrefs.GetInt("level") >= 2)
                {
                    machine.SetMachineState(ArcadeMachine.MachineState.UNLOCKED);
                }
                else
                {
                    machine.SetMachineState(ArcadeMachine.MachineState.LOCKED);
                }
            }
            else if (machine.getArcadeMachineNumber() == 3)
            {
                if (PlayerPrefs.GetInt("level") >= 3)
                {
                    machine.SetMachineState(ArcadeMachine.MachineState.UNLOCKED);
                }
                else
                {
                    machine.SetMachineState(ArcadeMachine.MachineState.LOCKED);
                }
            }
        }
    }
}
