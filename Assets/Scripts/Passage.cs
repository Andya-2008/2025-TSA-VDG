using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Passage : MonoBehaviour
{
    public Transform connection;

    bool left;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Vector3 position = connection.position;
        position.z = other.transform.position.z;
        other.transform.position = position;
        if (left)
            other.GetComponent<Movement>().SetDirection(Vector2.left);

        if (!left)
            other.GetComponent<Movement>().SetDirection(Vector2.right);
    }

}
