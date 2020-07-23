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

	public MainMap mainMap;
	public MapGrid personalMap;

	[SerializeField] protected int moveDist = 4;
	protected int attackDist = 4;

	protected List<MapNode> navigableNodes;

	public void GetPersonalMap(Vector3 startPos, int distance)
	{
		MapNode startNode = mainMap.grid.NodeFromWorldPosition(startPos + new Vector3(-distance, 0, -distance), Vector3.zero);
		personalMap = mainMap.grid.GetSubMap(
			distance * 2 + 1,
			distance * 2 + 1,
			startNode.gridX,
			startNode.gridY,
			mainMap.grid);
	}

	public void Move(Vector2 newPosition)
	{

	}

	public void FindNavigableNodes(Vector3 startPos, int distance, int windX = 0, int windY = 0)
	{
		MapNode startNode = personalMap.NodeFromWorldPosition(startPos, transform.position);
		Debug.DrawLine(startNode.worldPosition, startNode.worldPosition + Vector3.up, Color.cyan);
		startNode.cost = 0; // cost should already be zero but whatever
		//startNode.windCostX = windX;
		//startNode.windCostY = windY;

		navigableNodes = new List<MapNode>(); // list of nodes we can navigate to, nodes have parent and cost
		List<MapNode> nodesToEval = new List<MapNode>(); // nodes to evaluate
		List<MapNode> neighbors = new List<MapNode>(); // neighbors of evaluated nodes

		nodesToEval.Add(startNode); // add start node to navigable nodes

		for (int cost = 1; cost < moveDist + 1; cost++)
		{
			// get neighbors of eval nodes
			for (int i = 0; i < nodesToEval.Count; i++)
			{
				// check 4 neighbors of current nodetoeval, add to navigable nodes if they pass all tests
				CheckNeighbor(ref neighbors, nodesToEval[i], 1, 0, cost);
				CheckNeighbor(ref neighbors, nodesToEval[i], -1, 0, cost);
				CheckNeighbor(ref neighbors, nodesToEval[i], 0, 1, cost);
				CheckNeighbor(ref neighbors, nodesToEval[i], 0, -1, cost);
				// now we have a list of neighbors that pass, and are ready for next loop
			}

			nodesToEval.Clear();
			nodesToEval.AddRange(neighbors); // evaluate our neighbors next loop
			neighbors.Clear();
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

	void CheckNeighbor(ref List<MapNode> nodelist, MapNode node, int offX, int offY, int cost)
	{
		// get personal grid coordiantes of neighbor
		int x = node.gridX + offX;
		int y = node.gridY + offY;

		MapNode neighbor = personalMap.nodes[x, y];

		if (x >= 0 && x < personalMap.sizeX && y >= 0 && y < personalMap.sizeY)// is it actually in the grid
		{
			if (neighbor.walkable) // is it walkable
			{
				if (neighbor.cost > cost) // is it the shortest path to this node
				{
					// we good fam
					neighbor.parent = node;
					neighbor.cost = cost;

					nodelist.Add(neighbor);
					navigableNodes.Add(neighbor);
				}
			}
		}
	}
}
