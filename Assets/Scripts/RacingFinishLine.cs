using UnityEngine;

public class RacingFinishLine : MonoBehaviour
{
    [SerializeField] private RacingGameController gameController;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            gameController.finishLap();
        }
    }
}
