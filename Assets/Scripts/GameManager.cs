using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager self;
    public GUIManager guiManager;

    public enum turnPhase { environment, enemy, player, none }
	public turnPhase currentTurnPhase;
	public int currentTurn;
	public int turnActions;
	[HideInInspector] public MainMap mainMap;
	PlayerController player;
	EnemyManager enemyManager;
	WeatherController weather;

	private void Awake()
	{
		self = this;
		mainMap = GetComponentInChildren<MainMap>();
		player = GetComponent<PlayerController>();
		enemyManager = GetComponent<EnemyManager>();
		weather = GetComponent<WeatherController>();
	}

	private void Start()
	{
		BeginGame();
	}

	void BeginGame()
	{
		currentTurnPhase = turnPhase.environment;
		PhaseAction(currentTurnPhase);
	}

	public void NextPhase()
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
				currentTurnPhase = turnPhase.environment;
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
				// get new action count
				// update UI
				currentTurn++; // iterate

				weather.NextForecast();

				NextPhase();
				break;
			case turnPhase.enemy:
                guiManager.DisplayGameMessage("enemy phase");
                enemyManager.SetPhaseActive();
				break;
			case turnPhase.player:
				guiManager.DisplayGameMessage("player phase");
				player.SetPhaseActive(true);
				break;
			case turnPhase.none:
				break;
			default:
				break;
		}
	}

	public void PawnActionFinished()
	{
		switch (currentTurnPhase)
		{
			case turnPhase.environment:
				currentTurn++; // iterate
				break;
			case turnPhase.enemy:
				enemyManager.EnemyPhaseSequence();
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

	public void UseTurnAction() {
        turnActions--;
        guiManager.FillAction(turnActions);

		if (turnActions <= 0) {
            EndPlayerPhase();
        }
    }
}