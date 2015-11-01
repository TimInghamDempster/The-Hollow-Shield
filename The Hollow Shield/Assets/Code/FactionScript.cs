using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FactionScript : MonoBehaviour {

	public List<ArmyCounter> m_armies = new List<ArmyCounter>();
	public List<OrdersCourierScript> m_orders = new List<OrdersCourierScript>();

	public Color FactionColor;
	FactionAIScript m_controller;

	public Mesh FactionCastleMesh;
	public Material FactionCastleMaterial;
	public ArmyCounter HomeArmy;
	
	// Use this for initialization
	void Start () 
	{
		m_controller = GetComponent<FactionAIScript>();
	}

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
			foreach(OrdersCourierScript order in m_orders)
			{
				if(!order.TurnEnded)
				{
					return false;
				}
			}
			return true;
		}
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
		foreach(OrdersCourierScript order in m_orders)
		{
			order.BeginEndTurn();
		}
		if(m_controller)
		{
			m_controller.DoAI(this);
		}
	}
}
