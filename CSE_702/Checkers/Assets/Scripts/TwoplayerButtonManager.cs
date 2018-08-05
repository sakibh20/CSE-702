using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TwoplayerButtonManager : MonoBehaviour {
	public GameObject Intro;
	// Use this for initialization
	void Start () {
		if(PlayerPrefs.GetInt("first")!=1){
			Intro.SetActive (true);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape)){
			Application.Quit ();
		}
	}

	public void Quit(){
		Application.Quit ();
	}

	public void playAgain(){
		SceneManager.LoadScene (1);
	}

	public void Home(){
		SceneManager.LoadScene (0);
	}
	public void OK(){
		Intro.SetActive (false);
		PlayerPrefs.SetInt ("first", 1);
	}
}
