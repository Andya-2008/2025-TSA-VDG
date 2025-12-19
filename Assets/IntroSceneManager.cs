using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSceneManager : MonoBehaviour
{
    private bool isOutside = true;
    private bool isTouchingDoor = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (isOutside)
        {
            if (isTouchingDoor && Input.GetKeyDown(KeyCode.Return))
            {
                // Transition to inside scene
                Debug.Log("Entering the arcade...");
                isOutside = false;
                isTouchingDoor = false;

                // Load inside scene
                SceneManager.LoadScene("IntroSceneArcade");
            }
        }

        Debug.Log(isTouchingDoor);
    }

    public void SetIsOutside(bool outside)
    {
        isOutside = outside;
    }

    public void SetTouchingDoor(bool touching)
    {
        isTouchingDoor = touching;
    }
}
