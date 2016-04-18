using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUD : MonoBehaviour {

	public Image experienceBar;
	public Image lifeBar;


	public Text levelText;
	public Text goldText;

	float life = 1;
	int level = 0;
	float experience = 0;
	int gold = 0;


	void Awake() {
		UpdateHUD();
	}


	/****************
	 * Life
	 * **************/

	public void SetLife(float value) {

		// Save value
		life = Mathf.Min(1, Mathf.Max(0,value));

		// Update HUD
		UpdateHUD();

	}

	public void AddLife(float value) {
		
		// Update value
		life = Mathf.Min(1, life + value);
		
		// Update HUD
		UpdateHUD();
		
	}

	public void SubstractLife(float value) {

		// Update value
		life = Mathf.Max(0, life - value);
		
		// Update HUD
		UpdateHUD();
	}

	public float GetLife() {		
		return life;		
	}

	/****************
	* Level
	* **************/
	
	public void SetLevel(int value) {
		
		// Save value
		level = value;
		
		// Update HUD
		UpdateHUD();
		
	}
	
	public void AddLevel(int value) {
		
		// Update value
		level += value;
		
		// Update HUD
		UpdateHUD();
		
	}
	
	public int GetLevel() {		
		return level;		
	}

	/****************
	* Experience
	* **************/

	public void SetExperience(float value) {
		
		// Save value
		experience = value;
		
		// Update HUD
		UpdateHUD();
		
	}

	public void AddExperience(float value) {
		
		// Update value
		experience += value;
		
		// Update HUD
		UpdateHUD();
		
	}
	
	public float GetExperience() {		
		return experience;		
	}


	/****************
	* Gold
	* **************/

	public void SetGold(int value) {
		
		// Save value
		gold = value;
		
		// Update HUD
		UpdateHUD();
		
	}

	public void AddGold(int value) {
		
		// Update value
		gold += value;
		
		// Update HUD
		UpdateHUD();
		
	}

	public float GetGold() {		
		return gold;		
	}


	private void UpdateHUD() {

		// Life
		lifeBar.rectTransform.localScale = new Vector3(life,1,1);

		// Level
		levelText.text = "Level: "+level;

		// Experience
		experienceBar.rectTransform.localScale = new Vector3(Mathf.Max(0,experience),1,1);

		// Gold
		goldText.text = ""+gold;

	}

}
