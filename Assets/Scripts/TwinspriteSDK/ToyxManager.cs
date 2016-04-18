using UnityEngine;
using System.Collections;
using TwinSpriteSDK;

public class ToyxManager: MonoBehaviour  {

	public string API_KEY = "";
	public string SECRET_KEY = "";
	
	Toyx toyx;
	public string toyxId = "";	
	
	/*** Properties ***/
	public int level = 0;
	public int experience = 0;
	public int gold = 0;
	public WeaponType weapon;
	
	public enum WeaponType {Nothing,Axe};
	
	/*** Properties names ***/
	private const string LEVEL = "level";
	private const string EXPERIENCE = "experience";
	private const string GOLD = "gold";
	private const string WEAPON = "weapon";
	
	[HideInInspector]
	public string infoMessage = "";

	void Awake() {
		DontDestroyOnLoad(this.gameObject);
	}

	// Use this for initialization
	void Start () {

		// Clear message
		infoMessage = "";

		// Init Twinsprite SDK
		TwinSprite.initialize(API_KEY, SECRET_KEY);
	}


	/*** Toyx Functions ***/
	public void CreateSession(TwinSpriteDelegate createSessionDelegate) {
		CreateSession(toyxId, createSessionDelegate);
	}


	public void CreateSession(string id, TwinSpriteDelegate createSessionDelegate) {

		// If no id, do nothing
		if (id == null || id.Length == 0) {
			Debug.Log("No toyxID, playing without toyx.");

			if (createSessionDelegate != null) {
				createSessionDelegate.Invoke(null);
			}
			return;
		}

		// Make the toyx
		toyxId = id;
		toyx = new Toyx(id);

		toyx.CreateSessionInBackground(delegate(TwinSpriteError error) {
			if (error != null) {
				infoMessage += "Error creating session: "+error.message+"\nError code: " +error.errorCode+"\n";
			} else {
				infoMessage += "Created session!!!\n";
			}

			if (createSessionDelegate != null) {
				createSessionDelegate.Invoke(error);
			}
			
		});

	}
		

	public void FetchIfNeeded(TwinSpriteDelegate fetchDelegate) {

		// If no id, do nothing
		if (toyxId == null || toyxId.Length == 0) {
			Debug.Log("No toyxID, playing without toyx.");

			if (fetchDelegate != null) {
				fetchDelegate.Invoke(null);
			}
			return;
		}


		// No toyx, create session before
		if (toyx == null) {
			infoMessage += "Can't fetching, create session before.\n";
			return;
		}
		
		
		toyx.FetchIfNeededInBackground(delegate(TwinSpriteError error) {
			
			if (error != null) {
				infoMessage += "Error fetching: "+error.message+"\nError code: " +error.errorCode+"\n";
			} else {
				infoMessage += "Feched: "+toyx+"\n";
				
				// Fill properties
				experience = toyx.GetInt(EXPERIENCE);
				gold = toyx.GetInt(GOLD);
				level = toyx.GetInt(LEVEL);
				weapon = (WeaponType) toyx.GetInt(WEAPON);
								
			}
			
			if (fetchDelegate != null) {
				fetchDelegate.Invoke(error);
			}
		});
		
	}

	public void Fetch(TwinSpriteDelegate fetchDelegate) {

		// If no id, do nothing
		if (toyxId == null || toyxId.Length == 0) {
			Debug.Log("No toyxID, playing without toyx.");

			if (fetchDelegate != null) {
				fetchDelegate.Invoke(null);
			}
			return;
		}


		// No toyx, create session before
		if (toyx == null) {
			infoMessage += "Can't fetching, create session before.\n";
			return;
		}
		
		
		toyx.FetchInBackground(delegate(TwinSpriteError error) {
			
			if (error != null) {
				infoMessage += "Error fetching: "+error.message+"\nError code: " +error.errorCode+"\n";
			} else {
				infoMessage += "Feched: "+toyx+"\n";
				
				// Fill properties
				experience = toyx.GetInt(EXPERIENCE);
				gold = toyx.GetInt(GOLD);
				level = toyx.GetInt(LEVEL);
				weapon = (WeaponType) toyx.GetInt(WEAPON);
				
			}
			
			if (fetchDelegate != null) {
				fetchDelegate.Invoke(error);
			}
		});
		
	}


	public void Save(TwinSpriteDelegate saveDelegate) {

		// If no id, do nothing
		if (toyxId == null || toyxId.Length == 0) {
			Debug.Log("No toyxID, playing without toyx.");

			if (saveDelegate != null) {
				saveDelegate.Invoke(null);	
			}
			return;
		}


		// No toyx, create session before
		if (toyx == null) {
			infoMessage += "Can't fetching, create session before.\n";
			return;
		}

		// Set properties
		toyx.PutInt(EXPERIENCE,experience);
		toyx.PutInt(GOLD,gold);
		toyx.PutInt(LEVEL,level);
		toyx.PutInt(WEAPON, (int)weapon);

		// Save
		toyx.SaveInBackground(delegate(TwinSpriteError error) {
			if (error != null) {

				if (error.errorCode == TwinSpriteError.TwinSpriteNoDataAvaiable) {
					infoMessage += "No data avaiable, fetch it before.\n";
				} else {
					infoMessage += "Error saving: "+error.message+"\nError code: " +error.errorCode+"\n";
				}
			} else {
				infoMessage += "Saved: "+toyx+"\n";
			}

			if (saveDelegate != null) {
				saveDelegate.Invoke(error);
			}
			
		});
	}


	public void SaveEventually() {

		// If no id, do nothing
		if (toyxId== null || toyxId.Length == 0) {
			Debug.Log("No toyxID, playing without toyx.");
			return;
		}


		// No toyx, create session before
		if (toyx == null) {
			infoMessage += "Can't fetching, create session before.\n";
			return;
		}
		
		// Set properties
		toyx.PutInt(EXPERIENCE,experience);
		toyx.PutInt(GOLD,gold);
		toyx.PutInt(LEVEL,level);
		toyx.PutInt(WEAPON, (int)weapon);
		
		// Save
		toyx.SaveEventually(delegate(TwinSpriteError error) {
			if (error != null) {
				
				if (error.errorCode == TwinSpriteError.TwinSpriteNoDataAvaiable) {
					infoMessage += "No data avaiable, fetch it before.\n";
				} else {
					infoMessage += "Error saving: "+error.message+"\nError code: " +error.errorCode+"\n";
				}
			} else {
				infoMessage += "Saved: "+toyx+"\n";
			}
			
		});
	}



}
