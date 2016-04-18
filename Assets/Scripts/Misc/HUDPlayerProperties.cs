using UnityEngine;
using System.Collections;

public class HUDPlayerProperties : MonoBehaviour {

	private  float updateInterval = 0.5F;	
	private float timeleft; 				// Left time for current interval
	public Player player;

	void Start()
	{
		if( !GetComponent<GUIText>() )
		{
			Debug.Log("HUDPlayerProperties needs a GUIText component!");
			enabled = false;
			return;
		}
	}
	
	void Update()
	{
		timeleft -= Time.deltaTime;

		// Interval ended - update GUI text and start new interval
		if( timeleft <= 0.0 )
		{
			string format = System.String.Format("HEALTH {0:F2} ", player.health);
			GetComponent<GUIText>().text = format;
			
			if(player.health < 30)
				GetComponent<GUIText>().material.color = Color.yellow;
			else 
				if(player.health < 10)
					GetComponent<GUIText>().material.color = Color.red;
			else
				GetComponent<GUIText>().material.color = Color.green;
			timeleft = updateInterval;

		}
	}
}
