using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Pawn : MonoBehaviour
{
	// controlled either by player or cpu

	public MainMap mainMap;
	public MapGrid personalMap;

	[SerializeField] protected int moveDist = 4;
	[SerializeField] protected Vector2Int wind;
	[SerializeField] protected int attackDist = 4;

	public List<MapNode> navigableNodes;
	public List<Vector2Int> targetablePoints;

	private int _hitPoints;
	[SerializeField] protected TextMeshPro hp;

	protected int HitPoints
	{
		get { return _hitPoints; }
		set
		{
			_hitPoints = value;
			if (hp != null)
				hp.text = _hitPoints.ToString();
		}
	}

	bool performingAction;

	protected virtual void Start()
	{
		HitPoints = 1;
	}

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

	public void Move(Vector3 worldPosition)
	{
		performingAction = true;
		MapNode targetNode = personalMap.NodeFromWorldPosition(worldPosition, transform.position);
		int pathLength = targetNode.cost + 1;
		//print(worldPosition + " " + targetNode.worldPosition);
		Vector3[] nodePath = new Vector3[pathLength];
		// get a path
		for (int i = 0; i < pathLength; i++)
		{
			nodePath[i] = targetNode.worldPosition;
			//Debug.DrawLine(targetNode.worldPosition, targetNode.parent.worldPosition, Color.cyan, 3f);
			targetNode = targetNode.parent; // go up the chain
			print("hi");
		}
		print(nodePath);
		StartCoroutine(MoveSequence(nodePath, worldPosition));
	}

	IEnumerator MoveSequence(Vector3[] path, Vector3 dest)
	{
		float progress = path.Length - 1;
		int ceil;
		// foreach (Vector3 p in path)
		// {
		// 	print(p);
		// }
		while (progress > 0)
		{
			progress -= Time.deltaTime * 4f;
			//print(progress);
			ceil = Mathf.CeilToInt(progress);
			transform.position = Vector3.Lerp(path[ceil], path[Mathf.Max(ceil - 1, 0)], ceil - progress);
			//Debug.DrawLine(path[floor], path[Mathf.Min(floor + 1, path.Length - 1)], Color.cyan, 0.1f);
			yield return null;
		}
		GetPersonalMap(transform.position, moveDist);
		FindNavigableNodes(transform.position, moveDist);
		SetActivePawn();

		performingAction = false;
		yield break;
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

	int GetDistance(MapNode nodeA, MapNode nodeB)
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

		if (x >= 0 && x < personalMap.sizeX && y >= 0 && y < personalMap.sizeY) // is it actually in the grid
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
		Vector3 castPoint = startpos;
		castPoint = new Vector3(Mathf.RoundToInt(castPoint.x), 0f, Mathf.RoundToInt(castPoint.z));

		Vector3 pointTop = castPoint + new Vector3(0, 0, 1);
		Vector3 pointBottom = castPoint + new Vector3(0, 0, -1);
		Vector3 pointRight = castPoint + new Vector3(1, 0, 0);
		Vector3 pointLeft = castPoint + new Vector3(-1, 0, 0);

		// true if clear
		bool checkTop = !Physics.CheckSphere(pointTop, 0.1f);
		bool checkBottom = !Physics.CheckSphere(pointBottom, 0.1f);
		bool checkRight = !Physics.CheckSphere(pointRight, 0.1f);
		bool checkLeft = !Physics.CheckSphere(pointLeft, 0.1f);

		bool checkTopRight = !Physics.CheckSphere(castPoint + new Vector3(1, 0, 1), 0.1f);
		bool checkTopLeft = !Physics.CheckSphere(castPoint + new Vector3(-1, 0, 1), 0.1f);
		bool checkBottomRight = !Physics.CheckSphere(castPoint + new Vector3(1, 0, -1), 0.1f);
		bool checkBottomLeft = !Physics.CheckSphere(castPoint + new Vector3(-1, 0, -1), 0.1f);

		// raycast check all points
		for (int i = 0; i < targetablePoints.Count; i++)
		{
			Vector2Int targetablePoint = targetablePoints[i];

			if (!LineCastTest(castPoint, targetablePoint)) // only do these next checks if node is not accessible normally
			{
				if (checkTop && targetablePoints[i].y > castPoint.z)
				{
					if (checkTopLeft && targetablePoint.x < castPoint.x)
					{
						if (LineCastTest(pointTop, targetablePoint, true))
							continue;
					}
					else if (checkTopRight && targetablePoints[i].x > castPoint.x)
					{
						if (LineCastTest(pointTop, targetablePoint, true))
							continue;
					}
				}
				if (checkBottom && targetablePoint.y < castPoint.z)
				{
					if (checkBottomLeft && targetablePoint.x < castPoint.x)
					{
						if (LineCastTest(pointBottom, targetablePoint, true))
							continue;
					}
					else if (checkBottomRight && targetablePoints[i].x > castPoint.x)
					{
						if (LineCastTest(pointBottom, targetablePoint, true))
							continue;
					}
				}
				if (checkRight && targetablePoint.x > castPoint.x)
				{
					//targetable = CornerLineCast(pointRight, targetablePoints[i].toV3());
					//trying to get it to not select a thing if we're resting on a corner/ fuck.
					if (checkTopRight && targetablePoint.y > castPoint.z)
					{
						if (LineCastTest(pointRight, targetablePoint, true))
							continue;
					}
					else if (checkBottomRight && targetablePoint.y < castPoint.z)
					{
						if (LineCastTest(pointRight, targetablePoint, true))
							continue;
					}
				}
				if (checkLeft && targetablePoint.x < castPoint.x)
				{
					if (checkTopLeft && targetablePoint.y > castPoint.z)
					{
						if (LineCastTest(pointLeft, targetablePoint, true))
							continue;
					}
					else if (checkBottomLeft && targetablePoint.y < castPoint.z)
					{
						if (LineCastTest(pointLeft, targetablePoint, true))
							continue;
					}
				}
			}
		}
		targetablePoints = final; // it shouldn't need to be cleared since we're overriding it ????

		bool LineCastTest(Vector3 point, Vector2Int target, bool debug = false)
		{
			Vector3 target3 = target.toV3();

			if (Physics.Linecast(point, target3, out hit))
			{
				if (Vector3.SqrMagnitude(hit.point - target3) >= 0.55f) // slightly more than srt2 dist
					return false; // not close enough to count
				else
				{
					if (debug)
						Debug.DrawLine(point, target3);
					final.Add(target);
					return true;
				}
			}
			else
			{
				if (debug)
					Debug.DrawLine(point, target3);
				final.Add(target);
				return true;
			}
		}
	}

	public virtual void PhaseInit(int windX, int windY)
	{
		GetPersonalMap(transform.position, moveDist);
		FindNavigableNodes(transform.position, moveDist);
		FindTargettableNodes(transform.position, attackDist, windX, windY);
	}

	public virtual void SetActivePawn()
	{

	}
}