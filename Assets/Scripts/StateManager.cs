using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class StateManager : MonoBehaviour {

	//Boolean to determine if the game is over
	private bool gameOver;
	private bool gameWin;
	private bool creditsShow;

	//Our previous time to be stored for the spawn rate
	private float previousTime;

	//Our player object
	private PlayerControl user;

	//Our number of frames on startup
	//Rate (Frames) at which monsters should spawn
	public float spawnRateFrames;
	private int totalFrames;
	private int updateFrames;

	//Our enemy prefab
	public GameObject[] enemies;
	public GameObject[] bosses;
	private int numEnemies;
	//Suggest max enemies 50
	public int maxEnemies;
	//number of enemies spawned
	private int defeatedEnemies;
	//Total number of enemies spawned
	private int totalSpawnedEnemies;

	//Our Maps
	//public Sprite[] maps;
	//private SpriteRenderer gameMap;

	//Our Hud
	private UnityEngine.UI.Text hud;
	private UnityEngine.UI.Image healthBar;
	public UnityEngine.UI.Text gameOverText;

	//Our credits
	private Canvas credits;

	//Our score
	private int score;

	//Our slowmom
	private int currentSlowmo;
	private int maxSlowmo;
	private float slowmoRate;


	//Our background music
	public AudioSource bgFight;
	//Our background music
	public AudioSource deathSound;
	private bool deathPlayed;

	//Our select sound
	public AudioSource select;

	// Use this for initialization
	void Start () {

		//Scale our camera accordingly
		gameOver = false;
		gameWin = false;
		creditsShow = false;

		//Set our time to normal speed
		Time.timeScale = 1;

		//Get our player
		user = GameObject.Find ("Player").GetComponent<PlayerControl>();

		//Get our Hud
		hud = GameObject.FindWithTag("Health Text").GetComponent<UnityEngine.UI.Text> ();
		healthBar = GameObject.FindWithTag ("Health Bar").GetComponent<UnityEngine.UI.Image> ();

		//Get our Hud
		credits = GameObject.FindGameObjectWithTag ("Credits").GetComponent<Canvas>();
		credits.enabled = false;

		//get our bg music
		bgFight = GameObject.Find ("BgSong").GetComponent<AudioSource> ();
		//deathSound = GameObject.Find ("Death").GetComponent<AudioSource> ();
		deathPlayed = false;

		//Defeated enemies is one for score calculation at start
		defeatedEnemies = 0;
		//Total spawned enemies is one because we check for it to spawn enemies, and zero would get it stuck
		totalSpawnedEnemies = 0;

		//Set score to zero
		score = 0;

		//Show our score and things
		hud.text = ("Health: " + user.getHealth());

		//Spawn an enemies
		totalFrames = 500;
		updateFrames = totalFrames;

		//All of our slow mo stats
		maxSlowmo = 60;
		currentSlowmo = maxSlowmo;
		slowmoRate = 0.025f;

		//Hide Our gameover text
		gameOverText.enabled = false;
	}

	// Update is called once per frame
	void Update () {

		//Check if we need to restart the game
		if(Input.GetAxis("Submit") != 0) {

			StartCoroutine ("resetScene");
		}

		//Check if we need to quite the Game
		if(Input.GetAxis("Cancel") != 0) Application.Quit();

		if (gameWin) {

			//Finalize our hud
			hud.text = ("Health: " + user.getHealth());
			healthBar.fillAmount = (user.getHealth () / 100.0f);

			//Slow down the game Time
			Time.timeScale = 0.275f;

			//Fade in the credits
			if (!creditsShow) {
				creditsShow = true;
				StartCoroutine ("creditsFade");
			}
		}
		else if(gameOver)
		{
			
			//Show our game over
			hud.text = "Health: 0";
			healthBar.fillAmount = 0.0f;

			//stop the music! if it is playing
			if(bgFight.isPlaying)
			{
				bgFight.Stop();
			}

			if (!deathPlayed) {
				//Play the Death Sounds
				//deathSound.Play();
				deathPlayed = true;
			}

			//Slow down the game Time
			Time.timeScale = 0.45f;

			//Show the Game Over Text
			gameOverText.enabled = true;
		}
		else {

			//Do normal stuff

			//Get the score for the player
			//Going to calculate by enemies defeated, level, and minutes passed
			score = (int)(defeatedEnemies * 100) + defeatedEnemies;

			//Show our score and things
			hud.text = ("Health: " + user.getHealth());
			healthBar.fillAmount = (user.getHealth () / 100.0f);

			//start the music! if it is not playing
			if (!bgFight.isPlaying) {
				bgFight.Play ();
				bgFight.loop = true;
			}
				

			//Do some slowmo
			//Attacks with our player (Check for a level up here as well), only attack if not jumping
			if (Input.GetKeyDown (KeyCode.P) &&
				!gameOver && 
				Time.timeScale >= 1.0f) {

				//Now since we are allowing holding space to punch we gotta count for it
				if (currentSlowmo >= maxSlowmo) {

					//Time
					Time.timeScale = 0.25f;

					currentSlowmo = 0;
				}

			} else if (currentSlowmo < maxSlowmo) {

				currentSlowmo++;

				//Time
				if (Time.timeScale < 1.0f)
					Time.timeScale = Time.timeScale + slowmoRate;
				else
					Time.timeScale = 1.0f;
			}


			//Spawn an enemy every 500 frames
			if(updateFrames == 0) {

				//Spawn an enemy 
				if(numEnemies < maxEnemies) {

					//Spawn the enemy
					//StartCoroutine("spawnEnemy");

					if (totalFrames - spawnRateFrames > 0) {

						totalFrames = (int)(totalFrames - spawnRateFrames);
						updateFrames = totalFrames;
					} else
						updateFrames = 0;
				}
			}
			else updateFrames--;
		}

	}

	//Function to spawn and enemy
	IEnumerator spawnEnemy() {
		
		Vector2 spawnPos = new Vector2 (user.transform.position.x + 2.0f, user.transform.position.y + 2.0f);


		//Spawn our enemy
		Instantiate(enemies[0], spawnPos, Quaternion.identity);

		//increase our number of enemies
		numEnemies++;

		yield return null;
	}



	//Function to set gameover boolean
	public void setGameStatus(bool status)
	{
		gameOver = status;
	}

	//Function to get gameover boolean
	public bool getGameStatus()
	{
		return gameOver;
	}

	//Function to increase/decrease num enemies
	public void plusEnemy()
	{
		++numEnemies;
		++totalSpawnedEnemies;
	}

	public void minusEnemy()
	{
		--numEnemies;

		//Since enemy is gone add to defeated enemies
		++defeatedEnemies;

		//Increase our score
		score = (int) Math.Floor(score + defeatedEnemies + Math.Abs(1000 * Math.Abs(UnityEngine.Random.insideUnitCircle.x)));

		//Show our score and things
		hud.text = ("Health: " + user.getHealth());
		healthBar.fillAmount = (user.getHealth () / 100);

		//Remove the block if we have defeated all the enemies
		if(defeatedEnemies >= 19) {

			//Destroy the block
			Destroy (GameObject.Find ("KeyBlock"));

			//As well asremove the text
			GameObject.Find("KeyText").GetComponent<UnityEngine.UI.Text>().enabled = false;

			//As well as play a special sound
		}
	}

	//Function to reset the scene
	public IEnumerator resetScene() {

		//Play Selectect
		select.Play();

		//wait a tiny bit
		for(int i = 5; i > 0; i--) {
			yield return new WaitForFixedUpdate();
		}

		//Load the scene
		SceneManager.LoadScene ("GameMain");
	}



	//Function to win the game
	public void defeatedFinalBoss() {
		gameWin = true;
	}

	//Function to fade in some credits
	public IEnumerator creditsFade() {

		//Pause and continue the music
		bgFight.Pause();

		//wait a tiny bit
		for(int i = 22; i > 0; i--) {
			yield return new WaitForFixedUpdate();
		}

		//Nestedloop for awesome ness
		for(int j = 17; j > 0; j--) {

			//Wait some frames
			for(int i = j / 3; i > 0; i--) {
				yield return new WaitForFixedUpdate();
			}

			//Disable the credits
			credits.enabled = false;
			bgFight.Pause ();

			//Wait some frames
			for(int i = j / 3; i > 0; i--) {
				yield return new WaitForFixedUpdate();
			}

			//Enable the credits
			credits.enabled = true;
			bgFight.Play ();
		}

		//Once More Super Fast Flashing
		for(int j = 3; j > 0; j--) {

			//Wait some frames
			yield return new WaitForFixedUpdate();

			//Disable the credits
			credits.enabled = false;
			bgFight.Pause ();

			//Wait some frames
			yield return new WaitForFixedUpdate();

			//Enable the credits
			credits.enabled = true;
			bgFight.Play ();
		}

	}
}