using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class SparkleCollision : MonoBehaviour
{
    private bool triggered = false;
    [SerializeField] Transform cameraLockPos;
    [SerializeField] PlayableDirector parentAnim;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered || collision.gameObject.tag != "Pinball Ball") return;
        //if (!collision.CompareTag("Player")) return;

        triggered = true;
        GameObject.Find("Player").transform.Rotate(0, 0, 90);
        GameObject.Find("Player").GetComponent<Pacman>().enabled = false;
        parentAnim.Play();
        GameObject.Find("AnimationController")
            .GetComponent<SceneAnimationController1>()
            .GlitchChange();

        GameObject.Find("CineCamera (1)")
            .GetComponent<LayerFlickerGlitch>()
            .CallLayerGlitch(1f);

        GameObject.Find("Ghost_Blinky")
            .GetComponent<Movement>()
            .enabled = false;
        //GameObject.Find("CineCamera (1)").GetComponent<PacCameraFollow>().MoveToPoint(cameraLockPos.position);
    }
}
