using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode
{
	public bool walkable;
	public Vector3 worldPosition;
	public int gridX, pGridX;
	public int gridY, pGridY;

	public int cost = 0;
	public int windCostX = 0;
	public int windCostY = 0;
	public GridNode parent;

	public GridNode(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
	{
		walkable = _walkable;
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
	}
}
