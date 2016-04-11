//Taken/Modified from: https://unity3d.com/learn/tutorials/modules/beginner/live-training-archive/2d-scrolling-backgrounds?playlist=17093

using UnityEngine;
using System.Collections;

public class ScrollingBG : MonoBehaviour {

	[Tooltip("The Speed of Scrolling any value from 0 to 10")]
	public float scrollSpeed;

	//Teh Size of our BG Tiles
	[Tooltip("The Amount of offset used in the desired position")]
	public float spriteSize;
	private Vector3 tileSize;

	[Tooltip("True For Horizontal Scrolling, False for Vertical")]
	public bool horizontal;

	[Tooltip("Flip the direction of scrolling, e.g left-right -> right-left")]
	public bool flip;

	//The Start position of our background
	private Vector3 startPosition;

	// Use this for initialization
	void Start () {
		
		startPosition = transform.localPosition;

		//Set our Tile Size
		tileSize = new Vector3(spriteSize, spriteSize, 0);


	}
	
	// Update is called once per frame
	void Update () {

		//First grab our direction
		float newPosition;
		if(horizontal) newPosition = Mathf.Repeat(Time.time * scrollSpeed, tileSize.x);
		else newPosition = Mathf.Repeat(Time.time * scrollSpeed, tileSize.y);

		//Next, Grab our Scroll position vector
		Vector3 scrollPosition;
		if(horizontal) {
			if(flip) scrollPosition = new Vector3 (-1, 0, 0);
			else scrollPosition = new Vector3 (1, 0, 0);
		}
		else {
			if(flip) scrollPosition = new Vector3 (0, -1, 0);
			else scrollPosition = new Vector3 (0, 1, 0);
		}

		//Finally, set our position
		Vector3 velocity = Vector3.zero;
		transform.localPosition = startPosition + scrollPosition * newPosition;
	}
}
