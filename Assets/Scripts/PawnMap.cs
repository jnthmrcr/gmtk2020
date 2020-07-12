using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnMap : MonoBehaviour
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

	public GridNode NodeFromWorldPosition(Vector3 worldPosition)
	{
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

		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				if (x == 0 && y == 0) continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < mapSizeX && checkY >= 0 && checkY < mapSizeY && (Mathf.Abs(x + y) == 1))
				{
					neighbors.Add(map[checkX, checkY]);
				}
			}
		}

		return neighbors;
	}
}
