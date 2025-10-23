using UnityEngine;

public class SplitScreenRotator2D : MonoBehaviour
{
    public Material splitMat;
    public float rotationSpeed = 30f;
    private float angle = 0f;

    [SerializeField] RectTransform UILine;

    void Update()
    {
        angle += rotationSpeed * Time.deltaTime;
        if (angle > 360f) angle -= 360f;
        UILine.rotation = Quaternion.Euler(new Vector3(0,0,360-angle));
        splitMat.SetFloat("_Angle", angle);
    }
}
