using UnityEngine;
using System.Collections;

public class BossFightCamera : MonoBehaviour 
{
	public static bool IsIdle
	{
		get
		{
			return (cameraAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle_Camera_Animation"));
		}
	}
	public void PlayAnimation(int ID)
	{
		this.StartCoroutine(PlayOneShot(ID));
	}

	public static BossFightCamera instance;
	private static Animator cameraAnimator;

	// Use this for initialization
	void Start () {
		instance = this;
		cameraAnimator = this.GetComponentInChildren<Animator>();	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public IEnumerator PlayOneShot (int ID )
	{
		cameraAnimator.SetInteger( "AnimID", ID );
		yield return null;
		cameraAnimator.SetInteger( "AnimID", 0);
	}
}
