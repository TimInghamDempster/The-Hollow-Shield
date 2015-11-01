using UnityEngine;
using System.Collections;

public class KelToggleUI : MonoBehaviour
{
	public GameObject localUiToggle = null;
	public GameObject infoUiToggle = null;
	public int toggle = 1;

	private void OnMouseDown()
	{
		if (toggle != 0) 
		{
			localUiToggle.gameObject.SetActive (true);
			toggle --;
			//print ("if worked");
		}

		else
		{
			localUiToggle.gameObject.SetActive (false);
			toggle++;
			//print ("else worked");
		}

		infoUiToggle.gameObject.SetActive (true);
	}



	private void OnMouseUp()
	{	
		infoUiToggle.gameObject.SetActive (false) ;
		//print("mouseup worked");
	}
}