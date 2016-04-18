using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TwinSpriteSDK;

public class BossFight : MonoBehaviour 
{
	public BossFightPlayer glorten;
	public BossFightIA bossIA;
	public Button[] normalAttacksButtons;
	public Button[] specialAttacksButtons;

    public GameObject loadingBackground;
    public Image loadingSpinner;
    public GameObject popupBackground;
	public Text popupMessage;

	public GameObject gameOverScreen;
	public GameObject victoryScreen;

	public AudioClip BossRoar;
	public AudioClip BossIntro;
	public AudioClip BossLoop;

	public Image[] CinematicBars;
	public RectTransform buttonsPanel;
	public Transform HealthBarsPanel;

    private bool parsingToyx = false;
	
	public enum BossFightState : int
	{
		CINEMATIC = 0,
		PLAYING,
		END
	}
	private BossFightState currentState;
	private int[] specialAttackCharges;

	private BossFightCinematic bossFightCinematic;
	public int currentAttackDamage;
	private bool isPlayerTurn = true;
	private const long FIREWAVE_MODEL_ID = 122;
	private const long EARTHQUAKE_MODEL_ID = 123;
	private static string SPECIAL_ATTACK_USES_ATTRIB = "uses";
	private List<string> alreadyScannedToyxs = new List<string>();

	// Use this for initialization
	void Start () 
	{	
		//enable NFC background Scanning
		Reader.enableBackgroundScan();
		Reader.ScanNFC(gameObject.name, "OnFinishScan");

		//link player control with me
		glorten.SetBossFight(this);

		//link IA with me
		bossIA.SetBossFight(this);

		//disable special attacks on start
		specialAttackCharges = new int[specialAttacksButtons.Length];
		for (int i = 0; i < specialAttacksButtons.Length; i++)
		{
			UnlockSpecialButton(i, false);
		}

		SetState(BossFightState.CINEMATIC);

	}
	#region states
	private void SetState(BossFightState newState)
	{
		Debug.Log ("Entering state " + newState);
		switch (newState)
		{
			case BossFightState.CINEMATIC:
				//hide buttons				
				buttonsPanel.transform.localPosition = new Vector3(buttonsPanel.transform.localPosition.x, 
				                                                   buttonsPanel.transform.localPosition.y - 200, 
				                                                   buttonsPanel.transform.localPosition.z);

				//show cinematic bars			
				foreach(Image bar in CinematicBars)
				{
					Go.to( bar, 1.0f, new GoTweenConfig()
						.colorProp( "color", new Vector4( 1.0f, 1.0f, 1.0f, 1.0f) )
						.setEaseType(GoEaseType.SineOut)
					);
				}
				//hide health bars
				HealthBarsPanel.transform.localPosition = new Vector3(	HealthBarsPanel.transform.localPosition.x ,
			                                                      		HealthBarsPanel.transform.localPosition.y + 250,
			                                                      		HealthBarsPanel.transform.localPosition.x);

				//play ambient sound
				AudioSource audioSource = Camera.main.GetComponent<AudioSource>();
				audioSource.loop = false;
				audioSource.clip = BossIntro;
				audioSource.PlayDelayed(3.0f);
                    
				//start cinematic
				bossFightCinematic = new GameObject("bossfightCinematic").AddComponent<BossFightCinematic>();
				bossFightCinematic.BossRoar = BossRoar;
				bossFightCinematic.Begin(glorten.GetChar(), bossIA.GetChar());				
				break;
			case BossFightState.PLAYING:
				//hide cinematic bars			
				foreach(Image bar in CinematicBars)
				{
					Go.to( bar, 1.0f, new GoTweenConfig()
					      .colorProp( "color", new Vector4( 1.0f, 1.0f, 1.0f, 0.0f) )
					      .setEaseType(GoEaseType.SineOut));
				}
				
				//show buttons				
				Go.to( buttonsPanel.transform, 1.0f, new GoTweenConfig()
				      .localPosition(new Vector3(0.0f,200.0f,0.0f), true)
				      .setEaseType(GoEaseType.SineOut)
						.setDelay(0.5f));
				

				//show health bars				
				Go.to( HealthBarsPanel.transform, 1.0f, new GoTweenConfig()
				      .localPosition(new Vector3(0.0f,-250.0f,0.0f), true)
				      .setEaseType(GoEaseType.SineOut)
				      .setDelay(0.5f));

				DestroyImmediate(bossFightCinematic.gameObject);

				//play loop sound
				AudioSource audioSource2 = Camera.main.GetComponent<AudioSource>();
				audioSource2.loop = true;
				audioSource2.clip = BossLoop;
	            audioSource2.Play();
                
                //is player turn! yay!	
				glorten.YourTurn(true);
				isPlayerTurn = true;

				break;
			case BossFightState.END:
				//end playing
				if (glorten.GetChar().health <= 0)
				{
					Invoke("ShowGameOverScreen", 2.0f);
				}
			else if (bossIA.GetChar().health <= 0)
				{
					Invoke("ShowVictoryScreen", 7.0f);
				}
				break;
		}
		this.currentState = newState;
	}
	#endregion

