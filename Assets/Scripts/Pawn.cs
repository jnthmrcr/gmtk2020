using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour
{
	// controlled either by player or cpu

	GridMap map;
    Vector2 position;

    public void GetMoveableArea()
	{

	}

    public void Move(Vector2 newPosition)
	{

	}

	public void FindNavigableNodes(Vector3 startPos, int distance, int windX = 0, int windY = 0)
	{
		List<GridNode> navigableNodes = new List<GridNode>();

		GridNode startingNode = map.NodeFromWorldPosition(startPos);


	}
}
