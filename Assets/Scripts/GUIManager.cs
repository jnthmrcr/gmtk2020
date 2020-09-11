using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
	[SerializeField] GameManager gameManager;
	[SerializeField] GameObject menu;
	[SerializeField] GameObject howto;
	[SerializeField] GameObject menuBG;
	[SerializeField] Text startText;

	[SerializeField] GameObject forecastParent;
	[SerializeField] UI_ForecastItem[] forecastItems;

	[SerializeField] UI_WindDisplay[] forecastWindItems;
	[SerializeField] Text[] forecastMoveItems;

	[SerializeField] Text turnText;
	[SerializeField] Text actionText;
	[SerializeField] UI_WindDisplay windDisplay;

	bool gamestarted = false;
	bool inmenu = false;
	bool inhowto = false;
	bool inforcast = false;

	// Start is called before the first frame update
	void Start()
	{
		BtnMenu();
		BtnHowTo();
	}

	// Update is called once per frame
	void Update()
	{
		if (inmenu)
		{
			if (inhowto)
			{
				if (Input.anyKey)
				{
					exitHowTo();
				}
			}
		}
		else
		{
			if (inforcast)
			{
				if (Input.anyKey)
				{
					exitForcast();
				}
			}
		}
	}

	public void FillForecast(int[] move, Vector2Int[] wind)
	{
		// set current info
		turnText.text = gameManager.currentTurn.ToString();
		actionText.text = move[0].ToString();
		windDisplay.SetDisplay(wind[0]);

		// set forecast screen
		for (int i = 0; i < forecastItems.Length; i++)
		{
			forecastItems[i].SetDisplay(gameManager.currentTurn + i + 1, move[i + 1], wind[i + 1]);
			//forecastMoveItems[i].text = move[i].ToString();
		}

		for (int i = 0; i < forecastWindItems.Length; i++)
		{
			forecastWindItems[i].SetDisplay(wind[i]);
			forecastMoveItems[i].text = move[i].ToString();
		}
	}

	public void BtnForcast()
	{
		inforcast = true;
		menuBG.SetActive(true);
		forecastParent.SetActive(true);
	}

	public void exitForcast()
	{
		inforcast = false;
		menuBG.SetActive(false);
		forecastParent.SetActive(false);
	}

	public void BtnMenu()
	{
		inmenu = true;
		menuBG.SetActive(true);
		menu.SetActive(true);
	}

	public void BtnEndTurn()
	{

	}

	public void BtnHowTo()
	{
		inhowto = true;
		menuBG.SetActive(true);
		menu.SetActive(false);
		howto.SetActive(true);
	}

	void exitHowTo()
	{
		inhowto = false;
		howto.SetActive(false);
		menu.SetActive(true);
	}

	public void BtnStart()
	{
		inmenu = false;
		menuBG.SetActive(false);
		menu.SetActive(false);

		if (!gamestarted)
		{
			// start the game
			gamestarted = true;
			startText.text = "back";
		}
	}

	public void BtnExit()
	{
		Application.Quit();
	}
}