using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public static GameManager Instance; //da imajo drugi skripti dostop do GameManager.cs.

	void Awake() {
		Instance = this;
	}

	public delegate void GameDelegate(); //lahko ustvarimo evente.
	public static event GameDelegate OnGameStarted; //event ko se igra začne.
	public static event GameDelegate OnGameOverConfirmed; //event ko je konec.

	public GameObject StartPage; //da bomo meli dostop do strani.
	public GameObject GameOverPage; //da bomo meli dostop do strani.
	public GameObject CountdownPage; //da bomo meli dostop do strani.
	public Text ScoreText; //da bomo meli dostop do strani.
	
	enum PageState { //da imamo dostop do strani.
		None,
		Start,
		GameOver,
		Countdown,
	}

	int score = 0;
	bool gameOver = true; //preveri če je konce igre. Na začetku je true zaradi Starta v TapController.cs.
	public bool GameOver { get { return gameOver; } } //da bojo imajo drugi skritpi dostop.
	public int Score { get { return score; } }


	void OnEnable() { //naročimo evente iz drugih cs scriptov (Tap Controler in CountDownText)
		CountDownText.OnCountdownFinished += OnCountdownFinished;
		TapController.OnPlayerDied += OnPlayerDied; 
		TapController.OnPlayerScored += OnPlayerScored;
	}

	void OnDisable() { //odjavimo evente iz drugih cs scriptov.
		CountDownText.OnCountdownFinished -= OnCountdownFinished;
		TapController.OnPlayerDied -= OnPlayerDied;
		TapController.OnPlayerScored -= OnPlayerScored;
	}

	void SetPageState(PageState state) { //aktivriamo in deaktiviramo strani.
		switch (state) {
			case PageState.None: //preusmeri na nobeno stran.
				StartPage.SetActive(false);
				GameOverPage.SetActive(false);
				CountdownPage.SetActive(false);
				break;
			case PageState.Start: //preusmeri na Start starn.
				StartPage.SetActive(true);
				GameOverPage.SetActive(false);
				CountdownPage.SetActive(false);
				break;
			case PageState.GameOver: //preusmeri na GameOver stran.
				StartPage.SetActive(false);
				GameOverPage.SetActive(true);
				CountdownPage.SetActive(false);
				break;
			case PageState.Countdown: //preusmeri na CountDown stran.
				StartPage.SetActive(false);
				GameOverPage.SetActive(false);
				CountdownPage.SetActive(true);
				break;
		}
	}

	public void StartGame() { //aktiviran, ko je pritisnjen gumb za play.
		SetPageState(PageState.Countdown);
	}

	public void OnCountdownFinished() { //event ki smo dobili iz CountDownText.cs
		SetPageState(PageState.None); 
		OnGameStarted(); //event pošljemo tapControllerju.cs
		score = 0;
		gameOver = false;
	}

	public void OnPlayerScored() { //event smo dobili iz TapControlejra.cs
		score++;
		ScoreText.text = score.ToString();
	}

	public void OnPlayerDied() { //event smo dobili iz TapControlejra.cs
		gameOver = true;
		int SavedScore = PlayerPrefs.GetInt("HighScoreText"); //posebna lokacija, za shranjevanje highscora.
		if (score > SavedScore) {
			PlayerPrefs.SetInt("HighScoreText", score); //nov highscore smo sranli.
		}	
		SetPageState(PageState.GameOver); //spremeni stran na'Game Over'.
	}

	public void ConfirmGameOver() { //aktiviran, ko je pritisnjen gumb za resetiranje.
		OnGameOverConfirmed(); //event pošljemo tapControllerju.cs
		ScoreText.text = "0";
		SetPageState(PageState.Start);
	}
}
