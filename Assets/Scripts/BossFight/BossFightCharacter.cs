using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BossFightCharacter : MonoBehaviour 
{

	[System.Serializable]
	public class BossFightCharacterAttack
	{
		public int CameraAnimationID;
		public int CharacterAnimationID;
		public int minDamage;
		public int maxDamage;
		public AudioClip clip;
	}
	public List<BossFightCharacterAttack> characterAttacks = new List<BossFightCharacterAttack>();

	public int maxHealth = 100;
	public int health = 100;

	public ParticleSystem hitParticles;

	public AudioClip AttackClip;
	public AudioClip HitClip;
	public AudioClip DeathClip;

	public HealthBar healthBar;
	public Text healthText;
	
	private Animator characterAnimator;
	public Animator CharacterAnimator {
		get {
			return characterAnimator;
		}
	}

	private bool readyToAttack = false;
	public bool ReadyToAttack {
		get {
			return readyToAttack;
		}
	}

	private AudioSource audioSource;

	// Toyx properties
	private int level = 0;
	private int experience = 0;
	private int gold = 0;

	// Toyx Manager
	private ToyxManager toyxManager;
	private bool hasAxe = false;

	private Vector2 healthTextOriginalPosition;

	void Awake ()
	{
		characterAnimator = GetComponentInChildren<Animator>();
	}

	void Start() 
	{
		// Set health
		health = maxHealth;

		if (healthBar != null)
		{
			healthBar.SetMaxHealth(maxHealth);
		}

		if (healthText != null)
		{
			healthTextOriginalPosition = healthText.rectTransform.anchoredPosition;
		}
	}
		
	void Update ()
	{
		//don't move if we are dead
		if (health <= 0.0f)
			return;


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


	public void YourTurn(bool isYourTurn)
	{
		//DEAD!!
		if (health <= 0 && characterAnimator != null)
		{
			PlayClip(DeathClip, true);
			characterAnimator.SetTrigger("dead");	
			return;
		}
		readyToAttack = isYourTurn;
	}

	public void Hit(int damage, string triggerName = "hit")
	{
		Debug.Log("receiving damage: " + damage);
		health -= damage;
		if (health <= 0) { health = 0; }

		//update healthbar
		if (healthBar != null)
		{
			healthBar.SetHealth(health);
		}

		//update healthText
		if (healthText != null)
		{
			healthText.gameObject.SetActive(true);
			healthText.rectTransform.anchoredPosition = healthTextOriginalPosition;
			Go.killAllTweensWithTarget(healthText);
			Go.killAllTweensWithTarget(healthText.transform);

			healthText.text = "-" + damage;
			healthText.color = Color.red;

			Go.to( healthText, 2.0f, new GoTweenConfig()
			      .colorProp( "color", new Vector4( 1.0f, 0.0f, 0.0f, 0.0f) )
			      .setEaseType(GoEaseType.SineOut)
			      .onComplete( tween =>
		            {
						tween.rewind();	
					}));
			
			Go.to( healthText.rectTransform, 2.0f, new GoTweenConfig()
			      .vector2Prop("anchoredPosition", new Vector2(0.0f,100.0f), true)
			      .setEaseType(GoEaseType.SineOut)
			      .onComplete( tween =>
		            {
							tween.rewind();	
							healthText.gameObject.SetActive(false);
					}));
		}
		
		//show particles
		if (hitParticles != null)
		{
			hitParticles.Play ();
		}

		//animate
		if (characterAnimator != null)
		{	
			PlayClip(HitClip, true);
			characterAnimator.SetTrigger(triggerName);
		}

		if (health == 0)
		{
			readyToAttack = false;
        }
	}

	public void Heal(int quantity)
	{
		//update healthbar
		if (healthBar != null)
		{
			healthBar.SetHealth(health+quantity);
		}

		//update healthText
		if (healthText != null)
		{
			healthText.gameObject.SetActive(true);
			healthText.text = "+" + quantity;
			healthText.color = Color.green;
			
			Go.to( healthText, 2.0f, new GoTweenConfig()
			      .colorProp( "color", new Vector4( 0.0f, 1.0f, 0.0f, 0.0f) )
			      .setEaseType(GoEaseType.SineOut)
			      .onComplete( tween =>
			            {
				tween.rewind();	
			}));
			
			Go.to( healthText.rectTransform, 2.0f, new GoTweenConfig()
			      .vector2Prop("anchoredPosition", new Vector2(0.0f,100.0f), true)
			      .setEaseType(GoEaseType.SineOut)
			      .onComplete( tween =>
			            {
				tween.rewind();	
				healthText.gameObject.SetActive(false);
			}));
		}

	}

	public int[] PerformAttack(int attackIndex)
	{
		//Debug.Log("Performing attack " + attackIndex);
		readyToAttack = false;
		BossFightCharacterAttack bfca = characterAttacks[attackIndex];
		BossFightCamera.instance.PlayAnimation(bfca.CameraAnimationID);
		PlayOnce(bfca.CharacterAnimationID);
		if (bfca.clip != null)
		{
			PlayClip(bfca.clip);
		}
		return new int[]{bfca.minDamage, bfca.maxDamage};
	}
	
	public void PlayOnce(int ID)
	{
		this.StartCoroutine(PlayOneShot(ID));
	}

	private IEnumerator PlayOneShot (int ID )
	{
		Debug.Log ("ANIMID = " + ID);
		characterAnimator.SetInteger( "AnimID", ID );
		yield return null;
		characterAnimator.SetInteger( "AnimID", 0);
	}

	private void PlayClip(AudioClip clip, bool randomizePitch = false)
	{
		if (audioSource == null)
		{
			audioSource = Camera.main.gameObject.AddComponent<AudioSource>();
		}

		if (randomizePitch)
		{
			audioSource.pitch = Random.Range(0.90f, 1.10f);
		}
		audioSource.clip = clip;
		audioSource.Play();
	}
}