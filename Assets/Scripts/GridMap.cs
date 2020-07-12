using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap : MonoBehaviour
{
	public LayerMask unwalkableMask;
	public Vector2Int mapWorldSize;
	public static GridNode[,] map;

	int mapSizeX, mapSizeY;

	private void Start()
	{
		mapSizeX = Mathf.RoundToInt(mapWorldSize.x);
		mapSizeY = Mathf.RoundToInt(mapWorldSize.y);
		CreateMap();
	}

	void CreateMap()
	{
		map = new GridNode[mapSizeX, mapSizeY];
		Vector3 worldBottomLeft = transform.position - Vector3.right * mapWorldSize.x / 2 - Vector3.forward * mapWorldSize.y / 2;

		for (int x = 0; x < mapSizeX; x++)
		{
			for (int y = 0; y < mapSizeY; y++)
			{
				Vector3 worldpoint = worldBottomLeft + Vector3.right * (x + 0.5f) + Vector3.forward * (y + 0.5f);
				bool walkable = !(Physics.CheckSphere(worldpoint, 0.4f, unwalkableMask));
				map[x, y] = new GridNode(walkable, worldpoint, x, y);
			}
		}
	}

	public List<GridNode> GetNeighbors(GridNode node)
	{
		List<GridNode> neighbors = new List<GridNode>();

		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				if (x == 0 && y == 0) continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < mapSizeX && checkY >= 0 && checkY < mapSizeY)
				{
					neighbors.Add(map[checkX, checkY]);
				}
			}
		}

		return neighbors;
	}

	public GridNode NodeFromWorldPosition(Vector3 worldPosition)
	{
		float percentX = Mathf.Clamp01((worldPosition.x + mapSizeX / 2) / mapSizeX);
		float percentY = Mathf.Clamp01((worldPosition.z + mapSizeY / 2) / mapSizeY);

		int x = Mathf.RoundToInt((mapSizeX - 1) * percentX);
		int y = Mathf.RoundToInt((mapSizeY - 1) * percentY);
		return map[x, y];
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(transform.position, new Vector3(mapWorldSize.x, 1, mapWorldSize.y));

		if (map != null)
		{
			foreach(GridNode n in map)
			{
				Gizmos.color = (n.walkable) ? Color.white : Color.red;
				Gizmos.DrawCube(n.worldPosition, Vector3.one * 0.8f);
			}
		}
	}

	public PawnMap GrabPawnMap(int minX, int minY, int sizeX, int sizeY)
	{
		PawnMap pawnMap = new PawnMap(minX, minY, sizeX, sizeY);

		for (int x = 0; x < sizeX; x++)
		{
			for (int y = 0; y < sizeY; y++)
			{
				pawnMap.map[x, y] = map[minX + x, minY + y];
			}
		}
		return pawnMap;
	}
}
