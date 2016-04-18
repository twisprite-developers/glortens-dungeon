using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour {

	public Text healthText;
	public Image healthBar;

	private int health = 100;
	private int maxHealth = 100;
	

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetMaxHealth(int maxhealth)
	{
		this.maxHealth = maxhealth;
		this.health = maxhealth;
		SetHealth(this.health);
	}

	public void SetHealth(int newHealth)
	{
		if (newHealth > this.maxHealth)
		{
			newHealth = this.maxHealth;
		}

		//animate health bar to value
		Go.to( healthBar, 1.0f, new GoTweenConfig()
		      .floatProp("fillAmount", ((float)newHealth/(float)maxHealth))
		      .setEaseType(GoEaseType.SineOut));

		//set health text
		healthText.text = "" + newHealth;  

		//save health value
		health = newHealth;
	}
}
