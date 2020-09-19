using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemyManager : MonoBehaviour
{
	FlowFieldGenerator fieldGenerator;
	PlayerController playerController;

	[SerializeField] List<Pawn> enemyPawns;
	[SerializeField] List<Pawn> enemiesToMove;

	private void Awake()
	{
		fieldGenerator = GetComponent<FlowFieldGenerator>();
		Assert.IsNotNull(fieldGenerator, "no flow field generator on game manager");
		playerController = GetComponent<PlayerController>();
		Assert.IsNotNull(playerController, "no player controller on game manager");
	}

	public void SetPhaseActive()
	{
		// aw shizzle here we go
		// basically one long sequence
		// generate a flowfield (pathfinding)
		Vector2Int[] startPoints = new Vector2Int[playerController.myMechs.Count];
		for (int i = 0; i < startPoints.Length; i++)
		{
			startPoints[i] = playerController.myMechs[i].transform.position.toV2i();
		}

		Vector2Int[] endPoints = new Vector2Int[enemyPawns.Count];
		for (int i = 0; i < endPoints.Length; i++)
		{
			endPoints[i] = enemyPawns[i].transform.position.toV2i();
		}

		fieldGenerator.GenerateField(startPoints, endPoints);

		//StartCoroutine(EnemyPhaseSequence());
		EnemyPhaseSequence();
	}

	void EnemyPhaseSequence()
	{
		// choose a random enemy to move
		// look at flowfield, choose a place to move
		// if close enough to playerpawn, attack
		// choose next random enemy
		// idk
		Pawn activePawn;
		enemiesToMove.AddRange(enemyPawns);

		int r = Random.Range(0, enemiesToMove.Count);
		activePawn = enemiesToMove[r];
		enemiesToMove.RemoveAt(r);

		MapNode enemyNode = fieldGenerator.PersonalMap.NodeFromWorldPosition(activePawn.transform.position, Vector3.zero);
		int costGoal = enemyNode.cost - activePawn.MoveDist;
		// get the nodes close to me, within activepawnmovement
		// reuse target offset code from pawn.cs
		List<MapNode> nodesCloseToEnemy = new List<MapNode>();

		int x = -activePawn.MoveDist;
		int y = 0;

		// get all the points
		for (int i = 0; i <= activePawn.MoveDist; i++) // x
		{
			int numPoints = i * 2 + 1;
			for (int j = 0; j < numPoints; j++) // y
			{
				//possibleTargetsOffsets[index] = new Vector2Int(x, y + j);
				nodesCloseToEnemy.Add(fieldGenerator.PersonalMap.NodeFromWorldPosition(new Vector3(x + enemyNode.worldPosition.x, 0, y + j + enemyNode.worldPosition.z), Vector3.zero));
				if (i != activePawn.MoveDist) // if we are not at center
				{
					//possibleTargetsOffsets[index] = new Vector2Int(x + ((activePawn.AttackDist - i) * 2), y + j);
					nodesCloseToEnemy.Add(fieldGenerator.PersonalMap.NodeFromWorldPosition(new Vector3(x + ((activePawn.MoveDist - i) * 2) + enemyNode.worldPosition.x, 0, y + j + enemyNode.worldPosition.z), Vector3.zero));
				}
			}
			y--; // start one tile lower every tile
			x++; // start one tile to the right
		}

		// find nodes in nodes close to me that match cost goal, or are closest to cost goal
		List<MapNode> goalNodes = new List<MapNode>();
		int nearestCost = 999;
		foreach (MapNode n in nodesCloseToEnemy)
		{
			//Debug.DrawLine(n.worldPosition, n.worldPosition + Vector3.up, Color.magenta, 10f);

			if (n.cost < nearestCost)
			{
				goalNodes.Clear();
				goalNodes.Add(n);
				nearestCost = n.cost;
			}
			else if (n.cost == costGoal)
			{
				// we good
				goalNodes.Add(n);
				nearestCost = n.cost;
			}
		}
		// randomly choose node
		r = Random.Range(0, goalNodes.Count);

		// move there
		activePawn.PhaseInit(0, 0);
		print(enemyNode.cost.ToString() + " " + nearestCost.ToString());
		activePawn.Move(goalNodes[r].worldPosition);
		//Debug.DrawLine(goalNodes[r].worldPosition, goalNodes[r].worldPosition + Vector3.up, Color.green, 10f);
	}
}