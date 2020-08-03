using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerCursor myCursor;
    [SerializeField] List<PlayerMech> myMechs;
    public PlayerMech activeMech;

    [SerializeField] AnimationCurve indicatorwave1;
    [SerializeField] AnimationCurve indicatorwave2;

    bool phaseActive;

    // Start is called before the first frame update
    void Start()
    {
        foreach (PlayerMech mech in myMechs)
		{
            mech.player = this;
		}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPhaseActive(bool isActive)
	{
        phaseActive = isActive;
        if (isActive)
		{
            myCursor.ShowCursor(true);

			foreach (PlayerMech mech in myMechs)
			{
                mech.PhaseInit(0, 0);
			}

		} else
		{
            myCursor.ShowCursor(false);
        }
    }

    public void SetActiveMech(PlayerMech newMech)
	{
        if (activeMech != null)
		{
            activeMech.DeselectMech();
		}
        activeMech = newMech;
	}
}
