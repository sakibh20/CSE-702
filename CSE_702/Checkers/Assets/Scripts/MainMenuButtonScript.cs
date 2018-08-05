using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtonScript : MonoBehaviour {

	public GameObject playOptions;
	public GameObject rules;
	public GameObject info;
	// Use this for initialization
	void Start () {
		playOptions.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape)){
			Application.Quit ();
		}
	}

	public void play(){
		playOptions.SetActive (true);
	}

	public void twoPlayer(){
		PlayerPrefs.SetInt ("AI",0);
		SceneManager.LoadScene (1);
	}

	public void singlePlayer(){
		PlayerPrefs.SetInt ("AI",1);
		SceneManager.LoadScene (1);
	}

	public void Rules(){
		rules.SetActive (true);
	}

	public void Back(){
		playOptions.SetActive (false);
		rules.SetActive (false);
		info.SetActive (false);
	}

	public void Info(){
		info.SetActive (true);
	}
	public void Facebook(){
		Application.OpenURL("https://www.facebook.com/ArtYourDreams/");
	}
	public void Email(){
		Application.OpenURL("https://mail.google.com/mail/u/0/#inbox?compose=new");
	}

	public void Quit(){
			Application.Quit ();
	}
}
