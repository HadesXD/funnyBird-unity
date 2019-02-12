using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDownText : MonoBehaviour {

	public delegate void CountdownFinished(); //lahko ustvarimo evente.
	public static event CountdownFinished OnCountdownFinished; //ko konča s odštevanjem se začne igra.
	
	Text countdown;
    
	void OnEnable()
	{
		countdown = GetComponent<Text>(); //dobi komponento iz objekta.
		countdown.text = "3"; //default value.
		StartCoroutine("CountdownPlay"); 
	}

	IEnumerator CountdownPlay() { //vsako sekundo spremeni text, za manjšo vrednost
		int count = 3;
		for (int i = 0; i < count; i++){ 
			countdown.text = (count - i).ToString();
			yield return new WaitForSeconds(1); //počaka za 1 sekundo.
		}

		OnCountdownFinished(); //event gre na GameManager.cs
	}
}
