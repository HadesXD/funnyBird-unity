using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapController : MonoBehaviour {

	public delegate void PlayerDeleagate(); //lahko ustvarimo evente.
	public static event PlayerDeleagate OnPlayerDied; //event ko player umre.
	public static event PlayerDeleagate OnPlayerScored; //event ko player dobi točko.

	public float tapForce = 200; //moč, ki ma naš dotik.
	public Vector3 startPosition; //začetni položaj ptice.
	
	GameManager game; //hiter dostop do GameManagerja.
	Rigidbody2D rigidbody;

	public AudioSource flyAudio;
	public AudioSource scoreAudio;
	public AudioSource deathAudio;

	void Start() {
		rigidbody = GetComponent<Rigidbody2D>(); //dobi komponento iz objekta.
		game = GameManager.Instance;
		rigidbody.simulated = false; //na začetku mu onemogočimo premikanje.
	}

	void OnEnable() {  //naročimo evente iz drugih cs scriptov (GameManager.cs).
		GameManager.OnGameStarted += OnGameStarted;
		GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
	}

	void OnDisable() {  //odjavimo evente iz drugih cs scriptov (GameManager.cs).
		GameManager.OnGameStarted -= OnGameStarted;
		GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
	}

	public void OnGameStarted() { //event smo dobili iz GameManagerja.cs, in se igra začne.
		rigidbody.velocity = Vector3.zero; //resetira gravitacijo.
		rigidbody.simulated = true; //da se ptica premika.
	}

	void Update() {
		if(game.GameOver) return;
		if (Input.GetMouseButtonDown(0)) { //0 predstavlja levi click na miški, 1 pa desni click.		
			flyAudio.Play(); //zvok.	
			rigidbody.velocity = Vector3.zero; //gravitacija se resetira.
			rigidbody.AddForce(Vector2.up * tapForce, ForceMode2D.Force); //Vector2.up predstavlja y axis.
		}
	}

	public void OnTriggerEnter2D(Collider2D hit) { //išče predmete na katerem imajo oznake 'GameOverZone' ali 'ScoreZone'.
		if (hit.gameObject.tag == "ScoreZone"){ //za dostop do oznake.
			OnPlayerScored(); //event je bil poslan k GameManagerju.cs
			scoreAudio.Play(); //zvok
		}

		if (hit.gameObject.tag == "GameOverZone"){ //za dostop do oznake.
			rigidbody.simulated = false; //player se nremore premikat.
			OnPlayerDied(); //event je bil poslan k GameManagerju.cs
			deathAudio.Play(); //zvok
		}
	}

	public void OnGameOverConfirmed() { //event smo dobili iz GameManagerja.cs.
		transform.position = startPosition; //resetria, položaj na default.
	}
}
