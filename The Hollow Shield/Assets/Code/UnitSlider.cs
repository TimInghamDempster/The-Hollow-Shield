using UnityEngine;
using System.Collections;

public class UnitSlider : MonoBehaviour {

	bool m_dragging;
	float m_baseY;

	// Use this for initialization
	void Start () {
		m_baseY = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
	
		if(m_dragging)
		{
			Vector3 pos = transform.position;
			Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);

			Vector3 mousePos = Input.mousePosition;

			Vector3 adjustedMousePos = mousePos;
			adjustedMousePos.z = screenPos.z;

			Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(adjustedMousePos);

			pos.y = Mathf.Max(mouseWorldPos.y, m_baseY);
			
			transform.position = pos;
		}
	}
	
	void OnMouseUp()
	{
		m_dragging = false;
	}

	void OnMouseDown()
	{
		m_dragging = true;
	}
}
