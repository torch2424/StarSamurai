using UnityEngine;
using System;
using System.Collections;

public class CameraFeel : MonoBehaviour {

	//Shake amount for screenshake
	public float shakeAmount;
	private float currentShake;
	public float decreaseAmount;
	public float totalShake;
	private bool shaking;

	//The speed the camera will lerp, e.g. 1.5f
	[Tooltip("A Float value that will determine how fast the camera will lerp, and follow. 0.0 < lerpSpeed < 3.0")]
	public float lerpSpeed;


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

		//off set the camera by just a little bit to add some lerp when we start
		gameObject.transform.localPosition = new Vector3 (.05f, .05f, -10);

		//Get our gammaneger
		gameManager = GameObject.Find("StateManager").GetComponent<StateManager>();
	}

	// Update is called once per frame
	void Update () 
	{
		//translate back to ourself, 0,0, -10, with lerp
		//Multiply the lerp to inscrease it's effect
		Vector3 velocity = Vector3.zero;
		Vector3 cameraLerp = Vector3.SmoothDamp(gameObject.transform.localPosition, defaultPos, ref velocity, 0.5f);
		gameObject.transform.localPosition = cameraLerp;

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


				currentShake = currentShake - Time.deltaTime * decreaseAmount;
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

	//Function to increase screen shake
	public void incShake(float amount)
	{
		if(shakeAmount + amount < totalShake)
		{
			shakeAmount = shakeAmount + amount;
		}
	}

	//function to decrease lerp speed
	public void declerp(float amount)
	{
		if(lerpSpeed - amount > .1)
		{
			lerpSpeed = lerpSpeed - amount;
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
			yield return new WaitForSeconds(.002f);
			Time.timeScale = 1;
			impacting = false;
		}
	}

	//Function to push our camera in a direction
	public void forceLerp(float x, float y) {

		float forceMax = 0.35f;

		//Simply foce the camera in that direction
		if(gameObject.transform.localPosition.x + x < defaultPos.x + forceMax &&
			gameObject.transform.localPosition.x + x > defaultPos.x - forceMax) {
			gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x + x, gameObject.transform.localPosition.y, defaultPos.z);
		}

		if (gameObject.transform.localPosition.y + y < defaultPos.y + forceMax &&
			gameObject.transform.localPosition.y + y > defaultPos.y - forceMax) {
			gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x , gameObject.transform.localPosition.y + y, defaultPos.z);
		}
	}
}