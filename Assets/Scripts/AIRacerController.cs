using System.Collections.Generic;
using UnityEngine;

public class AIRacerController : MonoBehaviour
{
    public RacingNode currentNode;
    public RacingNode goalNode; // assign the last node here in the inspector
    public List<RacingNode> path = new List<RacingNode>();
    [SerializeField] private float speed = 5f;

    private void Start()
    {
        if (AStarManager.instance.startingNodes.Length > 0)
        {
            RacingNode closestNode = AStarManager.instance.startingNodes[0];
            float minDistance = Vector3.Distance(transform.position, closestNode.transform.position);

            // Check the rest of the nodes
            for (int i = 1; i < AStarManager.instance.startingNodes.Length; i++)
            {
                float dist = Vector3.Distance(transform.position, AStarManager.instance.startingNodes[i].transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closestNode = AStarManager.instance.startingNodes[i];
                }
            }

            currentNode = closestNode;
        }
        else
        {
            Debug.LogError("No starting nodes assigned to AI Racer!");
            return;
        }

        // Then generate the path from this starting node
        GenerateNewPath();
    }

    private void Update()
    {
        FollowPath();
    }

    private void FollowPath()
    {
        if (path.Count == 0)
        {
            GenerateNewPath();
            return;
        }

        RacingNode target = path[0];

        // Move in XZ, keep Y the same
        Vector3 targetPos = new Vector3(
            target.transform.position.x,
            transform.position.y,
            target.transform.position.z
        );

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            speed * Time.deltaTime
        );

        // XZ distance only
        Vector2 ai = new Vector2(transform.position.x, transform.position.z);
        Vector2 tgt = new Vector2(target.transform.position.x, target.transform.position.z);

        if (Vector2.Distance(ai, tgt) < 0.15f)
        {
            currentNode = target;
            path.RemoveAt(0);
        }
    }
    
    private void GenerateNewPath()
    {
        if (currentNode == null || goalNode == null)
        {
            Debug.LogError("Current node or goal node not assigned!");
            return;
        }

        var newPath = AStarManager.instance.GeneratePath(currentNode, goalNode);

        if (newPath == null || newPath.Count == 0)
        {
            Debug.LogWarning("AStar returned no path");
            return;
        }

        path = newPath;

        Debug.Log("New path length: " + newPath.Count);
        foreach (var n in newPath)
        {
            Debug.Log(" -> " + n.name);
        }
    }
}
