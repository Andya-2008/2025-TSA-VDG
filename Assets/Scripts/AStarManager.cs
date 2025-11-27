using System.Collections.Generic;
using UnityEngine;

public class AStarManager : MonoBehaviour
{
    public static AStarManager instance;
    public RacingNode[] startingNodes;

    private void Awake()
    {
        instance = this;
    }

    public List<RacingNode> GeneratePath(RacingNode start, RacingNode end)
    {
        if (start == null || end == null)
        {
            Debug.LogError("Start or end node is null!");
            return null;
        }

        // Reset all nodes before starting
        RacingNode[] allNodes = FindObjectsByType<RacingNode>(FindObjectsSortMode.None);
        foreach (RacingNode n in allNodes)
        {
            n.gScore = float.MaxValue;
            n.hScore = 0f;
            n.cameFrom = null;
        }

        List<RacingNode> openSet = new List<RacingNode>();

        start.gScore = 0f;
        start.hScore = Vector2.Distance(
            new Vector2(start.transform.position.x, start.transform.position.z),
            new Vector2(end.transform.position.x, end.transform.position.z)
        );
        openSet.Add(start);

        while (openSet.Count > 0)
        {
            // Find node in openSet with lowest FScore
            int lowestF = 0;
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FScore() < openSet[lowestF].FScore())
                    lowestF = i;
            }

            RacingNode currentNode = openSet[lowestF];
            openSet.Remove(currentNode);

            if (currentNode == end)
            {
                // Reconstruct path from end to start
                List<RacingNode> path = new List<RacingNode>();
                RacingNode temp = currentNode;
                while (temp != null)
                {
                    path.Add(temp);
                    temp = temp.cameFrom;
                }
                path.Reverse();
                return path;
            }

            foreach (RacingNode neighbor in currentNode.connections)
            {
                float tentativeG = currentNode.gScore + Vector2.Distance(
                    new Vector2(currentNode.transform.position.x, currentNode.transform.position.z),
                    new Vector2(neighbor.transform.position.x, neighbor.transform.position.z)
                );

                if (tentativeG < neighbor.gScore)
                {
                    neighbor.cameFrom = currentNode;
                    neighbor.gScore = tentativeG;
                    neighbor.hScore = Vector2.Distance(
                        new Vector2(neighbor.transform.position.x, neighbor.transform.position.z),
                        new Vector2(end.transform.position.x, end.transform.position.z)
                    );

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        // No path found
        Debug.LogWarning("AStar could not find a path from " + start.name + " to " + end.name);
        return null;
    }
}