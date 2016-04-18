using UnityEngine;
using System.Collections;
using TwinSpriteSDK;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

	ToyxManager toyxManager;

	bool loadLevel = false;
	bool loading = false;

	// Buttons
	public Button buttonQr;
	public Button buttonNFC;
	public Button buttonPlay;


	public GameObject loadingBackground;
	public Image loadingSpinner;

	public GameObject popupBackground;
	public Text popupMessage;

	public AudioClip clickAudio;
	public AudioClip popupAudio;


	string popupStringMessage = "";
	bool showPopup = false;

	private AsyncOperation async;

	void Start() {

		//disable NFC background Scanning
		Reader.disableBackgroundScan();

		// Get toyx manager
		toyxManager = GameObject.Find("ToyxManager").GetComponent<ToyxManager>();

		// In iOS NFC is not allowed
		#if UNITY_IPHONE

		buttonNFC.gameObject.SetActive(false);
		
		// Move others
		float deltaQR = (buttonNFC.transform.position.y - buttonQr.transform.position.y) / 2;
		
		buttonQr.transform.position += new Vector3(0,deltaQR,0);
		buttonPlay.transform.position -= new Vector3(0,deltaQR,0);

		#endif

	}

	void Update() {

		if (showPopup) {
			showPopup = false;
			popupBackground.SetActive(true);
			popupMessage.text = popupStringMessage;
			AudioSource.PlayClipAtPoint(popupAudio, Camera.main.transform.position);
		}


		// Spinner
		loadingSpinner.rectTransform.eulerAngles = new Vector3(loadingSpinner.rectTransform.eulerAngles.x,loadingSpinner.rectTransform.eulerAngles.y, loadingSpinner.rectTransform.eulerAngles.z + 3);


		if (loadLevel && !loading) {
			loadLevel = false;
			loading = true;
			StartCoroutine(LoadGame());
		}

		#if UNITY_ANDROID
		if (Input.GetKeyDown(KeyCode.Escape)) 
		{
			Application.Quit();
        }
        #endif        

	}

	IEnumerator LoadGame() 
	{
		async = Application.LoadLevelAsync("game");
		async.allowSceneActivation = false; //don't activate the level right away once it is done
		
		while (!async.isDone) 
		{
			//when the level is practically loaded charge it
			if (async.progress == 0.9f) 
			{
				async.allowSceneActivation = true;
			}
			yield return 0; 
		}       
	}
	

	public void OnClickPlay() {

		// Init toyx
		AudioSource.PlayClipAtPoint(clickAudio, Camera.main.transform.position);
		InitToyx(toyxManager.toyxId);		
	}

	public void OnClickNFC() {
		AudioSource.PlayClipAtPoint(clickAudio, Camera.main.transform.position);
		Reader.ScanNFC(gameObject.name, "OnFinishScan");
	}

	public void OnClickQR() {
		AudioSource.PlayClipAtPoint(clickAudio, Camera.main.transform.position);
        Reader.ScanQR(gameObject.name, "OnFinishScan");        
	}


	public void OnFinishScan(string result) {

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
			InitToyx(toyxId);

		}

	}

	private void InitToyx(string toyxId) {

	
		// Show spinner
		loadingBackground.SetActive(true);


		// Create sessión
		toyxManager.CreateSession(toyxId, delegate(TwinSpriteError error) {

			if (error != null) {
				
				ShowPopup("Error creating session: "+error.message+"\nError code: " +error.errorCode+"\n");
				
			// Fetch
			} else {

				toyxManager.Fetch(delegate(TwinSpriteError fetchError) {

					if (fetchError != null) {
						
						ShowPopup("Error fetching: "+error.message+"\nError code: " +error.errorCode+"\n");
						
					// No error, go to game
					} else {
						loadLevel = true;
					}
				});
				
			}				
		});		
	}

	private void ShowPopup(string message) {

		// Set message
		popupStringMessage = message;

		// Show panel
		showPopup = true;

	}

	public void OnClickClosePopup() {

		// hjide panel
		AudioSource.PlayClipAtPoint(clickAudio, Camera.main.transform.position);
		popupBackground.SetActive(false);

	}


	// Extract toyxId from url
	string getToyxFromUrl(string url) {		
		int index = url.LastIndexOf('/') + 1;
		
		if (url.Length > index) {
			return url.Substring(index);		
		} 
		
		return url;
	}
}
