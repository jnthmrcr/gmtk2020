using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour
{
	// controlled either by player or cpu

	public GridMap gMap;
	protected PawnMap pMap;

	public MainMap mainMap;
	public MapGrid personalMap;

	[SerializeField] protected int moveDist = 4;
	[SerializeField] protected Vector2Int wind;
	[SerializeField] protected int attackDist = 4;

	protected List<MapNode> navigableNodes;
	protected List<Vector2Int> targetablePoints;


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

	public void FindNavigableNodes(Vector3 startPos, int distance)
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

	public void FindTargettableNodes(Vector3 startpos, int distance, int windX = 0, int windY = 0)
	{
		int x = Mathf.RoundToInt(startpos.x) + windX - distance;
		int y = Mathf.RoundToInt(startpos.z) + windY;

		targetablePoints = new List<Vector2Int>();

		// get all the points
		for (int i = 0; i <= distance; i++) // x
		{
			int numPoints = i * 2 + 1;
			for (int j = 0; j < numPoints; j++) // y
			{
				targetablePoints.Add(new Vector2Int(x, y + j));
				if (i != distance) // if we are not at center
					targetablePoints.Add(new Vector2Int(x + ((distance - i) * 2), y + j)); // add second mirrored point
			}
			y--; // start one tile lower every tile
			x++; // start one tile to the right
		}

		RaycastHit hit;
		List<Vector2Int> final = new List<Vector2Int>();
		Vector3 castPoint = transform.position;
		castPoint = new Vector3(Mathf.RoundToInt(castPoint.x), 0f, Mathf.RoundToInt(castPoint.z));

		bool checkTopRight = !Physics.CheckSphere(castPoint + new Vector3(1, 0, 1), 0.1f);
		bool checkTopLeft = !Physics.CheckSphere(castPoint + new Vector3(-1, 0, 1), 0.1f);
		bool checkBottomRight = !Physics.CheckSphere(castPoint + new Vector3(1, 0, -1), 0.1f);
		bool checkBottomLeft = !Physics.CheckSphere(castPoint + new Vector3(-1, 0, -1), 0.1f);

		Vector3 pointTop = castPoint + new Vector3(0, 0, 1);
		Vector3 pointBottom = castPoint + new Vector3(0, 0, -1);
		Vector3 pointRight = castPoint + new Vector3(1, 0, 0);
		Vector3 pointLeft = castPoint + new Vector3(-1, 0, 0);

		bool checkTop = !Physics.CheckSphere(pointTop, 0.1f);
		bool checkBottom = !Physics.CheckSphere(pointBottom, 0.1f);
		bool checkRight = !Physics.CheckSphere(pointRight, 0.1f);
		bool checkLeft = !Physics.CheckSphere(pointLeft, 0.1f);

		// raycast check all points
		for (int i = 0; i < targetablePoints.Count; i++)
		{
			bool targetable = true;
			if (Physics.Linecast(castPoint, targetablePoints[i].toV3(), out hit))
			{	
				//if (Vector3.Distance(hit.point, targetablePoints[i].toV3()) > 1)
				if (Vector3.SqrMagnitude(hit.point - targetablePoints[i].toV3()) >= 0.55f) // slightly more than srt2 dist
				{
					targetable = false;	// not close enough to count
				}
			}

			if (!targetable) // only do this next check if node is not accessible normally
			{
				if (checkTop && !Physics.Linecast(pointTop, targetablePoints[i].toV3()) && checkTopLeft && checkTopRight)
				{
					if (targetablePoints[i].y >= pointTop.z)
						targetable = true;
				}
				if (checkBottom && !Physics.Linecast(pointBottom, targetablePoints[i].toV3()) && checkBottomLeft && checkBottomRight)
				{
					if (targetablePoints[i].y <= pointBottom.z)
						targetable = true;
				}
				if (checkRight && !Physics.Linecast(pointRight, targetablePoints[i].toV3()) && checkTopRight && checkBottomRight)
				{
					if (targetablePoints[i].x >= pointRight.x)
						targetable = true;
				}
				if (checkLeft && !Physics.Linecast(pointLeft, targetablePoints[i].toV3()) && checkTopLeft && checkBottomLeft)
				{
					if (targetablePoints[i].x <= pointLeft.x)
						targetable = true;
				}
			}

			if (targetable)
				final.Add(targetablePoints[i]);
		}
		targetablePoints = final;// it shouldn't need to be cleared since we're overriding it ????
	}
}
