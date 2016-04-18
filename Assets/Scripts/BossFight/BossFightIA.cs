using UnityEngine;
using System.Collections;

public class BossFightIA : MonoBehaviour 
{
	private float nextAttackWaitingTime;
	private BossFightCharacter bossChar;
	private bool myTurn = false;
	private BossFight bossFight;

	void Awake () 
	{
		bossChar = this.GetComponentInChildren<BossFightCharacter>();
	}

	public BossFightCharacter GetChar()
	{
		return bossChar;
	}

	public void SetBossFight(BossFight bf)
	{
		this.bossFight = bf;
	}

	// Update is called once per frame
	void Update () 
	{
		if (myTurn)
		{
			nextAttackWaitingTime -= Time.deltaTime;
			if (nextAttackWaitingTime <= 0)
			{
				DecideAndMove();
				nextAttackWaitingTime = float.MaxValue;
			}
		}
	
	}


	public void YourTurn(bool isYourTurn)
	{
		bossChar.YourTurn(isYourTurn);

		if (bossChar.health <= 0)
			return;

		myTurn = isYourTurn;
		//set a little delay before attacking
		nextAttackWaitingTime = Random.Range(1.0f, 2.0f);
	}

	private void DecideAndMove()
	{
		int attackIndex = 0;
		//start performing double attack when low on health
		if (bossChar.health < (bossChar.maxHealth / 2))
		{
			attackIndex = Random.Range(0, bossChar.characterAttacks.Count);
		}
		else
		{
			attackIndex = Random.Range(0, bossChar.characterAttacks.Count - 1);
		}

		int[] damages = bossChar.PerformAttack(attackIndex);
		bossFight.currentAttackDamage = Random.Range(damages[0], damages[1] + 1);
		
		Vector3 attackPlayerPosition = new Vector3(bossChar.transform.localPosition.x, bossChar.transform.localPosition.y, -0.2f);
		//special cases
		switch(attackIndex)
		{
		case 0:
			//normal attack	 1				
			//move
			Go.to( bossChar.transform, 0.4f, new GoTweenConfig()
			      .localPosition(attackPlayerPosition)
			      .setEaseType(GoEaseType.Linear)
			      .setIterations(2, GoLoopType.PingPong)
			      .onIterationEnd( tween => {		
				if (tween.completedIterations < 2)					
				{
					bossFight.DamagePlayer(null);
				}}
			)
			      .onComplete(bossFight.ChangeTurn));
			break;
		case 1:
			//normal attack	 2				
			//move
			Go.to( bossChar.transform, 0.6f, new GoTweenConfig()
			      .localPosition(attackPlayerPosition)
			      .setEaseType(GoEaseType.Linear)
			      .setIterations(2, GoLoopType.PingPong)
			      .onIterationEnd( tween => {		
				if (tween.completedIterations < 2)					
				{
					bossFight.DamagePlayer(null);
				}}
			)
			      .onComplete(bossFight.ChangeTurn));
			break;
		case 2: 			
			//double attack				
			
			int attack1 = (bossFight.currentAttackDamage / 3);
			int attack2 = bossFight.currentAttackDamage - attack1;
			//move
			Go.to( bossChar.transform, 0.4f, new GoTweenConfig()
			      .localPosition(attackPlayerPosition)
			      .setEaseType(GoEaseType.Linear)
			      .setIterations(2, GoLoopType.PingPong)
			      .onIterationEnd( tween => {		
				if (tween.completedIterations < 2)					
				{
					bossFight.currentAttackDamage = attack1;
					bossFight.DamagePlayer(null);
				}}
			)
			      .onComplete( tween2 => {
				
				Go.to( bossChar.transform, 0.6f, new GoTweenConfig()
				      .localPosition(attackPlayerPosition)
				      .setEaseType(GoEaseType.Linear)
				      .setIterations(2, GoLoopType.PingPong)
				      .onIterationEnd( tween3 => {		
					if (tween3.completedIterations < 2)					
					{
						bossFight.currentAttackDamage = attack2;
						bossFight.DamagePlayer(null);
					}}
				)
				      .onComplete(bossFight.ChangeTurn));
			}
			));
			break;
		}
	}
}
