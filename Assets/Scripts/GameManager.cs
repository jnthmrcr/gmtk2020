using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public enum gamePhase { environmentTurn, enemyTurn, playerTurn, noTurn }
	public gamePhase currentTurnPhase;
	public enum actionMode { move, attack }
	public actionMode currentActionMode;

	[SerializeField] PlayerController player;

	private void Start()
	{
		BeginGame();
	}

	void BeginGame()
	{
		currentTurnPhase = gamePhase.environmentTurn;
		currentActionMode = actionMode.move;
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

	public void SetUserMode(actionMode newMode)
	{
		currentActionMode = newMode;

		switch (currentActionMode)
		{
			case actionMode.attack:
				//player.activeMech.set
				player.SetAttackIndicators();
				break;
			case actionMode.move:
				player.SetMoveIndicators();
				break;
			default:
				break;
		}
	}
}