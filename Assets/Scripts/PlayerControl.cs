using UnityEngine;
using System.Collections;

public class PlayerControl : BaseCharacter {

	//Our sounds
	private AudioSource jump;

	//Boolean to check if attacking
	bool attacking;
	//Our Number of jumps we have done
	int jumps;
	bool jumping;
	public float jumpHeight;

	//Counter for holding space to punch
	private int holdAttack;
	//How long do they have to hold before attacking
	public int holdDuration;

	// Use this for initialization
	protected override void Start ()
	{

		//Call our superclass start
		base.Start();

		//Get our sounds
		//jump = GameObject.Find ("Jump").GetComponent<AudioSource> ();

		//Set our actions
		attacking = false;
		jumps = 0;
		jumping = false;
		holdAttack = 0;
	}

	// Update is called once per frame
	protected override void Update ()
	{
		//Call our base update
		base.Update ();

		//check if dead, allow movement if alive
		if (curHealth <= 0) {
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
		} else {

			//Call moving
			if(!gameManager.getGameStatus()) base.Move(Input.GetAxis("Horizontal"), attacking);

			//Attacks with our player (Check for a level up here as well), only attack if not jumping
			if (Input.GetKey (KeyCode.Backspace) &&
				!jumping &&
				!gameManager.getGameStatus()) {
				//Now since we are allowing holding space to punch we gotta count for it
				if(!attacking && holdAttack % holdDuration == 0)
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

			//Jumping INput, cant jump if attacking
			if(Input.GetKeyDown(KeyCode.Space) && !attacking && 
				jumps < 2 &&
				!gameManager.getGameStatus()) {

				//jump.Play ();

				//Jump Coroutine
				StopCoroutine ("Jump");
				StartCoroutine ("Jump");
			}

		}
	}

	//Function for shooting
	IEnumerator Slash() {

		//Set shooting to true
		attacking = true;


		//Let the frame finish
		yield return null;
		//set attacking to false
		attacking = false;
	}

	//Function for jumping
	IEnumerator Jump() {

		//Cancel the jump animation if jumping
		animator.SetBool ("Jump", false);
		yield return new WaitForFixedUpdate();

		//Set our booleans
		jumping = true;
		jumps++;

		//Reset our y velocity
		charBody.velocity = new Vector2 (charBody.velocity.x, 0);

		//Start jump animation
		animator.SetBool ("Jump", true);

		//Force some camera Lerp
		actionCamera.forceLerp(0, -0.0065f);

		//Add the jump force
		for(int i = 45; i > -10; i--) {

			charBody.AddForce( new Vector2(0, 100f * i * jumpHeight * Time.deltaTime));

			//Wait some frames
			//Wait a frame
			yield return new WaitForFixedUpdate();
		}
	}

	//Function for if jumping
	public bool isJumping()
	{
		return jumping;
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
		if (collision.gameObject.tag == "Floor") {
			
			//Set Jumps to zero
			jumps = 0;
			jumping = false;
			animator.SetBool ("Jump", false);
			actionCamera.impactPause();
			//actionCamera.startShake ();
		}

		//Check if it is spikes
		if(collision.gameObject.tag == "SpikeWall") {
			//Kill the players
			setHealth(0);
		}
	}

	//Function to check if we can jump again for collisions
	void OnCollisionStay2D(Collision2D collision)
	{

		//Check if it is the floor
		if (collision.gameObject.tag == "Floor") {
			//Set Jumps to zero
			jumps = 0;
			jumping = false;
			animator.SetBool ("Jump", false);
		}

		//Check if it is spikes
		if(collision.gameObject.tag == "SpikeWall") {
			//Kill the players
			setHealth(0);
		}
	}
}