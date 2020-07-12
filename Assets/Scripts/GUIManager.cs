using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    [SerializeField] GameObject menu;
    [SerializeField] GameObject howto;
    [SerializeField] GameObject menuBG;
    [SerializeField] Text startText;

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
		} else
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

    public void BtnAttack()
	{

	}

    public void BtnMove()
	{

	}

    public void BtnForcast()
	{
        inforcast = true;
        menuBG.SetActive(true);
	}

    public void exitForcast()
	{
        inforcast = false;
        menuBG.SetActive(false);
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
