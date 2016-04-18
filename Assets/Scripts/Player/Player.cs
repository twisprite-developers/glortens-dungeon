using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(CharacterController))]

public class Player : MonoBehaviour 
{
	public CNAbstractController MovementJoystick;
	public CNAbstractController AttackTouchPad;

	public float maxHealth = 100;
	public float health = 100;
	public float healthRegenerationRate = 0.2f;
	
	public float attackAngle = 110f;
	public float attackRange = 2.0f;
	public float clubDamage = 40.0f;
	public float axeDamage = 100.0f;

	public float runSpeed= 4.0f;

	public float speedSmoothing = 8.0f;
	
	public float gravity = 20.0f;

	public GameObject clubBone;
	public GameObject axeBone;

	public ParticleSystem fallParticles;
	public ParticleSystem hitParticles;
	public ParticleSystem levelUpParticles;

	public GameObject gameOverScreen;

	public AudioClip GetGoldClip;
	public AudioClip GetAxeClip;
	public AudioClip AttackClip;
	public AudioClip HitClip;
	public AudioClip DeathClip;
	public AudioClip LandClip;
	public AudioClip LevelUpClip;
	public AudioSource AppearAudioSource;
	public AudioSource AmbientAudioSource;

	// The current move direction in x-z
	private Vector3 moveDirection= Vector3.zero;
	// The current vertical speed
	private float verticalSpeed= 0.0f;
	// The current x-z move speed
	private float moveSpeed= 0.0f;
	
	// The last collision flags returned from controller.Move
	private CollisionFlags collisionFlags; 

	// Is the user pressing any keys?
	private bool isMoving= false;

	private Animator animator;

	private HUD hud;

	// Toyx properties
	private int level = 0;
	private int experience = 0;
	private int gold = 0;

	// Toyx Manager
	private ToyxManager toyxManager;

	private bool hasAxe = false;

	void Awake ()
	{
		moveDirection = transform.TransformDirection(Vector3.forward);
		animator = GetComponentInChildren<Animator>();
	}

	void Start() 
	{
		// Get HUD
		hud = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUD>();
		
		// GetToyxManager
		GameObject toyxManagerGameObject = GameObject.Find("ToyxManager");
		if (toyxManagerGameObject != null)
		{
			toyxManager = toyxManagerGameObject.GetComponent<ToyxManager>();

			level = toyxManager.level;
			AddExperience(toyxManager.experience);
			AddGold(toyxManager.gold);

			// Set weapon
			if (toyxManager.weapon == ToyxManager.WeaponType.Axe) {
				hasAxe = true;
			}
			EquipAxe(hasAxe);
		}
		else
		{
			Debug.LogError("ToyxManager not found on scene, data will not be sent to the server!");
		}

		// Set health
		health = maxHealth;

		animator.SetTrigger("appear");
		Invoke("ShowFallSmoke", 1.2f);
	}
		
	void Update ()
	{
	

		ApplyGravity();

		//don't move if we are animating the Appear animation
		AnimatorStateInfo currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
		if (currentStateInfo.IsName("Appear"))
		{
			return;
		}

		//also don't move if we are dead
		if (health <= 0.0f)
			return;

		//regenerate health
		health += Time.deltaTime * healthRegenerationRate;
		health = Mathf.Min(health, maxHealth);

		UpdateSmoothedMovementDirection();


		// Calculate actual motion
		Vector3 movement= moveDirection * moveSpeed + new Vector3 (0, verticalSpeed, 0);
		movement *= Time.deltaTime;
		
		// Move the controller
		CharacterController controller = GetComponent<CharacterController>();
		collisionFlags = controller.Move(movement);
		
		// ANIMATION sector
		if(animator != null) 
		{
			if (isMoving)
			{
				//avisar al animador
				animator.SetFloat("speed", 1.0f);
			}
			else
			{
				animator.SetFloat("speed", 0.0f);
			}
		
		}
		// ANIMATION sector
		
		// Set rotation to the move direction
		if (IsGrounded())
		{		
			transform.rotation = Quaternion.LookRotation(moveDirection);		
		}	
		else
		{
			Vector3 xzMove= movement;
			xzMove.y = 0;
			if (xzMove.sqrMagnitude > 0.001f)
			{
				transform.rotation = Quaternion.LookRotation(xzMove);
			}
		}	

		//update life every frame because it changes constantly
		hud.SetLife((float)health / (float)maxHealth);


		if (AppearAudioSource.enabled && !AmbientAudioSource.enabled)
		{
			//when we start moving, enable ambient sound
			AmbientAudioSource.enabled = true;
		}

		#if UNITY_ANDROID
		if (Input.GetKeyDown(KeyCode.Escape)) 
		{
			//save before quit
			if (toyxManager != null)
			{
				toyxManager.Save(null);
			}
			Application.LoadLevel("menu");
		}
		#endif
	}

	
	void  ApplyGravity ()
	{
		if (IsGrounded ())
			verticalSpeed = 0.0f;
		else
			verticalSpeed -= gravity * Time.deltaTime;
	}

