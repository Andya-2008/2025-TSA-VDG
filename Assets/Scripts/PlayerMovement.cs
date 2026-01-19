using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] ArcadeSceneManager arcadeSceneManager;
    private int arcadeMachineNumber = 0;
    public bool canMove = true;
    void Start()
    {
        // Move player from x pos -9.5 to x pos 2.5 over 1 second
        StartCoroutine(MovePlayer());
    }

    void Update()
    {

        if (canMove)
        {
            // Sidescroller 2d player movement
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                transform.Translate(Vector3.right * speed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                transform.Translate(Vector3.left * speed * Time.deltaTime);
            }

            if (Input.GetKeyDown(KeyCode.Return) && arcadeMachineNumber == 1)
            {
                // Load first arcade game
                arcadeSceneManager.LoadFirstArcadeGame();
            }
        }
    }

    public IEnumerator MovePlayer()
    {
        transform.position = new Vector3(-12f, transform.position.y, transform.position.z);
        yield return new WaitForSeconds(0.3f);
        Vector3 startPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 endPos = new Vector3(-7.5f, transform.position.y, transform.position.z);
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("FirstArcadeMachine"))
        {
            arcadeMachineNumber = 1;
        }
        else if (collision.gameObject.CompareTag("SecondArcadeMachine"))
        {
            arcadeMachineNumber = 2;
        }
        else if (collision.gameObject.CompareTag("ThirdArcadeMachine"))
        {
            arcadeMachineNumber = 3;
        }
        else
        {
            arcadeMachineNumber = 0;
        }
    }

    public int getArcadeMachineNumber()
    {
        return arcadeMachineNumber;
    }
}
