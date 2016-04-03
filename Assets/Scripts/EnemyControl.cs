using UnityEngine;
using System.Collections;

public class EnemyControl : BaseCharacter {

	//Our sounds
	private AudioSource hurt;

	//Our target to fight
	private PlayerControl player;

	//Our boolean if we are dead
	bool dead;

	//Our boolean if we are colliding with the player
	bool playerCollide;

	//Our amount of frames before we deal damage
	int totalFrames;
	int attackFrames;

	//Our Enenemy Damage Multiplier
	public int eDamage;

	// Use this for initialization
	protected override void Start ()
	{
		base.Start ();

		//Get our sounds
		hurt = GameObject.Find ("Hurt").GetComponent<AudioSource> ();

		//Set our Health
		maxHealth = maxHealth / 2;
		curHealth = maxHealth;

		//Set the enemy damage
		eDamage = eDamage * 6;


		//set dead to false;
		dead = false;
		playerCollide = false;

		//Set our attack frames
		totalFrames = 19;
		attackFrames = totalFrames;

		//Go after our player!
		player = GameObject.FindGameObjectWithTag ("PlayerChar").GetComponent<PlayerControl>();

	}

	// Update is called once per frame
	protected override void Update ()
	{
		base.Update ();

		if(!gameManager.getGameStatus() && !dead && !playerCollide)
		{
			//Check how far we are
			if(Vector3.Distance(gameObject.transform.position, player.transform.position) < 2.25) {

				//Move our enemy
				enemyMove ();
			}
		}

		//Check if enemy is dead
		if (curHealth <= 0 && !dead)
		{
			//Decrease the number of enemies we have
			gameManager.minusEnemy();

			//Destroy this enemy, possible display some animation first
			//Destroy(gameObject);

			//Move our enemy out of the way and play the death animation
			//animator.SetTrigger("DeathTrigger");
			//animator.SetBool("Death", true);

			//Set our sorting layer as a corpse so we step on top of it
			render.sortingLayerName = "Permenance";

			//and remove box collider
			GetComponent<Rigidbody2D>().isKinematic = true;
			GetComponent<BoxCollider2D>().enabled = false;

			//Set death boolean to true
			dead = true;

			//Check if it was the boss
			if (gameObject.tag == "BossChar") {

				GameObject.Find("StateManager").GetComponent<StateManager>().defeatedFinalBoss ();
			}

			//Set the death trigger
			animator.SetTrigger("Death");

			//Start the death coroutine
			StartCoroutine("deathStall");

		}

	}

	//Function to move our player
	void enemyMove ()
	{
		//Get our angle stuff
		float h = gameObject.transform.position.x - player.transform.position.x;

		//How fast we move
		float moveAmount = 300.0f;

		//Need to make a vector here to move towards
		Vector2 towards;

		if (h < 0) {

			towards = new Vector2 (player.transform.position.x + moveAmount, 0);
			direction = 1;
			animator.SetBool ("Running", true);

		} else if (h > 0) {

			towards = new Vector2 (player.transform.position.x - moveAmount, 0);
			direction = -1;
			animator.SetBool ("Running", true);
		} else {
			towards = new Vector2 (player.transform.position.x + moveAmount, 0);
			direction = 0;
			animator.SetBool ("Running", false);
		}


		//Get our speed according to our current level
		//Using enemy skill
		float superSpeed = moveSpeed * 2.45f;

		//movement vector
		Vector2 move = Vector2.MoveTowards(transform.position, towards, superSpeed * Time.deltaTime);

		//Get the position we want to move to, and go to it using move towards
		charBody.MovePosition(move);

		//Flip our sprite
		setFlip();
	}

	//Catch when we collide with enemy
	void OnCollisionStay2D(Collision2D collision)
	{
		//Check if it is the player
		if (collision.gameObject.tag == "PlayerChar") {

			//Set player collide to true
			playerCollide = true;

			//Decrease the number of frames until we attack
			if (attackFrames > 0) {
				--attackFrames;
			} else if (dead) {
				//Do nothing if dead
			}
			//attack the player
			else {

				//Set the attack trigger of the player's animation controller in order to play the player's attack animation.
				animator.SetTrigger ("Attack");

				//Only do damage if they are not dodging
				//Get the player
				PlayerControl p = (PlayerControl)collision.gameObject.GetComponent<PlayerControl>();

				//Now using an int to calulate our damage before we apply to health
				//Using enemy skill
				int damage = (int)(eDamage * 1.5);
				if (damage < 1) {
					damage = 1;
				}

				int newHealth = p.getHealth () - damage;
				p.setHealth (newHealth);

				//Play the sound of hurt, only if the game is still on
				if (!gameManager.getGameStatus ()) {

					if (hurt.isPlaying)
						hurt.Stop ();
					hurt.Play ();
				}

				//Shake the screen
				actionCamera.startShake ();

				//impact pause from the player
				actionCamera.startImpact ();

				//Reset attack frames
				attackFrames = totalFrames;

			}
		}

	}


	//Catch when we collide with something
	void OnCollisionExit2D(Collision2D collision)
	{

		//Check if it is the player
		if (collision.gameObject.tag == "PlayerChar") {

			//Set player collide to false
			playerCollide = false;
		}
	}

	//simply stall death before destroying
	//Function for jumping
	IEnumerator deathStall() {
		
		//Wait some frames
		for(float i = 100.0f; i > 0; i--) {
			
			//slighlty change the opacity
			render.color = new Color(render.color.r, render.color.g, render.color.b, (i / 100.0f));
			yield return new WaitForFixedUpdate();
		}

		//Destroy the game object
		Destroy(gameObject);
	}
}
