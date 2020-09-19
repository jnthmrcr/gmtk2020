using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager self;

	public enum turnPhase { environment, enemy, player, none }
	public turnPhase currentTurnPhase;
	public int currentTurn;
	[HideInInspector] public MainMap mainMap;
	PlayerController player;
	EnemyManager enemyManager;

	private void Awake()
	{
		self = this;
		mainMap = GetComponentInChildren<MainMap>();
		player = GetComponent<PlayerController>();
		enemyManager = GetComponent<EnemyManager>();
	}

	private void Start()
	{
		BeginGame();
	}

	void BeginGame()
	{
		currentTurnPhase = turnPhase.environment;
		NextPhase();
	}

	void NextPhase()
	{
		switch (currentTurnPhase)
		{
			case turnPhase.environment:
				currentTurnPhase = turnPhase.player;
				PhaseAction(currentTurnPhase);
				break;
			case turnPhase.player:
				currentTurnPhase = turnPhase.enemy;
				PhaseAction(currentTurnPhase);
				break;
			case turnPhase.enemy:
				currentTurnPhase = turnPhase.player;
				PhaseAction(currentTurnPhase);
				break;
			default:
				break;
		}
	}

	public void EndPlayerPhase()
	{
		if (currentTurnPhase == turnPhase.player)
		{
			currentTurnPhase = turnPhase.enemy;
			PhaseAction(currentTurnPhase);
		}
	}

	void PhaseAction(turnPhase phase)
	{
		switch (phase)
		{
			case turnPhase.environment:
				currentTurn++; // iterate
				break;
			case turnPhase.enemy:
				enemyManager.SetPhaseActive();
				break;
			case turnPhase.player:
				player.SetPhaseActive(true);
				break;
			case turnPhase.none:
				break;
			default:
				break;
		}
	}
}