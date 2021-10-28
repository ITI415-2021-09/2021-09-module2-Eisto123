using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class Clock : MonoBehaviour {

	static public Clock 	S;

	[Header("Set in Inspector")]
	public TextAsset			deckXML;


	[Header("Set Dynamically")]
	public cDeck					deck;

	void Awake(){
		S = this;
	}

	void Start() {
		deck = GetComponent<cDeck> ();
		deck.InitDeck (deckXML.text);
	}

}