	void  UpdateSmoothedMovementDirection (){
		Transform cameraTransform= Camera.main.transform;
		bool grounded= IsGrounded();
		
		// Forward vector relative to the camera along the x-z plane	
		Vector3 forward= cameraTransform.TransformDirection(Vector3.forward);
		forward.y = 0;
		forward = forward.normalized;
		
		// Right vector relative to the camera
		// Always orthogonal to the forward vector
		Vector3 right= new Vector3(forward.z, 0, -forward.x);
		
		float v= Input.GetAxisRaw("Vertical") + MovementJoystick.GetAxis("Vertical");
		float h= Input.GetAxisRaw("Horizontal") + MovementJoystick.GetAxis("Horizontal");
		
		isMoving = Mathf.Abs (h) > 0.1f || Mathf.Abs (v) > 0.1f;
		
		// Target direction relative to the camera
		Vector3 targetDirection= h * right + v * forward;
		
		// Grounded controls
		if (grounded)
		{
			if (targetDirection != Vector3.zero)
			{
				moveDirection = targetDirection.normalized;
			}
			
			float curSmooth= speedSmoothing * Time.deltaTime;
			
			float targetSpeed= Mathf.Min(targetDirection.magnitude, 1.0f) * runSpeed;
			
			moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, curSmooth);
		}
		
	}
	
	public void AddExperience(int value) {

		experience += value;

		// Calculate level
		int previousLevelValue = 0;
		int nextLevelValue = 0;
		int previousLevel = level;
		level = 0;

		do {

			previousLevelValue = (int)Mathf.Pow(2,level - 1) * 100;
			nextLevelValue = (int)Mathf.Pow(2,level) * 100;
			level ++;

		}while (nextLevelValue <= experience);


		// Sound and particles
		if (level > previousLevel) {
			AudioSource.PlayClipAtPoint(LevelUpClip, Camera.main.transform.position);
			if (levelUpParticles != null) {
				levelUpParticles.Play();
			}
		}

		// Update level and experience in HUD
		hud.SetLevel(level);
		hud.SetExperience( ((float)(experience - previousLevelValue)) / ((float)(nextLevelValue - previousLevelValue)));

		// Save toyx
		if (toyxManager != null)
		{
			toyxManager.experience = experience;
			toyxManager.level = level;
			toyxManager.Save(null);
		}
	}

	public void AddGold(int value) {

		gold += value;
		hud.SetGold(gold);
		if (toyxManager != null)
		{
			toyxManager.gold = gold;
			toyxManager.Save(null);
		}
	}


	public float GetSpeed (){
		return moveSpeed;
	}

	public bool  IsGrounded (){
		return (collisionFlags & CollisionFlags.CollidedBelow) != 0;
	}
	
	public Vector3 GetDirection (){
		return moveDirection;
	}

	public bool IsMoving (){
		return Mathf.Abs(Input.GetAxisRaw("Vertical")) + Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.5f;
	}
	
	void Reset (){
		gameObject.tag = "Player";
	}

	public void OnEnable()
	{
		//subscribe to touchpad's events
		AttackTouchPad.FingerTouchedEvent += OnAttackTouchPadTouched;
		AttackTouchPad.FingerTouchedEvent += OnAttackTouchPadLifted;
	}

	public void OnDisable()
	{
		//unsubscribe to touchpad's events
		AttackTouchPad.FingerTouchedEvent += OnAttackTouchPadTouched;
		AttackTouchPad.FingerTouchedEvent += OnAttackTouchPadLifted;
	}

	public void OnAttackTouchPadTouched(CNAbstractController source)
	{
		if (health <= 0.0f)
			return;

		if (animator != null)
		{
			//don't attack if appearing or already attacking
			AnimatorStateInfo currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
			if (currentStateInfo.IsName("Appear") || currentStateInfo.IsName("Attack1") || currentStateInfo.IsName("Attack2"))
			{
				//Debug.Log("skipping attack, already attacking!");
				return;
			}

			//attack sound
			AudioSource.PlayClipAtPoint(AttackClip, Camera.main.transform.position);

			//attack animation
            animator.SetTrigger("attack");

			//do some damage
			Enemy nearestEnemy = FindNearestEnemyInFront();
			if (nearestEnemy != null)
			{
				nearestEnemy.Hit((hasAxe) ? axeDamage : clubDamage);
			}

        }
	}

	public void OnAttackTouchPadLifted(CNAbstractController source)
	{
		
	}   

	public void Hit(float damage)
	{
		health -= damage;

		if (health <= 0)
		{
			//DEAD!!
			if (animator != null)
			{
				AudioSource.PlayClipAtPoint(DeathClip, Camera.main.transform.position);
				animator.SetTrigger("dead");
				Invoke("ShowGameOverScreen", 2.0f);
            }
        }
        else
		{
			if (animator != null)
			{
				AudioSource.PlayClipAtPoint(HitClip, Camera.main.transform.position);
				animator.SetTrigger("hit");
            }
        }

		//show particles
		if (hitParticles != null)
		{
			hitParticles.Play ();
		}
	}

	protected Enemy FindNearestEnemyInFront()
	{        
		Enemy nearestEnemy = null;

		List<GameObject> allEnemies = Utils.GetLivingEnemies();
		foreach(GameObject go in allEnemies)
		{
			if (go == null)
				continue;

			Enemy enemy = go.GetComponent<Enemy>();
			if (enemy.health <= 0.0f)
				continue;

			// Create a vector from the player to the enemy
			Vector3 direction = new Vector3(go.transform.position.x - transform.position.x, transform.position.y, go.transform.position.z - transform.position.z);

			//if we are close enought
			if (direction.magnitude <= attackRange)
			{
				//store the angle between it and forward.
				float angle = Vector3.Angle(direction, transform.forward);
				
				// If the angle between forward and where the enemy is, is less than half the angle of view, he is in range
				if(angle < attackAngle * 0.5f)
				{
					nearestEnemy = enemy;
		        }
			}
		}
		return nearestEnemy;
    }

	void OnTriggerEnter (Collider other)
	{
		// If the player has touched the axe
		if(other.gameObject.tag == "Axe")
		{
			AudioSource.PlayClipAtPoint(GetAxeClip, Camera.main.transform.position);
			EquipAxe(true);
			//show particles (reuse the hit particles)
			if (hitParticles != null)
			{
				hitParticles.Play ();
			}
			Destroy(other.gameObject);
		}
		else if (other.gameObject.tag == "Gold")
		{
			AudioSource.PlayClipAtPoint(GetGoldClip, Camera.main.transform.position);

			AddGold(1);
			//show particles (reuse the hit particles)
			if (hitParticles != null)
			{
				hitParticles.Play ();
			}
			Destroy (other.gameObject);
		}
	}

	private void EquipAxe(bool hasAxe)
	{
		this.hasAxe = hasAxe;
		clubBone.SetActive(!hasAxe);
		axeBone.SetActive(hasAxe);

		if (toyxManager != null)
		{
			toyxManager.weapon = (hasAxe) ? ToyxManager.WeaponType.Axe : ToyxManager.WeaponType.Nothing;
			toyxManager.Save(null);
		}
	}

	private void ShowFallSmoke()
	{
		AudioSource.PlayClipAtPoint(LandClip, Camera.main.transform.position);
		fallParticles.Play();

		Invoke("PlayAppearSound", 1.0f);
	}

	private void PlayAppearSound()
	{
		AppearAudioSource.enabled = true;
	}

	private void ShowGameOverScreen()
	{
		gameOverScreen.SetActive(true);
	}
}