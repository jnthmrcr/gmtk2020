using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode
{
	public bool walkable;
	public enum pawnType { none, player, enemy }
	public pawnType pawnOnNode = pawnType.none;
	public Vector3 worldPosition;
	/// <summary> stores position in the mapgrid <summary>
	public int gridX, gridY;

	public int cost = 999;
	public MapNode parent;

	public MapNode(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, pawnType _pawnOnNode)
	{
		walkable = _walkable;
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
		pawnOnNode = _pawnOnNode;
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