	#region updates
	// Update is called once per frame
	void LateUpdate() 
	{
        // Spinner
        if (loadingBackground.activeSelf)
        {
            loadingSpinner.rectTransform.eulerAngles = new Vector3(loadingSpinner.rectTransform.eulerAngles.x, loadingSpinner.rectTransform.eulerAngles.y, loadingSpinner.rectTransform.eulerAngles.z + 3);
        }

        switch (currentState)
		{
			case BossFightState.CINEMATIC:
				UpdateCinematic();
				break;
			case BossFightState.PLAYING:
				UpdatePlaying();
				break;
			case BossFightState.END:
				UpdateEnd();
				break;
		}
	}

	void UpdateCinematic ()
	{
		if (bossFightCinematic.Finished)
		{
			SetState(BossFightState.PLAYING);
		}
	}	

	void UpdatePlaying ()
	{
	
	}

	void UpdateEnd ()
	{

	}

	#endregion

	
	public void OnFinishScan(string result) {

        if (!isPlayerTurn || loadingBackground.activeSelf)
        {
            //if is not player turn or we are still parsing another toyx, abort
			return;
        }

        // Show spinner
        loadingBackground.SetActive(true);

        // Error
        if (result == Reader.ERROR) {
			ShowPopup("Error reading.");
			// No hardware
		} else if (result == Reader.NO_HARDWARE) {
			ShowPopup("No hardware detected.");
			// No software
		} else if (result == Reader.NO_SOFTWARE) {
			ShowPopup("No software detected.");
			// Cancelled
		} else if (result == Reader.CANCELLED) {
			ShowPopup("User cancelled operation.");
			// No allowed os
		} else if (result == Reader.NO_ALLOWED_OS) {
			ShowPopup("No allowed OS to perform operation.");
			// No error
		} else {				
			string toyxId =  getToyxFromUrl(result);
			if (alreadyScannedToyxs.Contains(toyxId))
			{
				ShowPopup("Toyx already scanned this session");
			}
			else
			{
				alreadyScannedToyxs.Add(toyxId);
				ParseToyx(toyxId);			
			}
		}		
	}


	// Extract toyxId from url
	string getToyxFromUrl(string url) {		
		int index = url.LastIndexOf('/') + 1;
		
		if (url.Length > index) {
			return url.Substring(index);		
		} 
		
		return url;
	}

	private void ParseToyx(string toyxId) {

		// If no id, do nothing
		if (toyxId == null || toyxId.Length == 0) {
            // remove spinner
            loadingBackground.SetActive(false);
            return;
		}


		if (Utils.HasInternetConnection())
		{
			//Fetch from internet
			// Make the toyx
			Toyx toyx = new Toyx(toyxId);
			
			toyx.CreateSessionInBackground(delegate(TwinSpriteError error1) {
				if (error1 != null) {
					ShowPopup("Error creating session: "+error1.message+"\nError code: " +error1.errorCode);
				} else {
					Debug.Log("Created session!!! now fetch it");
					
					toyx.FetchInBackground(delegate(TwinSpriteError error2) {
						
						if (error2 != null) {
							ShowPopup("Error fetching: "+error2.message+"\nError code: " +error2.errorCode);
						} else {
							Debug.Log("Fetched: "+toyx);
							
							long modelID = toyx.GetToyxSnapshot().GetToyxModel().GetId();
							int charges = 0;
							switch (modelID)
							{
							case FIREWAVE_MODEL_ID:
                                loadingBackground.SetActive(false); // remove spinner
								charges = toyx.GetInt(SPECIAL_ATTACK_USES_ATTRIB);
								UnlockSpecialButton(0, true, charges);
                            break;
							case EARTHQUAKE_MODEL_ID:
                                loadingBackground.SetActive(false); // remove spinner
								charges = toyx.GetInt(SPECIAL_ATTACK_USES_ATTRIB);
								UnlockSpecialButton(1, true, charges);
                            break;
							default:
								ShowPopup("Toyx Model not compatible with bossfight powers");
								break;
							}
						}
					});
				}			
			});
		}
		else
		{
			ShowPopup("Toyx Model not compatible with bossfight powers");
		}
	}

