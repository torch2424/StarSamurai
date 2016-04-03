using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleScreen : MonoBehaviour {

	public UnityEngine.UI.Text startText;
	public AudioSource select;

	// Use this for initialization
	void Start () {
		StartCoroutine ("flashText");
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.Return)) {
			StartCoroutine ("startScene");
		}

		if (!startText.enabled) {
			StartCoroutine ("flashText");
		}
	}

	public IEnumerator flashText() {
		float rate = 0.5f;
		yield return new WaitForSeconds(rate / 2);
		startText.enabled = true;
		yield return new WaitForSeconds(rate);
		startText.enabled = false;
	}

	//Function to reset the scene
	public IEnumerator startScene() {

		//Play Selectect
		select.Play();

		//wait a tiny bit
		for(int i = 5; i > 0; i--) {
			yield return new WaitForFixedUpdate();
		}

		//Load the scene
		SceneManager.LoadScene ("GameMain");
	}
}
