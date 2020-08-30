using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager self;

	public enum gamePhase { environmentTurn, enemyTurn, playerTurn, noTurn }
	public gamePhase currentTurnPhase;
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
		currentTurnPhase = gamePhase.environmentTurn;
		NextTurn();
	}

	void NextTurn()
	{
		switch (currentTurnPhase)
		{
			case gamePhase.environmentTurn:
				currentTurnPhase = gamePhase.playerTurn;
				TurnAction(currentTurnPhase);
				break;
			case gamePhase.playerTurn:
				currentTurnPhase = gamePhase.enemyTurn;
				TurnAction(currentTurnPhase);
				break;
			case gamePhase.enemyTurn:
				currentTurnPhase = gamePhase.playerTurn;
				TurnAction(currentTurnPhase);
				break;
			default:
				break;
		}
	}

	void TurnAction(gamePhase phase)
	{
		switch (phase)
		{
			case gamePhase.environmentTurn:
				break;
			case gamePhase.enemyTurn:
				break;
			case gamePhase.playerTurn:
				player.SetPhaseActive(true);
				break;
			case gamePhase.noTurn:
				break;
			default:
				break;
		}
	}
}