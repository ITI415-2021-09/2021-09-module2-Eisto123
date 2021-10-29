using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class Clock : MonoBehaviour {

	static public Clock 	S;

	[Header("Set in Inspector")]
	public TextAsset			deckXML;
	public TextAsset layoutXML;
	public float xOffset = 3;
	public float yOffset = -2.5f;
	public Vector3 layoutCenter;


	[Header("Set Dynamically")]
	public cDeck					deck;
	public cLayout layout;
	public List<Card> cards;
	public Transform layoutAnchor;

	void Awake(){
		S = this;
	}

	void Start() {
		deck = GetComponent<cDeck> ();
		deck.InitDeck (deckXML.text);
		cDeck.Shuffle(ref deck.cards);

		layout = GetComponent<cLayout>();
		layout.ReadLayout(layoutXML.text);
		cards = deck.cards;
		LayoutGame();
	}
	 
	void LayoutGame()
	{

		if (layoutAnchor == null)
		{
			GameObject tGO = new GameObject("_LayoutAnchor");
			layoutAnchor = tGO.transform;
			layoutAnchor.transform.position = layoutCenter;
		}
		UpdateDrawPile();
	}
	void UpdateDrawPile()
	{
		Card cd;

		for (int i = 0; i < cards.Count; i++)
		{
			cd = cards[i];
			cd.transform.parent = layoutAnchor;

			Vector2 dpStagger = layout.drawPile.stagger;
			cd.transform.localPosition = new Vector3(
				layout.multiplier.x * (layout.drawPile.x + i * dpStagger.x),
				layout.multiplier.y * (layout.drawPile.y + i * dpStagger.y),
				-layout.drawPile.layerID + 0.1f * i);
			cd.faceUp = false;
		}
	}
}