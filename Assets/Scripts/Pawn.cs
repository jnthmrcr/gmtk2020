using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Pawn : MonoBehaviour
{
	// controlled either by player or cpu

	public GridMap gMap;
	protected PawnMap pMap;
	protected int moveDist = 4;
	protected int attackDist = 4;

	protected List<GridNode> navigableNodes;
	protected List<GridNode> attackableNodes;

    public void GetPMap(Vector3 startPos, int distance, int windX = 0, int windY = 0)
	{
		// get a map to search through
		GridNode startNode = gMap.NodeFromWorldPosition(startPos);
		pMap = gMap.GrabPawnMap(
			startNode.gridX - (Mathf.Max(distance + windX, distance - windX)), // min x
			startNode.gridY - (Mathf.Max(distance + windY, distance - windY)), // min y
			distance * 2 + 1 + Mathf.Abs(windX), // size x
			distance * 2 + 1 + Mathf.Abs(windY)); // size y

		Debug.DrawLine(startNode.worldPosition, startNode.worldPosition + Vector3.up, Color.red, 0.1f);
		startNode = pMap.NodeFromWorldPosition(startNode.worldPosition, transform.position);
		Debug.DrawLine(startNode.worldPosition, startNode.worldPosition + Vector3.up, Color.green, 0.1f);


		// draw nav
		FindNavigableNodes(startPos, distance, windX, windY);
	}

    public void Move(Vector2 newPosition)
	{

	}

	public void FindNavigableNodes(Vector3 startPos, int distance, int windX = 0, int windY = 0)
	{
		GridNode startNode = pMap.NodeFromWorldPosition(startPos, transform.position);
		startNode.cost = 0; // cost should already be zero but whatever
		startNode.windCostX = windX;
		startNode.windCostY = windY;

		navigableNodes = new List<GridNode>(); // list of nodes we can navigate to, nodes have parent and cost
		List<GridNode> nodesToEval = new List<GridNode>(); // nodes to evaluate
		List<GridNode> neighbors = new List<GridNode>(); // neighbors of evaluated nodes

		nodesToEval.Add(startNode); // add start node to navigable nodes
		int cost = 1;

		for (int i = 0; i < nodesToEval.Count; i++)
		{
			neighbors.AddRange(pMap.GetImmediateNeighbors(nodesToEval[i]));
		}

		foreach (GridNode n in neighbors)
		{
			// cost test
		}

		navigableNodes.AddRange(neighbors);
		
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
