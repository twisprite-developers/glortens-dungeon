using UnityEngine;
using System.Collections;

public class BossFightCinematic : MonoBehaviour 
{

	private BossFightCharacter glorten;
	private BossFightCharacter boss;

	private Vector3 glortenCombatPosition;
	private float glortenWalkInDistance = -19f;
	private float glortenWalkInTime = 15.0f;
	private float glortenReadyTime = 1.0f;
	
	private Vector3 bossCombatPosition;
	private float bossWalkInDistance = 18f;
	private float bossWalkInTime = 6.0f;
	private float bossStopTime = 2.0f;
	private float bossRoarTime = 4.5f;
	private float bossRoarAudioTime = 1.0f;

	private float endPause = 4.0f;

	private int nextActionIndex = 0;
	private float nextActionTime = 0;

	private bool finished = false;
	public bool Finished {
		get {
			return finished;
		}
	}

	public AudioClip BossRoar;

	bool debugmode = false;

	public void Begin(BossFightCharacter glorten, BossFightCharacter boss)
	{
		finished = false;

		if (debugmode)
		{
			nextActionTime = 0;
			nextActionIndex = 6;
			return;
		}

		//init glorten
		this.glorten = glorten;
		glortenCombatPosition = glorten.transform.position;
		glorten.transform.position = new Vector3(glortenCombatPosition.x, 0.0f, glortenCombatPosition.z + glortenWalkInDistance);	

		//init boss
		this.boss = boss;
		bossCombatPosition = boss.transform.position;
		boss.transform.position = new Vector3(bossCombatPosition.x, bossCombatPosition.y, bossCombatPosition.z + bossWalkInDistance);	
	}

	// Update is called once per frame
	void Update () 
	{
		nextActionTime-= Time.deltaTime;
		if (nextActionTime<= 0)
		{
			NextAction();
		}
		
	}

	private void NextAction()
	{
		switch(nextActionIndex)
		{
			case 0:
				//camera animation
				BossFightCamera.instance.PlayAnimation(-1);

				//glorten enter animation
				glorten.CharacterAnimator.SetBool("walking", true);

				//move
				Go.to( glorten.transform, glortenWalkInTime, new GoTweenConfig()
				      .position(glortenCombatPosition)
				      .setEaseType(GoEaseType.Linear));

				//play boss walk animation
				boss.CharacterAnimator.SetBool("walking", true);
					
				nextActionTime = bossWalkInTime;
			break;
			case 1:
				//teleport boss
				boss.transform.position = new Vector3(bossCombatPosition.x, bossCombatPosition.y, bossCombatPosition.z + 3.0f);	
				
				//move boss				
				Go.to( boss.transform, bossStopTime, new GoTweenConfig()
				      .position(bossCombatPosition)
				      .setEaseType(GoEaseType.SineInOut));		

				nextActionTime = bossStopTime;
			break;
			case 2:
				
				//teleport glorten to his position
				Go.killAllTweensWithTarget(glorten.transform);
				glorten.transform.position = glortenCombatPosition;
				
				//glorten stop animating
				glorten.CharacterAnimator.SetBool("walking", false);

				//boss stop animating
				boss.CharacterAnimator.SetBool("walking", false);
				nextActionTime = glortenReadyTime;
			break;
			case 3:
				//glorten braces
				glorten.PlayOnce(-1);
				nextActionTime = bossRoarTime;
			break;
			case 4:
				//boss roar animation
				boss.PlayOnce(-1);		
				nextActionTime = bossRoarAudioTime;
			break;
			case 5:
				//boss roar audio
				if (BossRoar != null) 
				{
					this.transform.parent = Camera.main.transform;
					this.transform.localPosition = Vector3.zero;
                    AudioSource asource = this.gameObject.AddComponent<AudioSource>();
					asource.clip = BossRoar;
					asource.Play();                    
                }
				nextActionTime = endPause;
                break;
			case 6:
				finished = true;
				nextActionTime = float.MaxValue;
			break;
		}

		nextActionIndex++;
	}


	// Use this for initialization
	void Start () {
	
	}
	

}
