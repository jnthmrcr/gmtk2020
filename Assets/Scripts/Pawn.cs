using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Pawn : MonoBehaviour
{
	// controlled either by player or cpu

	public MapGrid personalMap;
	[SerializeField] protected LayerMask targettingObstacle;
	[SerializeField] protected int moveDist = 4;
	public int MoveDist { get => moveDist; }

	[SerializeField] protected Vector2Int wind;
	[SerializeField] protected int attackDist = 4;
	public int AttackDist { get => attackDist; }

	public List<MapNode> navigableNodes;
	public List<Vector3> targetablePoints;
	public List<Vector3> targetableDamageTakerPoints;

	private int _hitPoints;
	[SerializeField] protected TextMeshPro hp;
	[SerializeField] protected Transform pawnMesh;

	private Vector2Int[] possibleTargetsOffsets;

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

	protected virtual void Awake()
	{
		GeneratePossibleTargetsOffsets();
	}

	protected virtual void Start()
	{
		HitPoints = 1;
	}

	public void GetPersonalMap(Vector3 startPos, int distance)
	{
		MapNode startNode = GameManager.self.mainMap.grid.NodeFromWorldPosition(startPos + new Vector3(-distance, 0, -distance), Vector3.zero);
		personalMap = GameManager.self.mainMap.grid.GetSubMap(
			distance * 2 + 1,
			distance * 2 + 1,
			startNode.gridX,
			startNode.gridY,
			GameManager.self.mainMap.grid);
	}

	public void Move(Vector3 worldPosition)
	{
		performingAction = true;

		MapNode targetNode;
		targetNode = personalMap.NodeFromWorldPosition(worldPosition, transform.position);

		int pathLength = targetNode.cost + 1;

		//print(worldPosition + " " + targetNode.worldPosition);
		Vector3[] nodePath = new Vector3[pathLength];
		// get a path
		for (int i = 0; i < pathLength; i++)
		{
			nodePath[i] = targetNode.worldPosition;
			//Debug.DrawLine(targetNode.worldPosition, targetNode.parent.worldPosition, Color.cyan, 3f);
			targetNode = targetNode.parent; // go up the chain
		}
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
			progress -= Time.deltaTime * 8f;
			//print(progress);
			ceil = Mathf.CeilToInt(progress);
			transform.position = Vector3.Lerp(path[ceil], path[Mathf.Max(ceil - 1, 0)], ceil - progress);
			//Debug.DrawLine(path[floor], path[Mathf.Min(floor + 1, path.Length - 1)], Color.cyan, 0.1f);
			//pawnMesh.localScale = Vector3.Lerp(new Vector3(1f, 0.2f, 1f), new Vector3(0.65f, 0.2f, 0.65f), progress % 1f);
			//transform.localScale = Vector3.one * Mathf.Lerp(1f, 0.65f, progress % 1f);
			yield return null;
		}

		// rescan starting and ending nodes
		GameManager.self.mainMap.RescanNodeAtPoint(transform.position);
		GameManager.self.mainMap.RescanNodeAtPoint(path[path.Length - 1]);

		RescanPawn();

		performingAction = false;
		yield break;
	}

	public void RescanPawn()
	{
		GetPersonalMap(transform.position, moveDist);
		FindNavigableNodes(transform.position, moveDist);
		FindTargettableNodes(transform.position, attackDist);
		SetActivePawn();
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

	void CheckNeighbor(ref List<MapNode> nodelist, MapNode node, int offX, int offY, int cost)
	{
		// get personal grid coordiantes of neighbor
		int x = node.gridX + offX;
		int y = node.gridY + offY;

		MapNode neighbor = personalMap.nodes[x, y];

		if (x >= 0 && x < personalMap.sizeX && y >= 0 && y < personalMap.sizeY) // is it actually in the grid
		{
			if (neighbor.walkable && neighbor.pawnOnNode == MapNode.pawnType.none) // is it walkable
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

	int GetDistance(MapNode nodeA, MapNode nodeB)
	{
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if (dstX > dstY)
			return 2 * dstY + (dstX - dstY);
		return 2 * dstX + (dstY - dstX);
	}

	int GetDistance(Vector3 a, Vector3 b)
	{
		int distX = (int) Mathf.Abs(a.x - b.x);
		int distY = (int) Mathf.Abs(a.z - b.z);

		return distX + distY;
	}

	/// <summary> caches possibleTargetsOffsets for use in FindTargetableNodes</summary>
	void GeneratePossibleTargetsOffsets()
	{
		possibleTargetsOffsets = new Vector2Int[41];
		int index = 0;

		int x = -attackDist;
		int y = 0;

		// get all the points
		for (int i = 0; i <= attackDist; i++) // x
		{
			int numPoints = i * 2 + 1;
			for (int j = 0; j < numPoints; j++) // y
			{
				possibleTargetsOffsets[index] = new Vector2Int(x, y + j);
				index++;
				if (i != attackDist) // if we are not at center
				{
					possibleTargetsOffsets[index] = new Vector2Int(x + ((attackDist - i) * 2), y + j);
					index++;
				}
			}
			y--; // start one tile lower every tile
			x++; // start one tile to the right
		}
	}

	public void FindTargettableNodes(Vector3 startpos, int distance, int windX = 0, int windY = 0)
	{
		int x = Mathf.RoundToInt(startpos.x) + windX;
		int y = Mathf.RoundToInt(startpos.z) + windY;

		targetablePoints = new List<Vector3>();
		targetableDamageTakerPoints = new List<Vector3>();

		RaycastHit hit;
		Vector3 castPoint = startpos;
		castPoint = new Vector3(Mathf.RoundToInt(castPoint.x), 0f, Mathf.RoundToInt(castPoint.z));

		Vector3 pointTop = castPoint + new Vector3(0, 0, 1);
		Vector3 pointBottom = castPoint + new Vector3(0, 0, -1);
		Vector3 pointRight = castPoint + new Vector3(1, 0, 0);
		Vector3 pointLeft = castPoint + new Vector3(-1, 0, 0);

		// true if clear, could also be checked by accessing map's walkable field
		bool checkTop = !Physics.CheckSphere(pointTop, 0.1f, targettingObstacle);
		bool checkBottom = !Physics.CheckSphere(pointBottom, 0.1f, targettingObstacle);
		bool checkRight = !Physics.CheckSphere(pointRight, 0.1f, targettingObstacle);
		bool checkLeft = !Physics.CheckSphere(pointLeft, 0.1f, targettingObstacle);

		bool checkTopRight = !Physics.CheckSphere(castPoint + new Vector3(1, 0, 1), 0.1f, targettingObstacle);
		bool checkTopLeft = !Physics.CheckSphere(castPoint + new Vector3(-1, 0, 1), 0.1f, targettingObstacle);
		bool checkBottomRight = !Physics.CheckSphere(castPoint + new Vector3(1, 0, -1), 0.1f, targettingObstacle);
		bool checkBottomLeft = !Physics.CheckSphere(castPoint + new Vector3(-1, 0, -1), 0.1f, targettingObstacle);

		// raycast check all points
		for (int i = 0; i < possibleTargetsOffsets.Length; i++)
		{
			Vector2Int targetablePoint = new Vector2Int(x + possibleTargetsOffsets[i].x, y + possibleTargetsOffsets[i].y);

			if (!LineCastTest(castPoint, targetablePoint)) // only do these next checks if node is not accessible normally
			{
				if (checkTop && targetablePoint.y > castPoint.z)
				{
					if (checkTopLeft && targetablePoint.x < castPoint.x)
					{
						if (LineCastTest(pointTop, targetablePoint, true))
							continue;
					}
					else if (checkTopRight && targetablePoint.x > castPoint.x)
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
					else if (checkBottomRight && targetablePoint.x > castPoint.x)
					{
						if (LineCastTest(pointBottom, targetablePoint, true))
							continue;
					}
				}
				if (checkRight && targetablePoint.x > castPoint.x)
				{
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

		bool LineCastTest(Vector3 point, Vector2Int target, bool debug = false)
		{
			Vector3 target3 = target.toV3();

			if (Physics.Linecast(point, target3, out hit, targettingObstacle))
			{
				if (Vector3.SqrMagnitude(hit.point - target3) >= 0.55f) // slightly more than srt2 dist
					return false; // not close enough to count
				else
				{
					if (debug)
						Debug.DrawLine(point, target3);
					AddTargetableNodeFromPoint(target3);
					return true;
				}
			}
			else
			{
				if (debug)
					Debug.DrawLine(point, target3);
				AddTargetableNodeFromPoint(target3);
				return true;
			}
		}

		void AddTargetableNodeFromPoint(Vector3 point3)
		{
			targetablePoints.Add(point3);
			// check to see if node has a damagetaker
			Collider[] hits = Physics.OverlapSphere(point3, 0.1f, targettingObstacle);
			if (hits.Length > 0)
			{
				DamageTaker dt = hits[0].GetComponent<DamageTaker>();
				if (dt != null)
				{
					targetableDamageTakerPoints.Add(dt.transform.position);
				}
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