using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] AudioSource PlaySFX;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPressPlay()
    {
        //Load the 1st cutscene
        SceneManager.LoadScene("Pinball", LoadSceneMode.Single);
        SFXManager.Instance.PlaySFX(0);
    }
}
