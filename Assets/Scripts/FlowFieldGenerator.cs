using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowFieldGenerator : MonoBehaviour
{
	[SerializeField] MainMap mainMap;
	MapGrid personalMap;

	public List<MapNode> flowNodes = new List<MapNode>();

	[SerializeField] Transform[] startTransforms, endTransforms;

	private void Start()
	{
		Vector2Int[] startPoints = new Vector2Int[startTransforms.Length];
		for (int i = 0; i < startPoints.Length; i++)
		{
			startPoints[i] = startTransforms[i].position.toV2i();
		}

		Vector2Int[] endPoints = new Vector2Int[endTransforms.Length];
		for (int i = 0; i < endPoints.Length; i++)
		{
			endPoints[i] = endTransforms[i].position.toV2i();
		}

		GenerateField(startPoints, endPoints);
		print(flowNodes.Count);
	}

	public void GenerateField(Vector2Int[] startPoints, Vector2Int[] endPoints)
	{
		personalMap = mainMap.grid.GetSubMap(mainMap.grid.sizeX, mainMap.grid.sizeY, mainMap.grid.minX, mainMap.grid.minY, mainMap.grid);

		MapNode[] startNodes = new MapNode[startPoints.Length];
		for (int i = 0; i < startNodes.Length; i++)
		{
			startNodes[i] = personalMap.NodeFromWorldPosition(startPoints[i].toV3(), Vector3.zero);
		}

		List<MapNode> endNodes = new List<MapNode>();
		for (int i = 0; i < endPoints.Length; i++)
		{
			endNodes.Add(personalMap.NodeFromWorldPosition(endPoints[i].toV3(), Vector3.zero));
		}

		flowNodes = new List<MapNode>();
		List<MapNode> nodesToEval = new List<MapNode>(); // nodes to evaluate
		List<MapNode> neighbors = new List<MapNode>(); // neighbors of evaluated nodes

		nodesToEval.AddRange(startNodes); // add start node to navigable nodes

		for (int cost = 1; endNodes.Count > 0; cost++)
		{
			// get neighbors of eval nodes
			for (int i = 0; i < nodesToEval.Count; i++)
			{
				// check 4 neighbors of current nodetoeval, add to navigable nodes if they pass all tests
				CheckNeighbor(nodesToEval[i], 1, 0, cost);
				CheckNeighbor(nodesToEval[i], -1, 0, cost);
				CheckNeighbor(nodesToEval[i], 0, 1, cost);
				CheckNeighbor(nodesToEval[i], 0, -1, cost);
				// now we have a list of neighbors that pass, and are ready for next loop
			}

			nodesToEval.Clear();
			nodesToEval.AddRange(neighbors); // evaluate our neighbors next loop
			neighbors.Clear();
		}

		void CheckNeighbor(MapNode node, int offX, int offY, int cost)
		{
			// get personal grid coordiantes of neighbor
			int x = node.gridX + offX;
			int y = node.gridY + offY;

			print(x.ToString() + ' ' + y.ToString());
			MapNode neighbor = personalMap.nodes[Mathf.Clamp(x, 0, personalMap.sizeX - 1), Mathf.Clamp(y, 0, personalMap.sizeY - 1)];

			if (x >= 0 && x < personalMap.sizeX && y >= 0 && y < personalMap.sizeY) // is it actually in the grid
			{
				if (neighbor.walkable) // is it walkable
				{
					if (neighbor.cost > cost) // is it the shortest path to this node
					{
						// we good fam
						neighbor.parent = node;
						neighbor.cost = cost;

						neighbors.Add(neighbor);
						flowNodes.Add(neighbor);
						endNodes.Remove(neighbor); // plz no break
					}
				}
				//}
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (flowNodes != null)
		{
			foreach (MapNode n in flowNodes)
			{
				Gizmos.color = new Color(n.cost % 10 / 10f, 0, 0);
				Gizmos.DrawCube(n.worldPosition, Vector3.one * 0.8f);
			}
		}
	}
}