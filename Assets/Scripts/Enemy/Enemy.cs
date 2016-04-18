using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{

	public float attackRate = 1.5f;						// Rate at which the enemy will keep attacking
	public float attackRange = 2.5f;					// Attack range of the enemy
	public float attackDamage = 12.0f;					// Damage the enemy does when attacking		
	public float health = 100.0f;		
	public int experience = 10;
	public ParticleSystem hitParticles;					// Reference to the particle system to be played when hit
	public GameObject enemyDebris;						// Reference to the dismembered skeleton that will be spawned when dead
	public GameObject goldIngot;						// Reference to the gold Ingot the skeleton will spawn when dead

	public AudioClip summonClip;
	public AudioClip hitClip;
	public AudioClip awakeClip;
	public AudioClip deathClip;
	    
	private EnemySight enemySight;						// Reference to the EnemySight script.
	private Vector3 previousSightingPosition;			// Last place where the player was seen
	private float nextAttackTime;						// Time to the next attack to be done
	private NavMeshAgent nav;							// Reference to the nav mesh agent.
	private Player player;								// Reference to the player
	private Animator animator;							// Reference to the animator controller
	private EnemySpawner spawner;						// Reference to the Spawner that spawned this enemy

	public enum EnemyState : int
	{
		IDLE = 0,
		CHASING,
		ATTACKING,
		DEAD
	}
	private EnemyState enemyState;
	
	void Awake ()
	{
		// Setting up the references.
		enemyState = EnemyState.IDLE;
		enemySight = GetComponent<EnemySight>();
		nav = GetComponent<NavMeshAgent>();
		player = GameObject.Find("Player").GetComponent<Player>();
		animator = GetComponentInChildren<Animator>();
	}

	void Start()
	{
		//play summon sound where the skeleton is, but to the same height as the camera
		AudioSource.PlayClipAtPoint(summonClip, new Vector3(transform.position.x, Camera.main.transform.position.y, transform.position.z));
	}
	
	
	void Update ()
	{
		switch(enemyState)
		{
			case EnemyState.IDLE:
				UpdateIdle();
				break;
			case EnemyState.CHASING:
				UpdateChasing();
				break;
			case EnemyState.ATTACKING:
				UpdateAttacking();
				break;
			case EnemyState.DEAD:
				//don't continue to update speed if dead
				return;				
		}

		animator.SetFloat("speed", nav.velocity.magnitude);
	}

	private void SetState(EnemyState newState)
	{
		//just in case we need to initialize something before setting the state...
		switch(newState)
		{
		case EnemyState.IDLE:
			break;
		case EnemyState.CHASING:
			AudioSource.PlayClipAtPoint(awakeClip, Camera.main.transform.position);
			break;
		case EnemyState.ATTACKING:
			nav.Stop();
			nextAttackTime = 0.0f;
			break;
		case EnemyState.DEAD:
			nav.Stop();
			//remove from living enemies array
			Utils.RemoveLivingEnemy(this.gameObject);
			spawner.UnManageEnemy(this.gameObject);

			//spawn the debris passing the material
			Material skeletonMaterial = this.GetComponentInChildren<SkinnedMeshRenderer>().material;
			GameObject enemyDebrisGameObject = (GameObject) Instantiate(enemyDebris, this.transform.position, this.transform.rotation);
			EnemyDebris ed = enemyDebrisGameObject.GetComponent<EnemyDebris>();
			ed.Init(skeletonMaterial);

			//spawn the gold ingot
			Instantiate(goldIngot, this.transform.position, this.transform.rotation);

			//add experience to the player
			player.AddExperience(this.experience);

			DestroyImmediate(this.gameObject);
			return;
        }

		this.enemyState = newState;
	}


	void UpdateIdle()
	{
		// If the player is in sight and is alive...
		if(enemySight.playerInSight && player.health > 0f)
		{
			SetState(EnemyState.CHASING);
		}

	}

	void UpdateChasing ()
	{
		if (!enemySight.playerInSight || player.health <= 0.0f)
		{
			SetState(EnemyState.IDLE);
			return;
		}

		Vector3 currentSightingPosition = enemySight.sightingPosition;
		if (previousSightingPosition != currentSightingPosition)
		{
			// keep chasing, set the destination for the NavMeshAgent to the last personal sighting of the player.
			// don't move to the exact position of the player to avoid collision, instead move to close range
			Vector3 inRangePosition = currentSightingPosition + ((transform.position - currentSightingPosition).normalized * (attackRange*0.55f));
			nav.destination = inRangePosition;
			previousSightingPosition = currentSightingPosition;
		}

		// Create a vector from the enemy to the last sighting of the player.
		Vector3 deltaPos = currentSightingPosition - transform.position;
		
		//is in attack range!, attack him!
		if(deltaPos.sqrMagnitude <= attackRange)
		{
			SetState(EnemyState.ATTACKING);
        }
	}
       
	void UpdateAttacking()
	{
		//if player went away, chase him again
		Vector3 deltaPos = enemySight.sightingPosition - transform.position;
		
		// If the the last personal sighting of the player is not close...
		if(deltaPos.sqrMagnitude > attackRange)
		{
			//where did he go? he is d**n fast!
			SetState(EnemyState.IDLE);
		}
		else if (player.health <= 0)
		{
			//we killed the big bad dwarf, yay!
			SetState(EnemyState.IDLE);
		}
		else
		{

			//keep looking at player
			Vector3 direction = new Vector3(player.transform.position.x - transform.position.x, transform.position.y, player.transform.position.z - transform.position.z);
			direction.Normalize();
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.3f * Time.deltaTime);
			
			nextAttackTime -= Time.deltaTime;
			if (nextAttackTime <= 0.0f)
			{
				nextAttackTime = attackRate;
				animator.SetTrigger("attack");
				player.Hit(attackDamage);
			}
		}
	}

	public void Hit(float damage)
	{
		health -= damage;

		//show particles
		if (hitParticles != null)
		{
			hitParticles.Play ();
        }

		if (health > 0.0f)
		{
			//hit
			AudioSource.PlayClipAtPoint(hitClip, Camera.main.transform.position);
			animator.SetTrigger("hit");
		}
		else
		{
			//dead!
			AudioSource.PlayClipAtPoint(deathClip, Camera.main.transform.position);
			SetState(EnemyState.DEAD);                       
		}	
	}

	public void SetSpawner(EnemySpawner spawner)
	{
		this.spawner = spawner;
	}
}
