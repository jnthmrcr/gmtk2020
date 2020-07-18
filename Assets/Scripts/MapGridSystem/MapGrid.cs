using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGrid
{
	public MapGrid parentMap;
    public MapNode[,] nodes;
    public int sizeX, sizeY;
	public int minX, minY;
	bool filled = false;

	public MapGrid(int sizeX, int sizeY, int minX = 0, int minY = 0, MapGrid parentMap = null)
	{
		this.parentMap = parentMap;
		nodes = new MapNode[sizeX, sizeY];
		this.sizeX = sizeX;
		this.sizeY = sizeY;

		this.minX = minX;
		this.minY = minY;

		filled = false;
	}

	/// <summary> get subsection of the map for pawn use </summary>
	public MapGrid GetSubMap(int sizeX, int sizeY, int minX, int minY)
	{
		MapGrid submap = new MapGrid(sizeX, sizeY, minX, minY, this);

		// populate submap
		for (int x = 0; x < sizeX; x++)
		{
			for (int y = 0; y < sizeY; y++)
			{
				submap.nodes[x, y] = nodes[minX + x, minY + y];
				submap.nodes[x, y].gridX = x;
				submap.nodes[x, y].gridX = y;
			}
		}

		return submap;
	}

	public MapNode NodeFromWorldPosition(Vector3 worldPosition)
	{
		float percentX = Mathf.Clamp01((worldPosition.x + sizeX / 2f) / sizeX);
		float percentY = Mathf.Clamp01((worldPosition.z + sizeY / 2f) / sizeY);

		int x = Mathf.RoundToInt((sizeX - 1f) * percentX);
		int y = Mathf.RoundToInt((sizeY - 1f) * percentY);
		return nodes[x, y];
	}

	public MapNode GetNodeInParentMap(int posX, int posY)
	{
		return parentMap.nodes[posX + minX, posY + minY];
	}
}
