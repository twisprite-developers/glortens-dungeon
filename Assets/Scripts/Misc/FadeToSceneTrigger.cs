using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeToSceneTrigger : MonoBehaviour {

	public GameObject target;
	public Vector3 restrictToAxis = Vector3.one;
	public ScreenFader screenFader;
	public string sceneName;

	private BoxCollider trigger;
	private Vector3 enterPosition;
	private Vector3 endPosition;
	private float distanceToEnd = 0f;
	private float currentDistance= 0f;
	private AsyncOperation async;

	void Awake()
	{
		trigger = this.GetComponent<BoxCollider>();
		if(trigger == null)
		{
			Debug.LogError("FadeToSceneTrigger needs a box collider to work!");
			DestroyImmediate(this.gameObject);
			return;
		}

		if (screenFader == null)
		{
			Debug.LogError("FadeToSceneTrigger needs a screenFader to work!");
			DestroyImmediate(this.gameObject);
			return;
		}

	}

	void OnTriggerEnter(Collider other) 
	{
		Debug.Log("OnTriggerEnter");
		if (other.gameObject == target)
		{
			enterPosition = Vector3.Scale(other.gameObject.transform.position,restrictToAxis);
			endPosition = enterPosition + Vector3.Scale(trigger.size,restrictToAxis);
			distanceToEnd = Vector3.Distance(enterPosition, endPosition);  
			currentDistance = 0f;
			screenFader.GetComponent<Image>().color = new Color(0.0f,0.0f,0.0f,0.0f);
			screenFader.fadeTime = 0.3f;
			screenFader.gameObject.SetActive(true);
		}	
	}

	void OnTriggerExit(Collider other) 
	{
		Debug.Log("OnTriggerExit");
		if (other.gameObject == target)
		{
			if (currentDistance < distanceToEnd)
			{
				//we are exiting ahead the finish
				screenFader.SetTargetAlpha(1.0f);
				StartCoroutine(LoadScene());
				return;
			}
			enterPosition = Vector3.zero;
			endPosition = Vector3.zero;
			currentDistance = 0f;
			distanceToEnd = 0f;
			screenFader.gameObject.SetActive(false);
		}
	}


	void OnTriggerStay(Collider other) 
	{
		Debug.Log("OnTriggerStay");
		if (other.gameObject == target)
		{
			currentDistance = Vector3.Distance((Vector3.Scale(other.gameObject.transform.position, restrictToAxis)), endPosition);  

			float targetAlpha = 1.0f - (currentDistance / distanceToEnd);
			screenFader.SetTargetAlpha(targetAlpha);
		}
	}

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		Debug.Log("entering position: " + enterPosition);
		Debug.Log ("this size: " + trigger.size);
		Debug.Log("end position: " + endPosition);
		Debug.Log("distanceToEnd: " + distanceToEnd);
		Debug.Log ("currentDistance: " + currentDistance);
	}

	IEnumerator LoadScene() 
	{
		async = Application.LoadLevelAsync(sceneName);
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
}
