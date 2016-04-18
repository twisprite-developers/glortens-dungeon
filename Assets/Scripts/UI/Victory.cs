﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Victory : MonoBehaviour {

	public GameObject contentPanel;
    public GameObject loadingBackground;
    public Image loadingSpinner;
	public AudioClip VictoryAudioClipIntro;
	public AudioClip VictoryAudioClipLoop;
	private AudioSource audioSource;

	void Start ()
	{
		audioSource = Camera.main.GetComponent<AudioSource>();
		audioSource.Stop();
		audioSource.loop = false;		
		audioSource.PlayOneShot(VictoryAudioClipIntro);

		Invoke ("PlayLoop", VictoryAudioClipIntro.length);
	}

	public void PlayLoop()
	{
		audioSource.loop = true;
		audioSource.clip = VictoryAudioClipLoop;
		audioSource.Play();
    }

	public void OnClickRetry() {

		// Hide content pane
		contentPanel.SetActive(false);

        // Show spinner
        loadingBackground.SetActive(true);

        // Load level
        StartCoroutine(LoadGame(Application.loadedLevelName));


	}

	public void OnClickHome() 
	{

		// Save toyx if toyxmanager exists
		GameObject toyxManagerGameObject = GameObject.Find("ToyxManager");
		if (toyxManagerGameObject != null)
		{
			ToyxManager toyxManager = toyxManagerGameObject.GetComponent<ToyxManager>();
			toyxManager.Save(null);
		}

		// Hide content pane
		contentPanel.SetActive(false);

        // Show spinner
        loadingBackground.SetActive(true);

        // Load level
        StartCoroutine(LoadGame("menu"));

	}

	IEnumerator LoadGame(string scene) {
		AsyncOperation async = Application.LoadLevelAsync(scene);		
		while (!async.isDone) {
			
			// Spinner
			loadingSpinner.rectTransform.eulerAngles = new Vector3(loadingSpinner.rectTransform.eulerAngles.x,loadingSpinner.rectTransform.eulerAngles.y, loadingSpinner.rectTransform.eulerAngles.z - 3);
			
			yield return null;
		}
	}

}
