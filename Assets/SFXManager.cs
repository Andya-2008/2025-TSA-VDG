using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    private static SFXManager _instance;

    public static SFXManager Instance
    {
        get
        {
            // If the instance hasn't been assigned yet, find it in the scene
            if (_instance == null)
                _instance = FindFirstObjectByType<SFXManager>();
            return _instance;
        }
    }

    void Awake()
    {
        // Cache it early if possible
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(gameObject);
    }
    [SerializeField] List<AudioSource> sfx = new List<AudioSource>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySFX(int sfxIndex)
    {
        sfx[sfxIndex].Play();
    }
}
