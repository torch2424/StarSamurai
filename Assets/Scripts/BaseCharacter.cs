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
	protected int moveDec;

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
		if (newHealth < curHealth) {
		
			//Give some knockBack
			charBody.AddForce(new Vector2(225 * direction * -1, 0));
		}

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
		//Now set the color
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
