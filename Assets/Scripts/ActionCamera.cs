using UnityEngine;
using System;
using System.Collections;

public class ActionCamera : MonoBehaviour {

	//The speed the camera will lerp, e.g. 1.5f
	[Tooltip("A Float value that will determine how fast the camera will lerp, and follow. 0.1 < lerpSpeed < 3.0")]
	public float lerpSpeed = 1.0f;

	//Shake amount for screenshake
	[Tooltip("A Float value that will determine the maximum amount of screenshake. 0.1 < shakeAmount < 3.0")]
	public float shakeAmount = 1.0f;
	[Tooltip("The Float value that determines the rate at which screenshake settles. 0.1 < shakeSettleSpeed < 3.0")]
	public float shakeSettleSpeed = 1.0f;

	//The bounds in which how far the camera can move
	[Tooltip("The Maxmimum Bounds at which the Camera can lerp. 0.0 < camera Bounds < 3.0")]
	public float cameraBounds = 1.5f;

	//Screenshake Booleans
	private float currentShake;
	private bool shaking;

	//Boolean if currently in an impact pause
	private bool impacting;

	//our postion
	private Vector3 defaultPos;

	//Our game manager
	StateManager gameManager;


	// Use this for initialization
	void Start () 
	{
		shaking = false;
		defaultPos = new Vector3 (0, 0, -10);
		impacting = false;
		shakeAmount = shakeAmount / 10.0f;
		shakeSettleSpeed = shakeSettleSpeed / 3.0f;

		//off set the camera by just a little bit to add some lerp when we start
		gameObject.transform.localPosition = 
			new Vector3 (.05f, .05f, -10);

		//Get our gammaneger
		gameManager = GameObject.Find("StateManager").GetComponent<StateManager>();
	}

	// Update is called once per frame
	void Update () 
	{
		//translate back to ourself, 0,0, -10, with lerp
		//Multiply the lerp to inscrease it's effect
		Vector3 velocity = Vector3.zero;
		Vector3 cameraLerp = Vector3.Lerp(gameObject.transform.localPosition, defaultPos, lerpSpeed * Time.deltaTime);
		gameObject.transform.localPosition = cameraLerp;

		//Make Sure we are within our bounds
		Vector3 boundsPos = new Vector3 (Mathf.Clamp (gameObject.transform.localPosition.x, cameraBounds * -1, cameraBounds), 
			Mathf.Clamp (gameObject.transform.localPosition.y, cameraBounds * -1, cameraBounds), 
			defaultPos.z);

		gameObject.transform.localPosition = boundsPos;

		//Now check if we need to shake the camera
		if(shaking)
		{
			if(currentShake > 0)
			{

				float xShake = UnityEngine.Random.insideUnitCircle.x;
				float yShake = UnityEngine.Random.insideUnitCircle.y;

				//If we still have some shake value, make the current camera position that much more amount
				//Also need to lerp our screen shake, and because of this make sure the camera cant go out of a certain distnace
				if(Vector3.Distance(defaultPos, gameObject.transform.localPosition) > 1.25f)
				{
					//Mve away from player
					gameObject.transform.localPosition =  gameObject.transform.localPosition + 
						new Vector3(xShake * currentShake, yShake * currentShake, 0);
				}
				else
				{
					//Move towards player
					gameObject.transform.localPosition =  gameObject.transform.localPosition - 
						new Vector3(xShake * currentShake, yShake * currentShake, 0);
				}


				currentShake = currentShake - Time.fixedDeltaTime * shakeSettleSpeed;
			}
			else
			{
				shaking = false;
				currentShake = shakeAmount;
			}
		}
	}

	//Function tto start shaking
	public void startShake()
	{
		//First check if the game is still going, gamestatus returns gameover
		if(!gameManager.getGameStatus())
		{
			//reset current shake and shake!
			currentShake = shakeAmount;
			shaking = true;
		}
	}

	//Function for impact pause
	//Function to call for impact pause
	public void startImpact()
	{
		StartCoroutine("impactPause");
	}

	//Pause our game for some slight seconds
	public IEnumerator impactPause()
	{
		if(!impacting)
		{
			impacting = true;
			Time.timeScale = 0.1f;
			yield return new WaitForSeconds(.0015f);
			Time.timeScale = 1;
			impacting = false;
		}
	}

	//Function to push our camera in a direction
	public void addLerp(float x, float y) {

		//The max we can throw the camera
		float forceMax = lerpSpeed / 10;

		//First check for our bounds for input
		x = Mathf.Clamp(x, forceMax * -1, forceMax);
		y = Mathf.Clamp(y, forceMax * -1, forceMax);

		//Create our Vector
		Vector3 newPos = new Vector3 (Mathf.Clamp (gameObject.transform.localPosition.x + x, cameraBounds * -1, cameraBounds), 
			                 Mathf.Clamp (gameObject.transform.localPosition.y + y, cameraBounds * -1, cameraBounds), 
			                 defaultPos.z);

		Vector3 cameraLerp = Vector3.Lerp(newPos, gameObject.transform.localPosition, 0.95f);

		//Set our value
		gameObject.transform.localPosition = newPos;
	}
}