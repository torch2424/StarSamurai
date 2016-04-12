﻿using UnityEngine;
using System.Collections;

public class PlayerControl : BaseCharacter {

	//Our sounds
	private AudioSource jump;
	private AudioSource attack;
	private AudioSource attackHit;

	//Boolean to check if attacking
	bool attacking;


	//Our Number of jumps we have done
	public float jumpHeight;
	private int jumps;
	private bool grounded;
	public Transform groundCheck;
	public LayerMask groundLayer;
	private bool walled;
	public Transform wallCheck;
	public LayerMask wallLayer;
	public float checkRadius;


	//Counter for holding space to punch
	private int holdAttack;
	//How long do they have to hold before attacking
	public int holdDuration;

	//Amount of damage sword deals
	public int playerDamage;

	// Use this for initialization
	protected override void Start ()
	{
		//Call our superclass start
		base.Start();

		//Get our sounds
		jump = GameObject.Find ("Jump").GetComponent<AudioSource> ();
		attack = GameObject.Find ("Attack").GetComponent<AudioSource> ();
		attackHit = GameObject.Find ("AttackHit").GetComponent<AudioSource> ();

		//Set our actions
		attacking = false;
		jumps = 0;
		holdAttack = 0;

		//Set up our Jumping Ground Detection
		grounded = false;
	}

	// Update is called once per frame
	protected override void Update ()
	{
		//Call our base update
		base.Update ();

		//check if dead, allow movement if alive
		if (curHealth > 0) {

			//Check ground/wall status
			//Following: UNity 2d Character Controllers
			grounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
			walled = Physics2D.OverlapCircle(wallCheck.position, checkRadius,wallLayer);


			//Call moving
			if(!gameManager.getGameStatus()) Move(Input.GetAxis("Horizontal"), true, attacking);

			//Attacks with our player (Check for a level up here as well), only attack if not jumping
			if (Input.GetKey (KeyCode.Backspace) &&
				!gameManager.getGameStatus()) {

				//Now since we are allowing holding space to punch we gotta count for it
				if(!attacking && (holdAttack % holdDuration == 0))
				{
					//Set hold punch to zero
					holdAttack = 0;

					//Attacking working great
					StopCoroutine("Slash");
					StartCoroutine ("Slash");
				}

				//Increase hold punch
				holdAttack++;
			}

			//Check for attack Key Up
			if(Input.GetKeyUp(KeyCode.Backspace)) {

				//Set hold punch to zero
				holdAttack = 0;
			}

			//Jumping INput, cant jump if attacking
			if(Input.GetKeyDown(KeyCode.Space) && !attacking && 
				jumps < 2 &&
				!gameManager.getGameStatus()) {

				if (jump.isPlaying)
					jump.Stop ();
				jump.Play ();

				//Jump Coroutine
				StopCoroutine ("Jump");
				StartCoroutine ("Jump");
			}
		} 
		else {

			//We is ded

			//make our player object invisible
			//possible display some animation first
			//Renderer r = (Renderer) gameObject.GetComponent("SpriteRenderer");
			//r.enabled = false;
			//No longer turning invisible, just looping death animation
			//play our death animation
			animator.SetTrigger ("Death");

			//play the death sound
			//if (!death.isPlaying) {
			//death.Play ();
			//}

			//Set our gameover text
			gameManager.setGameStatus (true);

			//set health to 0
			curHealth = 0;
		}
	}

	//Function to move our Character
	public void Move (float inputDirection, bool playerLerp, bool inAction)
	{
		//Get our input
		float h = inputDirection;

		//Force some camera Lerp
		if(playerLerp) actionCamera.addLerp(h / 20, 0);

		//Also check to make sure we stay that direction when not moving, so check that we are
		if (h != 0) {

			//animate to the direction we are going to move
			//Find the greatest absolute value to get most promenint direction
			/*
			 *
			 * -1		1
			 *
			 * */

			if (h > 0) {
				direction = 1;
			} else {
				direction = -1;
			}

			//Create a vector to where we are moving
			Vector2 movement = new Vector2 (h, 0);

			//When attacking start a slow movemnt coroutine
			if (!inAction) {

				//Attacking working great
				StopCoroutine ("slowMoving");
				StartCoroutine ("slowMoving");
			}


			//Get our speed according to our current level
			float levelSpeed = moveSpeed;


			//Get our actual speed
			float superSpeed = moveSpeed / moveDec;

			//Can't go above .5 though
			if (superSpeed > (.032f * moveSpeed)) {
				superSpeed = (.032f * moveSpeed);
			}

			//Flip our sprite
			setFlip();

			//Move to that position
			animator.SetBool ("Running", true);
			charBody.MovePosition (charBody.position + movement * superSpeed);
		}

		//then we are not moving
		else {

			//Set our position to our current position, so we dont drift away
			charBody.MovePosition (charBody.position);

			//tell the animator we are no longer moving
			//direction = 0;

			animator.SetBool ("Running", false);
		}
	}

