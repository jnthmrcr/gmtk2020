﻿using System;
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

	public MainMap mainMap;
	public MapGrid personalMap;

	[SerializeField] protected int moveDist = 4;
	protected int attackDist = 4;

	protected List<GridNode> navigableNodes;
	protected List<GridNode> attackableNodes;

	public void GetPersonalMap(Vector3 startPos, int distance)
	{
		MapNode startNode = mainMap.grid.NodeFromWorldPosition(startPos + new Vector3(-distance, 0, -distance));
		//print("startnotede " + startNode.gridX + " " + startNode.gridY);
		personalMap = mainMap.grid.GetSubMap(
			distance * 2 + 1,
			distance * 2 + 1,
			startNode.gridX,
			startNode.gridY,
			mainMap.grid);

		//Debug.DrawLine(startNode.worldPosition, startNode.worldPosition + Vector3.up, Color.red, 0.1f);
		//startNode = personalMap.NodeFromWorldPosition(startNode.worldPosition);
		//Debug.DrawLine(startNode.worldPosition, startNode.worldPosition + Vector3.up, Color.green, 0.1f);

		//foreach(MapNode n in personalMap.nodes)
		//{
			//Debug.DrawLine(n.worldPosition + Vector3.one * 0.25f, n.worldPosition + Vector3.one * 0.25f + Vector3.up, Color.green, 0.1f);
		//}
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

		for (int cost = 1; cost < moveDist + 1; cost++)
		{
			// get neighbors of eval nodes
			for (int i = 0; i < nodesToEval.Count; i++)
			{
				neighbors.AddRange(pMap.GetImmediateNeighbors(nodesToEval[i]));
			}

			nodesToEval.Clear();

			print(neighbors.Count);
			// cost test
			foreach (GridNode n in neighbors)
			{
				//if (n.walkable)
				//{
					//if (n.cost > cost) // if a nodes previous cost is greater than the new cost
					//{
						// replace cost and parent
						//n.cost = cost;
						//n.parent =    fuck...

						//nodesToEval.Add(n);
						//navigableNodes.Add(n);
					//print(navigableNodes.Count);
					//}
				//}
				//neighbors.Remove(n);
			}

			for (int i = 0; i < neighbors.Count; i++)
			{
				if (neighbors[i].walkable)
				{
				navigableNodes.Add(neighbors[i]);
				nodesToEval.Add(neighbors[i]);
				neighbors.Remove(neighbors[i]);
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
