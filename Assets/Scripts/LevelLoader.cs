using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;
    public float firstTTime = 4f;

    [SerializeField] GameObject Player;
    [SerializeField] Transform lerpTrans;


    public void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator LoadArcadeGame(int levelIndex)
    {
        Player.GetComponent<PlayerMovement>().canMove = false;
        Player.GetComponent<PlayableDirector>().Play();

        if (SFXManager.Instance)
            SFXManager.Instance.PlaySFX(8);
        GameObject.Find("GlowThingy").GetComponent<ParticleSystem>().Play();

        Vector3 startPos = Player.transform.position;
        float elapsed = 0f;
        float duration = 1f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float newX = Mathf.Lerp(startPos.x, lerpTrans.position.x, elapsed / duration);
            Player.transform.position = new Vector3(newX, startPos.y, startPos.z);

            yield return null;
        }

        // Snap exactly to target X at the end
        Player.transform.position = new Vector3(
            lerpTrans.position.x,
            startPos.y,
            startPos.z
        );

        yield return new WaitForSeconds(firstTTime);
        
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        if (levelIndex == 1)
            SceneManager.LoadScene("PinballTutorial");
    }
}
