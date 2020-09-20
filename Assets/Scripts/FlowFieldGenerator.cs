using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowFieldGenerator : MonoBehaviour
{
	[SerializeField] MainMap mainMap;
	MapGrid _personalMap;
	public MapGrid PersonalMap
	{
		get => _personalMap;
		private set => _personalMap = value;
	}

	public List<MapNode> flowNodes = new List<MapNode>();

	public void GenerateField(Vector2Int[] startPoints, Vector2Int[] endPoints)
	{
		PersonalMap = mainMap.grid.GetSubMap(mainMap.grid.sizeX, mainMap.grid.sizeY, mainMap.grid.minX, mainMap.grid.minY, mainMap.grid);

		MapNode[] startNodes = new MapNode[startPoints.Length];
		for (int i = 0; i < startNodes.Length; i++)
		{
			startNodes[i] = PersonalMap.NodeFromWorldPosition(startPoints[i].toV3(), Vector3.zero);
		}

		List<MapNode> endNodes = new List<MapNode>();
		for (int i = 0; i < endPoints.Length; i++)
		{
			endNodes.Add(PersonalMap.NodeFromWorldPosition(endPoints[i].toV3(), Vector3.zero));
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

			MapNode neighbor = PersonalMap.nodes[Mathf.Clamp(x, 0, PersonalMap.sizeX - 1), Mathf.Clamp(y, 0, PersonalMap.sizeY - 1)];

			if (x >= 0 && x < PersonalMap.sizeX && y >= 0 && y < PersonalMap.sizeY) // is it actually in the grid
			{
				if (neighbor.walkable) // is it walkable
				{
					if (neighbor.cost > cost) // is it the shortest path to this node
					{
						// we good fam
						neighbor.parent = node;
						neighbor.cost = cost;

						if (neighbor.pawnOnNode == MapNode.pawnType.none)
						{
							neighbors.Add(neighbor);
						}
						flowNodes.Add(neighbor);
						endNodes.Remove(neighbor); // plz no break
					}
				}
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