	//Function for attacking
	IEnumerator Slash() {

		//Set shooting to true
		attacking = true;

		animator.SetTrigger ("Attack");

		//Play the attack sound
		if(attack.isPlaying) attack.Stop();
		attack.Play ();

		//Knock Forward
		float knockMove = 0.0025f;
		//Get our directions
		if (direction == 1)
			gameObject.transform.position = new Vector3 (gameObject.transform.position.x + knockMove, gameObject.transform.position.y, 0);
		else if (direction == -1)
			gameObject.transform.position = new Vector3 (gameObject.transform.position.x - knockMove, gameObject.transform.position.y, 0);

		//Let the frame finish
		yield return new WaitForFixedUpdate();

		//set attacking to false
		attacking = false;
	}

	//Function for jumping
	IEnumerator Jump() {

		//Cancel the jump animation if jumping
		animator.SetBool ("Jump", false);
		yield return new WaitForFixedUpdate();

		//Increase our jumps
		jumps++;

		//Reset our y velocity
		charBody.velocity = new Vector2 (charBody.velocity.x, 0);

		//Increase our drag
		charBody.drag = 100000.0f;

		//Start jump animation
		animator.SetBool ("Jump", true);

		//Check if we need the walljump boolean
		bool wallForce = false;
		int wallDirection = direction;
		//if (wallJumping) wallForce = true;

		//Add the jump force
		//Needs to be intervals of 30, gives best accleration
		for(int i = 30; i > 0; i--) {

			float jumpY = 420.0f * i * jumpHeight * Time.deltaTime;

			//Walljumping force
			float jumpX = 0;
			if (wallForce) {

				if (wallDirection < 0) jumpX = 400f * i * jumpHeight * Time.deltaTime;
				else jumpX = 25f * i * jumpHeight * Time.deltaTime * -1;
			}

			//Add the Force
			charBody.AddForce( new Vector2(jumpX, jumpY));

			//Force some camera Lerp
			actionCamera.addLerp(0, i / -380.0f);


			//Wait some frames
			//Wait a frame
			yield return new WaitForFixedUpdate();
		}
	}

	//Function to check if we can jump again for collisions
	void OnCollisionEnter2D(Collision2D collision)
	{

		//Check if it is tthe jumping wall
		if (collision.gameObject.tag == "JumpWall") {

			//Reset Jumps
			jumps = 1;
		}

		//Check if it is the floor
		if (collision.gameObject.tag == "Floor" ||
			collision.gameObject.tag == "EnemyChar" ||
			collision.gameObject.tag == "BossChar") {
			
			//Set Jumps to zero
			StopCoroutine ("Jump");
			jumps = 0;
			animator.SetBool ("Jump", false);
			//Reset our drag
			charBody.drag = 0.0f;
			actionCamera.impactPause();
			actionCamera.startShake ();
		}

		//Check if it is spikes
		if(collision.gameObject.tag == "SpikeWall") {
			//Kill the players
			setHealth(0);
		}
	}

	//Function to check if we can jump again for collisions
	void OnCollisionExit2D(Collision2D collision)
	{
		//Check if it is tthe jumping wall
		if (collision.gameObject.tag == "JumpWall") {

			//Stop wall jumping
			//wallJumping = false;
		}
	}

	//Function to check if we can jump again for collisions
	void OnCollisionStay2D(Collision2D collision)
	{

		//Check if it is tthe jumping wall
		if (collision.gameObject.tag == "JumpWall") {

			//set wallJumping
			//wallJumping = true;
		}

		//Check if it is spikes
		if (collision.gameObject.tag == "SpikeWall") {
			//Kill the players
			setHealth (0);
		}

		//Check if it is the floor
		if (collision.gameObject.tag == "Floor" ||
			collision.gameObject.tag == "EnemyChar" ||
			collision.gameObject.tag == "BossChar") {

			//Set Jumps to zero
			jumps = 0;
			animator.SetBool ("Jump", false);
			//Reset our drag
			charBody.drag = 0.0f;
			//actionCamera.impactPause();
			//actionCamera.startShake ();
		}

		//Check if it is an enemy
		if (collision.gameObject.tag == "EnemyChar" ||
			collision.gameObject.tag == "BossChar") {

			if (attacking) {

				//Do some damage
				//Check if the enemy is in the direction we are facing
				//Get our x and y
				float playerX = gameObject.transform.position.x;
				float playerY = gameObject.transform.position.y;
				float enemyX = collision.gameObject.transform.position.x;
				float enemyY = collision.gameObject.transform.position.y;

				//Our window for our Attack range (Fixes standing still no attack bug)
				float window = .05f;

				//Deal damage if we are facing the right direction
				if((direction == 1 && (enemyX + window) >= playerX) ||
					(direction == -1 && (enemyX - window) <= playerX))
				{
					
					//Get the enemy and decrease it's health
					EnemyControl e = (EnemyControl) collision.gameObject.GetComponent("EnemyControl");

					//Do damage
					int newHealth = e.getHealth() - playerDamage;
					e.setHealth(newHealth);

					//Shake the screen
					actionCamera.startShake();

					//Add slight impact pause
					actionCamera.startImpact();

					//Play the attack sound
					if(attackHit.isPlaying) attackHit.Stop();
					attackHit.Play ();
				}

			}
		}
	}
}