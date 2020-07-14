using System;
using System.Collections;
using System.Collections.Generic;
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

		navigableNodes = new List<GridNode>();
		attackableNodes = new List<GridNode>();
		List<GridNode> neighbors;

		navigableNodes.Add(startNode); // add start node to navigable nodes
		navigableNodes.AddRange(pMap.GetImmediateNeighbors(startNode));

		for (int i = 0; i < navigableNodes.Count; i++) // create list of navigable nodes, starting with start node
		{
			// get neighbors of current navigableNode
			neighbors = pMap.GetImmediateNeighbors(navigableNodes[i]);
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

	public void FindAttackableNodes(Vector3 startPos, int distance, int windX = 0, int windY = 0)
	{
		GridNode startNode = pMap.NodeFromWorldPosition(startPos, transform.position);
		startNode.cost = 0; // cost should already be zero but whatever
		startNode.windCostX = windX;
		startNode.windCostY = windY;

		navigableNodes = new List<GridNode>();
		attackableNodes = new List<GridNode>();
		List<GridNode> neighbors;

		navigableNodes.Add(startNode); // add start node to navigable nodes

		for (int i = 0; i < navigableNodes.Count; i++) // create list of navigable nodes, starting with start node
		{
			// get neighbors of current navigableNode
			neighbors = pMap.GetImmediateNeighbors(navigableNodes[i]);
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
