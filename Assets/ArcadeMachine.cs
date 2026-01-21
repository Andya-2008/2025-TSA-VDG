using UnityEngine;
using UnityEngine.AI;

public class ArcadeMachine : MonoBehaviour
{
    public enum MachineState
    {
        LOCKED,
        UNLOCKED,
        HIGHLIGHTED
    }

    private MachineState currentState = MachineState.LOCKED;
    [SerializeField] private int levelNum;
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Sprite unlockedSprite;
    [SerializeField] private Sprite highlightedSprite;
    [SerializeField] private GameObject glitchEffect;
    [SerializeField] private GameObject enterText;

    private SpriteRenderer spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (levelNum == 1)
        {
            currentState = MachineState.UNLOCKED;
        } 

        else if (levelNum == 2)
        {
            if (PlayerPrefs.GetInt("level") >= 2)
            {
                currentState = MachineState.UNLOCKED;
            } 
            else
            {
                currentState = MachineState.LOCKED;
            }
        } 
        else if (levelNum == 3)
        {
            if (PlayerPrefs.GetInt("level") >= 3)
            {
                currentState = MachineState.UNLOCKED;
            } else
            {
                currentState = MachineState.LOCKED;
            }
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        if (currentState == MachineState.UNLOCKED)
        {
            spriteRenderer.sprite = unlockedSprite;
            enterText.SetActive(false);

            if (PlayerPrefs.GetInt("level") == levelNum)
            {
                glitchEffect.SetActive(true);
            }
            else
            {
                glitchEffect.SetActive(false);
            }
        }

        else if (currentState == MachineState.LOCKED)
        {
            spriteRenderer.sprite = lockedSprite;
            glitchEffect.SetActive(false);
            enterText.SetActive(false);
        }

        else if (currentState == MachineState.HIGHLIGHTED)
        {
            spriteRenderer.sprite = highlightedSprite;
            enterText.SetActive(true);

            if (PlayerPrefs.GetInt("level") == levelNum)
            {
                glitchEffect.SetActive(true);
            }
            else
            {
                glitchEffect.SetActive(false);
            }
        }
    }

    public void SetMachineState(MachineState newState)
    {
        this.currentState = newState;
    }

    public int getArcadeMachineNumber()
    {
        return this.levelNum;
    }

    public MachineState GetMachineState()
    {
        return this.currentState;
    }
}