	private void ShowPopup(string message) 
	{
        // remove spinner
        loadingBackground.SetActive(false);

        popupBackground.SetActive(true);
		popupMessage.text = message;
    }

	public void OnClickClosePopup() {
		
		// hide panel
		popupBackground.SetActive(false);
		
	}
	
	public void PerformAttack(int attackIndex)
	{
		if (isPlayerTurn && glorten.GetChar().ReadyToAttack)
		{
			//special attack charges start at index 2
			int indexShift = 2;
			if (attackIndex >= indexShift)
			{
				attackIndex -= indexShift; //shift the index
				if (specialAttackCharges[attackIndex] > 0)
				{
					specialAttackCharges[attackIndex]--;
					if (specialAttackCharges[attackIndex] > 0)
					{
						specialAttacksButtons[attackIndex].transform.Find("chargeText").GetComponent<Text>().text = ""+specialAttackCharges[attackIndex];
					}
					else
					{
						UnlockSpecialButton(attackIndex, false);
					}
				}
				else
				{
					//specialAttackCharges are zero? this means it is locked, show the unlock screen
					ShowUnlockScreen();
					return;
				}
				attackIndex += indexShift; //unshift the index
			}
			glorten.PerformAttack(attackIndex);
		}
	}

	public void DamageBoss(AbstractGoTween tw)
	{
		bossIA.GetChar().Hit(currentAttackDamage, "hit");
	}

	public void DamageBossBig(AbstractGoTween tw)
	{
		bossIA.GetChar().Hit(currentAttackDamage, "hit2");
	}

	public void DamagePlayer(AbstractGoTween tw)
	{
		glorten.GetChar().Hit(currentAttackDamage);
	}

	public void HealPlayer(AbstractGoTween tw)
	{
		glorten.GetChar().Heal(currentAttackDamage);
	}


	public void ChangeTurn(AbstractGoTween tween)
	{
		isPlayerTurn = !isPlayerTurn;

		if (!isPlayerTurn)
		{
			glorten.YourTurn(false);
			bossIA.YourTurn(true);
		}
		else
		{		
			glorten.YourTurn(true);
			bossIA.YourTurn(false);
		}

		if (glorten.GetChar().health <= 0 || bossIA.GetChar().health <=0)
		{		
			SetState(BossFightState.END);
		}
	}

	void UnlockSpecialButton(int index, bool unlock, int unlockCharges = 3)
	{
		if (!unlock)
		{
			specialAttackCharges[index] = 0; 
			specialAttacksButtons[index].transform.Find("chargeText").GetComponent<Text>().text = "";
			specialAttacksButtons[index].transform.Find("disabled").gameObject.SetActive(true);
			ColorBlock cb = specialAttacksButtons[index].colors;
			cb.normalColor = new Color(1.0f,1.0f,1.0f,0.5f);
			cb.highlightedColor = new Color(1.0f,1.0f,1.0f,0.5f);
			cb.pressedColor = new Color(1.0f,1.0f,1.0f,0.5f);
			specialAttacksButtons[index].colors = cb;
		}
		else
		{
			specialAttackCharges[index] = unlockCharges; 
			specialAttacksButtons[index].transform.Find("chargeText").GetComponent<Text>().text = ""+specialAttackCharges[index];
			specialAttacksButtons[index].transform.Find("disabled").gameObject.SetActive(false);
			ColorBlock cb = specialAttacksButtons[index].colors;
			cb.normalColor = Color.white;
			cb.highlightedColor = Color.white;
			cb.pressedColor = Color.white;
			specialAttacksButtons[index].colors = cb;


			specialAttacksButtons[index].transform.localScale = new Vector3(1.3f,1.3f,1.3f);
			//animate the button
			Go.to( specialAttacksButtons[index].transform, 0.5f, new GoTweenConfig()
			      .setUpdateType(GoUpdateType.LateUpdate)
			      .scale(new Vector3(1.0f, 1.0f, 1.0f))
			      .setEaseType(GoEaseType.BounceOut)			
			      );
        }
	}

	void ShowUnlockScreen()
	{
		if (isPlayerTurn) //which should...
		{
			Reader.ScanQR(gameObject.name, "OnFinishScan");
		}
	}

	private void ShowGameOverScreen()
	{
		gameOverScreen.SetActive(true);
	}

	private void ShowVictoryScreen()
	{
		glorten.GetChar().PlayOnce(5);
		victoryScreen.SetActive(true);
    }
}
