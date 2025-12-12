using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private KartController kart;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI lapText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Slider powerSlider;
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
        // TODO: vary the speed display at a slower rate to simulate speedometer fluctuation
        float speedVariation = Random.Range(0.9f, 1.1f);
        speedText.text = Mathf.FloorToInt(kart.Speed * 2 * speedVariation) + " mph";
        lapText.text = "Lap: " + gameController.getLapsCompleted() + "/3";
        float time = Time.timeSinceLevelLoad;
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time - minutes * 60);
        int milliseconds = Mathf.FloorToInt((time * 1000F) % 1000);
        timeText.text = string.Format("{0:0}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        powerSlider.value = kart.currentSpeed / kart.acceleration;
    }
}
