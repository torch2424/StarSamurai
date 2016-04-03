using UnityEngine;
using System;
using System.Collections;

public class StateManager : MonoBehaviour {

	//Boolean to determine if the game is over
	private bool gameOver;

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

	//Our score
	private int score;


	//Our background music
	public AudioSource bgFight;
	//Our background music
	public AudioSource deathSound;
	private bool deathPlayed;

	//Array pf things to say once you die
	String[] epitaph = {"See you space samurai..."};

	// Use this for initialization
	void Start () {

		//Scale our camera accordingly
		gameOver = false;

		//Set our time to normal speed
		Time.timeScale = 1;

		//Get our player
		user = GameObject.Find ("Player").GetComponent<PlayerControl>();

		//Get our Hud
		hud = GameObject.Find ("PlayerHUD").GetComponent<UnityEngine.UI.Text> ();

		//get our bg music
		//bgFight = GameObject.Find ("BgSong").GetComponent<AudioSource> ();
		//deathSound = GameObject.Find ("Death").GetComponent<AudioSource> ();
		deathPlayed = false;

		//Defeated enemies is one for score calculation at start
		defeatedEnemies = 0;
		//Total spawned enemies is one because we check for it to spawn enemies, and zero would get it stuck
		totalSpawnedEnemies = 0;

		//Set score to zero
		score = 0;

		//Show our score and things
		hud.text = ("Enemies Defeated: " + defeatedEnemies + "\nHighest Score: " + score);

		//Spawn an enemies
		totalFrames = 100;
		updateFrames = totalFrames;
	}

	// Update is called once per frame
	void Update () {

		//Check if we need to restart the game
		if(Input.GetKey(KeyCode.Return)) {
			Application.LoadLevel ("GameScene");
		}

		//Spawn enemies every frame
		if (!gameOver) {

			//Get the score for the player
			//Going to calculate by enemies defeated, level, and minutes passed
			score = (int) (defeatedEnemies * 100) + defeatedEnemies;


			//Update our hud to player
			//hud.text = ("Health: " + user.getHealth () + "\tScore: " + score);

			//start the music! if it is not playing
			//if(!bgFight.isPlaying)
			//{
				//bgFight.Play();
				//bgFight.loop = true;
			//}
		}
		else
		{
			//First get our epitaph index
			int epitaphIndex = 0;
			if(score / 1000 <= epitaph.Length - 1)
			{
				epitaphIndex = score / 1000;
			}
			else
			{
				epitaphIndex = epitaph.Length - 1;
			}


			//Show our game over
			hud.text = ("GAMEOVER!!!" + "\n" + epitaph[epitaphIndex] + "\nEnemies Defeated:" + defeatedEnemies
				+ "\nScore:" + score +"\nPress Start/Enter to restart...");

			//stop the music! if it is playing
			if(bgFight.isPlaying)
			{
				bgFight.Stop();
			}

			if (!deathPlayed) {
				//Play the Death Sounds
				deathSound.Play();
				deathPlayed = true;
			}

			//Slow down the game Time
			Time.timeScale = 0.45f;
		}


		//Spawn an enemy every 500 frames
		if(updateFrames == 0) {

			//Spawn an enemy 
			if(numEnemies < maxEnemies) {

				//Spawn the enemy
				StartCoroutine("spawnEnemy");

				if (totalFrames - spawnRateFrames > 0) {

					totalFrames = (int)(totalFrames - spawnRateFrames);
					updateFrames = totalFrames;
				} else
					updateFrames = 0;
			}
		}
		else updateFrames--;

	}

	//Function to spawn and enemy
	IEnumerator spawnEnemy() {
		
		Vector2 spawnPos = Vector2.zero;

		spawnPos.x = 1.0f;


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
		score = (int) Math.Floor(defeatedEnemies + (1000 * Math.Abs(UnityEngine.Random.insideUnitCircle.x)));

		//Show our score and things
		hud.text = ("Enemies Defeated: " + defeatedEnemies + "\nHighest Score: " + score);
	}
}