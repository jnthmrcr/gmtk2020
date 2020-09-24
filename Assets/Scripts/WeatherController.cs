using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherController : MonoBehaviour
{
	public GUIManager guiManager;

	public int forecastLength = 6;

	public int movePeriod = 6; // the sum of the actions in this length of forecast will give you the amplitude avg
	public int moveAmplitudeAvg = 5; // the average "default" actions per period
	public int moveAmplitudeMin = 2; // the fewest actions in a period
	public int moveAmplitudeAdditivePeak = 1; // creates a peak in the period that takes this many points from the rest of the period
	int _moveAmplitudeRange;
	public int windMagnitudeMax = 4;
	public List<int> moveForecast;
	public List<Vector2Int> windForecast;

	void CreateMoveForecast()
	{
		int startingIndex = moveForecast.Count;

		_moveAmplitudeRange = (moveAmplitudeAvg - moveAmplitudeMin) * 2 + 1;
		int budget = movePeriod * moveAmplitudeAvg;
		int balance = 0;
		int forecast = 0;
		for (int i = 0; i < movePeriod; i++)
		{
			forecast = Random.Range(0, _moveAmplitudeRange) + moveAmplitudeMin;
			balance += forecast;
			moveForecast.Add(forecast);
		}

		// check balance against budget
		int index;
		while (balance > budget) // subtract from random forecast
		{
			index = Random.Range(startingIndex, moveForecast.Count);
			if (moveForecast[index] > moveAmplitudeMin)
			{
				moveForecast[index]--;
				balance--;
			}
		}

		while (budget > balance) // add to random forecast
		{
			index = Random.Range(startingIndex, moveForecast.Count);
			moveForecast[index]++;
			balance++;
		}

		// int targetIndex = Random.Range(startingIndex, moveForecast.Count);
		// for (int i = 0; i < moveAmplitudeAdditivePeak; i++) // choose a node and transfer balance to it
		// {
		// 	index = Random.Range(startingIndex, moveForecast.Count);

		// }

	}

	void CreateWindForcast()
	{
		// r top limit would be wind magnitude max or something
		int r = Mathf.RoundToInt(Mathf.PerlinNoise(0f, 0.210451f * (float) windForecast.Count) * 6) - 1;
		int x = Random.Range(-r, r + 1);
		int y = Random.Range((r - Mathf.Abs(x)), (r - Mathf.Abs(x)));
		windForecast.Add(new Vector2Int(x, y));

		//Random.insideUnitCircle();
	}

	void FillForecasts()
	{
		while (moveForecast.Count <= forecastLength)
		{
			CreateMoveForecast();
		}

		while (windForecast.Count <= forecastLength)
		{
			CreateWindForcast();
		}
	}

	public void NextForecast()
	{
		if (moveForecast.Count > 0)
			moveForecast.RemoveAt(0);

		if (windForecast.Count > 0)
			windForecast.RemoveAt(0);

		FillForecasts();
		guiManager.FillForecast(moveForecast.ToArray(), windForecast.ToArray());
	}
}