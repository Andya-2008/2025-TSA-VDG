using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private KartController kart;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI lapText;
    [SerializeField] private RacingGameController gameController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        speedText.text = kart.Speed * 2 + " mph";
        lapText.text = "Lap: 0/3";
    }

    // Update is called once per frame
    void Update()
    {
        speedText.text = kart.Speed * 2 + " mph";
        lapText.text = "Lap: " + gameController.getLapsCompleted() + "/3";
    }
}
