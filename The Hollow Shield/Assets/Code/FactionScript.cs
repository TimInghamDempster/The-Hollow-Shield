using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FactionScript : MonoBehaviour {

	public List<ArmyCounter> m_armies = new List<ArmyCounter>();

	public bool TurnEnded
	{
		get
		{
			foreach(ArmyCounter army in m_armies)
			{
				if(!army.TurnEnded)
				{
					return false;
				}
			}
			return true;
		}
	}

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void EndTurn()
	{
		foreach(ArmyCounter army in m_armies)
		{
			army.BeginEndTurn();
		}
	}
}
