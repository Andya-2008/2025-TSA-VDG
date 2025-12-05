using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-10)]
[RequireComponent(typeof(Movement))]
public class Ghost : MonoBehaviour
{
    public Movement movement { get; private set; }
    public GhostHome home { get; private set; }
    public GhostScatter scatter { get; private set; }
    public GhostChase chase { get; private set; }
    public GhostFrightened frightened { get; private set; }
    public GhostBehavior initialBehavior;
    public Transform target;
    public int points = 200;
    public bool tutorial;
    private void Awake()
    {
        movement = GetComponent<Movement>();
        home = GetComponent<GhostHome>();
        scatter = GetComponent<GhostScatter>();
        chase = GetComponent<GhostChase>();
        frightened = GetComponent<GhostFrightened>();
    }

    private void Start()
    {
        ResetState();
        if (tutorial)
        {
            movement.enabled = false;
        }
    }

    public void ResetState()
    {
        if (!tutorial)
        {
            gameObject.SetActive(true);
            movement.ResetState();

            frightened.Disable();
            chase.Disable();
            //scatter.Enable();

            if (home != initialBehavior)
            {
                home.Disable();
            }

            if (initialBehavior != null)
            {
                initialBehavior.Enable();
            }
        }
        if (tutorial)
        {
            gameObject.SetActive(true);
            movement.ResetState();

            frightened.Disable();
            chase.Enable();
            //scatter.Enable();

            if (home != initialBehavior)
            {
                home.Disable();
            }

            if (initialBehavior != null)
            {
                //initialBehavior.Enable();
            }
        }
    }

    public void SetPosition(Vector3 position)
    {
        // Keep the z-position the same since it determines draw depth
        position.z = transform.position.z;
        transform.position = position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == ("Pinball Ball"))
        {
            if (frightened.enabled) {
                Debug.Log("Ghost eaten");
                GameManager.Instance.GhostEaten(this);
                ResetState();
            } else {
                GameManager.Instance.PacmanEaten();
                if (tutorial)
                    StartCoroutine(ResetScene());

            }
        }
    }

    public void ActivateInTutorial()
    {
        if(tutorial)
        {
            movement.enabled = true;
        }
    }
    public void DeactivateInTutorial()
    {
        if (tutorial)
        {
            movement.enabled = false;
        }
    }

    public IEnumerator ResetScene()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(1);
    }

}
