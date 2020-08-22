using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode
{
	public bool walkable;
	public Vector3 worldPosition;
	/// <summary> stores position in the mapgrid <summary>
	public int gridX, gridY;

	public int cost = 999;
	public MapNode parent;
	// ref to either the obstacle or pawn on this node
	public int costToAttack = 999;

	public MapNode(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
	{
		walkable = _walkable;
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
	}

	public MapNode(MapNode clone) // fuck you reference copy
	{
		walkable = clone.walkable;
		worldPosition = clone.worldPosition;
		gridX = clone.gridX;
		gridY = clone.gridY;
		cost = clone.cost;
		parent = clone.parent;
	}
}