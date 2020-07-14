using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PawnMap
{
	public GridNode[,] map;
	int mapMinX, mapMinY;
	int mapSizeX, mapSizeY;

	public PawnMap(int minX, int minY, int sizeX, int sizeY)
	{
		mapMinX = minX;
		mapMinY = minY;
		mapSizeX = sizeX;
		mapSizeY = sizeY;
		map = new GridNode[mapSizeX, mapSizeY];
	}

	public GridNode NodeFromWorldPosition(Vector3 worldPosition, Vector3 transPosition)
	{
		worldPosition -= transPosition;
		float percentX = Mathf.Clamp01((worldPosition.x + mapSizeX / 2) / mapSizeX);
		float percentY = Mathf.Clamp01((worldPosition.z + mapSizeY / 2) / mapSizeY);

		int x = Mathf.RoundToInt((mapSizeX - 1) * percentX);
		int y = Mathf.RoundToInt((mapSizeY - 1) * percentY);
		return map[x, y];
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

	public List<GridNode> GetImmediateNeighbors(GridNode node)
	{
		List<GridNode> neighbors = new List<GridNode>();

		int checkX = node.gridX + 1;
		int checkY = node.gridY + 0;
		NeighborCheck(checkX, checkY, ref neighbors);
		checkX = node.gridX - 1;
		checkY = node.gridY + 0;
		NeighborCheck(checkX, checkY, ref neighbors);
		checkX = node.gridX + 0;
		checkY = node.gridY + 1;
		NeighborCheck(checkX, checkY, ref neighbors);
		checkX = node.gridX + 0;
		checkY = node.gridY - 1;
		NeighborCheck(checkX, checkY, ref neighbors);

		return neighbors;
	}

	void NeighborCheck(int x, int y, ref List<GridNode> neib)
	{
		if (x >= 0 && x < mapSizeX && y >= 0 && y < mapSizeY)
		{
			// valid
			if (map[x, y].walkable)
			{
				neib.Add(map[x, y]);
			}
		}
	}
}
