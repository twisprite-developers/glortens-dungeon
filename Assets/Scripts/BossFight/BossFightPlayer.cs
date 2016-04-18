using UnityEngine;
using System.Collections;

public class BossFightPlayer : MonoBehaviour 
{
	private BossFightCharacter glorten;
	private BossFight bossFight;
	
	void Awake () {

		glorten = this.GetComponentInChildren<BossFightCharacter>();
	}

	public BossFightCharacter GetChar()
	{
		return glorten;
	}

	public void SetBossFight(BossFight bf)
	{
		this.bossFight = bf;
	}

	public void YourTurn(bool isYourTurn)
	{
		glorten.YourTurn(isYourTurn);
	}

	public void PerformAttack(int attackIndex)
	{
		if (glorten.ReadyToAttack)
		{
			int[] damages = glorten.PerformAttack(attackIndex);
			bossFight.currentAttackDamage = Random.Range(damages[0], damages[1] + 1);
			
			//special cases
			switch(attackIndex)
			{
			case 0:
				//normal attack					
				Vector3 attackBossPosition = new Vector3(glorten.transform.localPosition.x, glorten.transform.localPosition.y, 1.0f);
				//move
				Go.to( glorten.transform, 0.4f, new GoTweenConfig()
				      .localPosition(attackBossPosition)
				      .setEaseType(GoEaseType.Linear)
				      .setIterations(2, GoLoopType.PingPong)
				      .onIterationEnd( tween => {		
					if (tween.completedIterations < 2)					
					{
						bossFight.DamageBoss(null);
					}}
				)
				      .onComplete(bossFight.ChangeTurn));
				break;
			case 1: 					
				//heal
				Transform healSpell = glorten.transform.Find("HealSpell");
				
				//make decal appear
				Transform Decal = healSpell.Find("Decal");					
				if (Decal != null)
				{				
					SpriteRenderer sr = Decal.gameObject.GetComponent<SpriteRenderer>();
					sr.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
					sr.transform.localScale = new Vector3(0.4f,0.4f,1.0f);
					Go.to (sr, 5.0f, new GoTweenConfig()
					       .colorProp( "color", new Vector4( 1.0f, 1.0f, 1.0f, 1.0f) )
					       .setEaseType(GoEaseType.SineOut)
					       .onComplete(tween => 
					            { 
						Go.to (sr, 0.5f, new GoTweenConfig()
						       .colorProp( "color", new Vector4( 1.0f, 1.0f, 1.0f, 0.0f))
						       .setEaseType(GoEaseType.SineIn)
						       );
						
						Go.to (sr.transform, 0.5f, new GoTweenConfig()
						       .scale(new Vector3(0.1f,0.1f,1.0f))	
						       .setEaseType(GoEaseType.SineIn)
						       );	

								bossFight.HealPlayer(null);
								bossFight.ChangeTurn(null);								
							})					       							       	
					       );
				}
				
				//launch particles
				ParticleSystem[] particles = healSpell.GetComponentsInChildren<ParticleSystem>();
				foreach(ParticleSystem ps in particles)
				{
					ps.Play();
				}
				
				//rotate whole group
				healSpell.localScale = new Vector3(0.6f, 1.0f, 0.6f);
				Go.to ( healSpell, 5.0f, new GoTweenConfig()
				       .localRotation(new Vector3(0,1440f,0), true)
				       .scale(1.0f)
				       .setEaseType(GoEaseType.Linear));
				
				break;
			case 2:
				//fire attack
				Transform fireSpell = glorten.transform.Find("FireSpell");
				//rotate fireSpell
				Go.to( fireSpell,10.0f, new GoTweenConfig()
				      .localRotation(new Vector3(0,1440f,0), true)
				      .scale(1.0f)
				      .setEaseType(GoEaseType.Linear));
				
				//make decal appear
				Transform fireDecal = fireSpell.Find("Decal");					
				if (fireDecal != null)
				{				
					SpriteRenderer sr = fireDecal.gameObject.GetComponent<SpriteRenderer>();
					sr.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
					sr.transform.localScale = new Vector3(0.4f,0.4f,0.4f);
					Go.to (sr, 10.0f, new GoTweenConfig()
					       .colorProp( "color", new Vector4( 1.0f, 1.0f, 1.0f, 1.0f) )
					       .setEaseType(GoEaseType.SineOut)
					       .onComplete(tween => 
					            { 
						Go.to (sr, 0.5f, new GoTweenConfig()
						       .colorProp( "color", new Vector4( 1.0f, 1.0f, 1.0f, 0.0f))
						       .setEaseType(GoEaseType.SineIn)
						       );
						
						Go.to (sr.transform, 0.5f, new GoTweenConfig()
						       .scale(new Vector3(0.1f,0.1f,1.0f))	
						       .setEaseType(GoEaseType.SineIn)
						       .onComplete(bossFight.ChangeTurn)
						       );	
						
					})					       							       	
					       );
				}
				
				//launch particles
				ParticleSystem[] fireParticles = fireSpell.GetComponentsInChildren<ParticleSystem>();
				foreach(ParticleSystem ps in fireParticles)
				{
					ps.Play();
				}
				
				int fireball1Damage = (bossFight.currentAttackDamage / 7) * 1;
				int fireball2Damage = (bossFight.currentAttackDamage / 7) * 2;
				int fireball3Damage = (bossFight.currentAttackDamage / 7) * 4;
				
				//fireball1
				Transform fireBall1 = fireSpell.Find("FireBall1");
				fireBall1.localPosition = new Vector3(0.0f, fireBall1.localPosition.y ,0.0f);
				GoTween fb1tw = new GoTween( fireBall1, 1.0f, new GoTweenConfig()
				                            .position(new Vector3(0,0,5f), true)
				                            .setEaseType(GoEaseType.Linear)
				                            .onComplete(tweenfb1 =>
								            {
												bossFight.currentAttackDamage = fireball1Damage;
												bossFight.DamageBoss(null);												
											}));
				//fireball2
				Transform fireBall2 = fireSpell.Find("FireBall2");
				fireBall2.localPosition = new Vector3(0.0f, fireBall2.localPosition.y,0.0f);
				GoTween fb2tw = new GoTween( fireBall2, 1.0f, new GoTweenConfig()
				                            .position(new Vector3(0,0,5f), true)
				                            .setEaseType(GoEaseType.Linear)
				                            .onComplete(tweenfb2 =>
								            {
												bossFight.currentAttackDamage = fireball2Damage;
												bossFight.DamageBoss(null);	
											}));
				
				//fireball3
				Transform fireBall3 = fireSpell.Find("FireBall3");
				fireBall3.localPosition = new Vector3(0.0f, fireBall3.localPosition.y,0.0f);
				GoTween fb3tw = new GoTween( fireBall3, 1.0f, new GoTweenConfig()
				                            .position(new Vector3(0,0,5f), true)
				                            .setEaseType(GoEaseType.Linear)
				                            .onComplete(tweenfb3 =>
								            {
												bossFight.currentAttackDamage = fireball3Damage;
												bossFight.DamageBossBig(null);	
											}));
				
				GoTweenChain chain = new GoTweenChain();
				chain.appendDelay(3f).append( fb1tw ).appendDelay( 0.5f ).append( fb2tw ).appendDelay( 0.3f ).append( fb3tw );
				chain.play();
				
				break;
			case 3:
				//hearthquake
				Transform earthSpell = glorten.transform.Find("EarthSpell");
				//rotate spell
				Go.to( earthSpell,9.0f, new GoTweenConfig()
				      .localRotation(new Vector3(0,1440f,0), true)
				      .scale(1.0f)
				      .setEaseType(GoEaseType.Linear));
				
				//make decal appear
				Transform earthDecal = earthSpell.Find("Decal");					
				if (earthDecal != null)
				{				
					SpriteRenderer sr = earthDecal.gameObject.GetComponent<SpriteRenderer>();
					sr.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
					sr.transform.localScale = new Vector3(0.4f,0.4f,0.4f);
					Go.to (sr, 9.0f, new GoTweenConfig()
					       .colorProp( "color", new Vector4( 1.0f, 1.0f, 1.0f, 1.0f) )
					       .setEaseType(GoEaseType.SineOut)
					       .onComplete(tween => 
					            { 
						Go.to (sr, 0.5f, new GoTweenConfig()
						       .colorProp( "color", new Vector4( 1.0f, 1.0f, 1.0f, 0.0f))
						       .setEaseType(GoEaseType.SineIn)
						       );
						
						Go.to (sr.transform, 0.5f, new GoTweenConfig()
						       .scale(new Vector3(0.1f,0.1f,1.0f))	
						       .setEaseType(GoEaseType.SineIn)
						       .onComplete(bossFight.ChangeTurn)
						       );	
					})					       							       	
					       );
				}
				
				//launch particles
				ParticleSystem[] earthParticles  = earthSpell.GetComponentsInChildren<ParticleSystem>();
				foreach(ParticleSystem ps in earthParticles)
				{
					ps.Play();
				}
				
				//earthquake rocks
				Transform movingRocks = earthSpell.Find("MovingRocks");
				movingRocks.localPosition = new Vector3(0.0f, 0.0f ,1.56f);
				Go.to( movingRocks, 1.0f, new GoTweenConfig()
				      .position(new Vector3(0,0,5f), true)
				      .setEaseType(GoEaseType.Linear)
				      .setDelay(5.0f)
				      .onComplete(bossFight.DamageBossBig));									
				
				break;
			}
		}
	}
}
