using UnityEngine;

public class RotatingSplitLine : MonoBehaviour
{
    [Header("Rotation")]
    public float rotationSpeed = 25f; // degrees per second
    public bool rotateClockwise = true;

    [Header("Level Control")]
    public int rotateOnLevel = 2;

    private GameManager _gm;

    void Start()
    {
        var gmObj = GameObject.Find("GameManager");
        if (gmObj != null)
            _gm = gmObj.GetComponent<GameManager>();
    }

    void Update()
    {
        if (_gm == null) return;
        if (_gm.level != rotateOnLevel) return;

        float dir = rotateClockwise ? -1f : 1f;
        transform.Rotate(0f, 0f, dir * rotationSpeed * Time.deltaTime);
    }
}