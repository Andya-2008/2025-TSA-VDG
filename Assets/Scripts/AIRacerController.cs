using System.Collections.Generic;
using UnityEngine;

public class AIRacerController : MonoBehaviour
{
    public RacingNode currentNode;
    public List<RacingNode> path = new List<RacingNode>();

    private void Update()
    {
        CreatePath();
    }

    public void CreatePath()
    {
        RacingNode targetNode = FindClosestNode();

        if (targetNode != null && currentNode != targetNode)
        {
            path = AStarManager.instance.GeneratePath(currentNode, targetNode);
        }

        // Use kartcontroller movement to follow path
        if (path.Count > 0)
        {
            Vector3 targetPosition = path[0].transform.position;
            Vector3 direction = (targetPosition - transform.position).normalized;

            float angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);

            KartController kartController = GetComponent<KartController>();

            if (angle > 5f)
            {
                kartController.Steer(1, Mathf.Abs(angle) / 90f);
            }
            else if (angle < -5f)
            {
                kartController.Steer(-1, Mathf.Abs(angle) / 90f);
            }

            kartController.speed = kartController.acceleration;

            if (Vector3.Distance(transform.position, targetPosition) < 1f)
            {
                path.RemoveAt(0);
            }
        }
    }

    private RacingNode FindClosestNode()
    {
        RacingNode closestNode = null;
        float closestDistance = float.MaxValue;

        foreach (RacingNode node in FindObjectsByType<RacingNode>(FindObjectsSortMode.None))
        {
            float distance = Vector2.Distance(transform.position, node.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNode = node;
            }
        }

        return closestNode;
    }
}
