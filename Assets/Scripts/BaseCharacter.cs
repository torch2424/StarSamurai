using UnityEngine;
using System.Collections;

public class BaseCharacter : MonoBehaviour {

	/*
	 * PUBLIC INSPECTOR VARIABLES
	*/

	//Our game manager
	[Tooltip("The Game Object that manages the game state")]
	public StateManager gameManager;

	//Our player movepseed
	[Tooltip("The Speed of which the player will move")]
	public float moveSpeed;

	//Our Player Health
	[Tooltip("The Maximum Health Of the player")]
	public int maxHealth;

	//Player health regen rate
	[Tooltip("The rate of health regeneration, suggested to be 1/20 of maxHealth")]
	public int healthRegenRate;

	//Boolean to reverseflip
	[Tooltip("Flip the direction of the player sprite. False == Right Facing Sprite, True == Left Facing Sprite")]
	public bool reverseFlip;

	/*
	 * PRIVATE VARIABLES
	*/

	//Our player sprite
	protected Rigidbody2D charBody;

	//Our player stats
	protected int curHealth;

	//If A Player can regenerate
	private bool canRegen;

	//Game jucin', slow when attacking
	private int moveDec;

	//Our sprite renderer to change our sprite color
	protected SpriteRenderer render;
	//Used to store a reference to the Player's animator component.
	protected Animator animator;

	//our camera Script
	protected ActionCamera actionCamera;

	//Our current Direction
	protected int direction;



	// Use this for initialization
	protected virtual void Start () {

		//Get a component reference to the Character's animator component
		animator = GetComponent<Animator>();
		render = GetComponent<SpriteRenderer>();

		//Get the rigid body on the prefab
		charBody = GetComponent<Rigidbody2D>();

		//Set our default values
		moveDec = 1;

		//Set our health
		curHealth = maxHealth;
		canRegen = true;

		//Default looking idle/right
		direction = 1;

		//Get our gamemaneger
		gameManager = GameObject.Find("StateManager").GetComponent<StateManager>();

		//Get our camera script
		actionCamera = Camera.main.GetComponent<ActionCamera>();
	}

	// Update is called once per frame
	protected virtual void Update () {

		//Check if we can Regen
		if(canRegen && !gameManager.getGameStatus()) {

			//increase health by regen Rate
			if (curHealth + healthRegenRate > maxHealth) {
				setHealth(maxHealth);
			} else {
				setHealth(curHealth + healthRegenRate);
			}

			//Stop regen
			StopCoroutine("noRegen");
			StartCoroutine("noRegen");

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

	//Function to slow movement for a certain amount of time
	IEnumerator slowMoving()
	{
		//Increase Move Decrement
		moveDec = 5;
		yield return new WaitForSeconds(.5f);
		moveDec = 1;
	}

	//Function to notHealth Regen for a certain amount of time
	IEnumerator noRegen()
	{
		//Boolean for health
		canRegen = false;
		yield return new WaitForSeconds(0.45f);
		canRegen = true;
	}

	//Get/set funtion for health
	public int getHealth()
	{
		return curHealth;
	}

	//Get/set funtion for health
	public void setHealth(int newHealth)
	{
		curHealth = newHealth;
		if (curHealth > 0)
		{
			//Set the character damage indicator
			editDamage();

			//Stop regen
			StopCoroutine("noRegen");
			StartCoroutine("noRegen");
		}
	}

	//Function to indicate health
	public void editDamage()
	{
		//Create our red color indicator
		float curFloat = curHealth * 1.0f;
		float maxFloat = maxHealth * 1.0f;
		float healthPercent = curFloat / maxFloat;
		Color damage = new Color (1.0f, healthPercent, healthPercent);
		render.material.color = damage;

	}

	//Function to return our current direction
	public int getDirection() {
		return direction;
	}

	//function to flip the sprite
	public void setFlip() {

		//Get the direction
		if (direction > 0) {

			//Right
			if(reverseFlip) render.flipX = true;
			else render.flipX = false;

		} else if(direction < 0) {

			//Left
			if(reverseFlip) render.flipX = false;
			else render.flipX = true;
		}
	}
}
