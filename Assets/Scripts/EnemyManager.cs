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

	int r;

	private void Awake()
	{
		fieldGenerator = GetComponent<FlowFieldGenerator>();
		Assert.IsNotNull(fieldGenerator, "no flow field generator on game manager");
		playerController = GetComponent<PlayerController>();
		Assert.IsNotNull(playerController, "no player controller on game manager");
	}

	public void SetPhaseActive()
	{
		// clean and init enemypawns list
		for (int i = enemyPawns.Count - 1; i >= 0; i--)
		{
			if (enemyPawns[i] == null)
				enemyPawns.RemoveAt(i);
			else
				enemyPawns[i].PhaseInit(0, 0);
		}

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
		enemiesToMove.Clear();
		enemiesToMove.AddRange(enemyPawns);

		r = Random.Range(0, enemiesToMove.Count);
		activePawn = enemiesToMove[r];
		enemiesToMove.RemoveAt(r);

		// check to see if we have any targetable pawns
		// if not, move closer
		if (activePawn.targetableDamageTakerPoints.Count > 0)
			EnemyPawnAttack(activePawn);
		else
			EnemyPawnMovement(activePawn);
	}

	void EnemyPawnAttack(Pawn pawn)
	{
		Collider[] colliderDamageTaker;
		Pawn damagablePawn;
		foreach (Vector3 p in pawn.targetableDamageTakerPoints)
		{
			colliderDamageTaker = Physics.OverlapSphere(p, 0.1f);
			damagablePawn = colliderDamageTaker[0].GetComponent<Pawn>();
			if (damagablePawn != null)
			{
				colliderDamageTaker[0].GetComponent<DamageTaker>().Damage();
				break;
			}
		}
	}

	void EnemyPawnMovement(Pawn pawn)
	{
		MapNode enemyNode = fieldGenerator.PersonalMap.NodeFromWorldPosition(pawn.transform.position, Vector3.zero);
		int costGoal = enemyNode.cost - pawn.MoveDist;
		// get the nodes close to me, within activepawnmovement
		// reuse target offset code from pawn.cs
		List<MapNode> nodesCloseToEnemy = new List<MapNode>();

		int x = -pawn.MoveDist;
		int y = 0;

		// get all the points
		for (int i = 0; i <= pawn.MoveDist; i++) // x
		{
			int numPoints = i * 2 + 1;
			for (int j = 0; j < numPoints; j++) // y
			{
				//possibleTargetsOffsets[index] = new Vector2Int(x, y + j);
				nodesCloseToEnemy.Add(fieldGenerator.PersonalMap.NodeFromWorldPosition(new Vector3(x + enemyNode.worldPosition.x, 0, y + j + enemyNode.worldPosition.z), Vector3.zero));
				if (i != pawn.MoveDist) // if we are not at center
				{
					//possibleTargetsOffsets[index] = new Vector2Int(x + ((activePawn.AttackDist - i) * 2), y + j);
					nodesCloseToEnemy.Add(fieldGenerator.PersonalMap.NodeFromWorldPosition(new Vector3(x + ((pawn.MoveDist - i) * 2) + enemyNode.worldPosition.x, 0, y + j + enemyNode.worldPosition.z), Vector3.zero));
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
		print(enemyNode.cost.ToString() + " " + nearestCost.ToString() + " " + (enemyNode.cost - nearestCost).ToString());
		pawn.Move(goalNodes[r].worldPosition);
		//Debug.DrawLine(goalNodes[r].worldPosition, goalNodes[r].worldPosition + Vector3.up, Color.green, 10f);
	}
}