using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager self;

	public enum turnPhase { environment, enemy, player, none }
	public turnPhase currentTurnPhase;
	public int currentTurn;
	public MainMap mainMap;

	[SerializeField] PlayerController player;

	private void Awake()
	{
		self = this;
		mainMap = GetComponentInChildren<MainMap>();
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

	void PhaseAction(turnPhase phase)
	{
		switch (phase)
		{
			case turnPhase.environment:
				currentTurn++; // iterate
				break;
			case turnPhase.enemy:
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