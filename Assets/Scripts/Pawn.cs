using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Pawn : MonoBehaviour
{
	// controlled either by player or cpu

	GridMap gMap;
	PawnMap pMap;
    Vector2 position;
	protected int moveDist = 4;

    public void GetMoveableArea(Vector3 startPos, int distance, int windX = 0, int windY = 0)
	{
		// get a map to search through
		GridNode startNode = gMap.NodeFromWorldPosition(startPos);
		pMap = gMap.GrabPawnMap(
			startNode.gridX - (Mathf.Max(distance + windX, distance - windX)), // min x
			startNode.gridY - (Mathf.Max(distance + windY, distance - windY)), // min y
			distance * 2 + 1 + Mathf.Abs(windX), // size x
			distance * 2 + 1 + Mathf.Abs(windY)); // size y

		// draw nav
		FindNavigableNodes(startPos, distance, windX, windY);
	}

    public void Move(Vector2 newPosition)
	{

	}

	public void FindNavigableNodes(Vector3 startPos, int distance, int windX = 0, int windY = 0)
	{
		GridNode startNode = pMap.NodeFromWorldPosition(startPos);
		startNode.cost = 0; // cost should already be zero but whatever

		List<GridNode> navigableNodes = new List<GridNode>();

		navigableNodes.Add(startNode); // add start node to navigable nodes

		for (int i = 0; i < navigableNodes.Count; i++) // create list of navigable nodes, starting with start node
		{
			// get neighbors of current navigableNode
			List<GridNode> neighbors = pMap.GetImmediateNeighbors(navigableNodes[i]);
			for (int j = 0; j < neighbors.Count; j++)
			{
				// is neighbor walkable
				if (neighbors[j].walkable)
				{
					int newCost = navigableNodes[i].cost++; // what the neighbor could cost
					if (newCost <= moveDist) // if the newcost is navigable
					{
						// does neighbor have cost/parent
						if (neighbors[j].cost > newCost) // if new cost is lower, replace old cost/parent
						{
							neighbors[j].cost = newCost;
							neighbors[j].parent = navigableNodes[i].parent;
							navigableNodes.Add(neighbors[j]); // neighbors have passed all test, can be added to navigable nodes safely
						}
					}
				}
			}
		}
	}

	int GetDistance(GridNode nodeA, GridNode nodeB)
	{
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if (dstX > dstY)
			return 2 * dstY + (dstX - dstY);
		return 2 * dstX + (dstY - dstX);
	}
}
