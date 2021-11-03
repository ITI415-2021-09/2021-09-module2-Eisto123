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
	public List<CardClock> drawPile;
	public List<CardClock> Pile;
	public List<List<CardClock>> clock = new List<List<CardClock>>();
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
		Pile = ConvertListCardsToListCardClock(deck.cards);
		drawPile = Pile;
		LayoutGame();
	}

	List<CardClock> ConvertListCardsToListCardClock(List<Card> lCD)
	{
		List<CardClock> lCP = new List<CardClock>();
		CardClock tCP;
		foreach (Card tCD in lCD)
		{
			tCP = tCD as CardClock;
			lCP.Add(tCP);
		}
		return (lCP);
	}
	CardClock Draw()
	{
		CardClock cd = drawPile[0];
		drawPile.RemoveAt(0);
		return (cd);
	}
	void LayoutGame()
	{
		if (layoutAnchor == null)
		{
			GameObject tGO = new GameObject("_LayoutAnchor");
			layoutAnchor = tGO.transform;
			layoutAnchor.transform.position = layoutCenter;
		}
		CardClock card;
		foreach (cSlotDef tSD in layout.slotDefs)
        {
			List<CardClock> clockPile = new List<CardClock>();
			for (int i = 0; i<4; i++)
            {
				card = Draw();
				card.layoutID = tSD.id;
				card.slotDef = tSD;
				card.state = cCardState.tableau;
				clockPile.Add(card);
			}

			for (int i = 0; i < clockPile.Count; i++)
			{
				CardClock cc = clockPile[i];
			

				cc.transform.parent = layoutAnchor;

				Vector2 dpStagger = layout.slotDefs[tSD.layerID].stagger;
				cc.transform.localPosition = new Vector3(
					layout.multiplier.x * (layout.slotDefs[tSD.layerID].x + i * dpStagger.x),
					layout.multiplier.y * (layout.slotDefs[tSD.layerID].y + i * dpStagger.y),
					-layout.slotDefs[tSD.layerID].layerID + 0.1f * i);
				cc.faceUp = tSD.faceUp;
			}
			clock.Add(clockPile);
			
		}
		CardClock iniCard = clock[12][0];
		iniCard.faceUp = true;
		iniCard.state = cCardState.target;
		iniCard.SetSortingLayerName("layer1");
		UpdateDrawPile();
	}
	void UpdateDrawPile()
	{
		CardClock cd;

		for (int i = 0; i < drawPile.Count; i++)
		{
			cd = drawPile[i];
			cd.transform.parent = layoutAnchor;

			Vector2 dpStagger = layout.drawPile.stagger;
			cd.transform.localPosition = new Vector3(
				layout.multiplier.x * (layout.drawPile.x + i * dpStagger.x),
				layout.multiplier.y * (layout.drawPile.y + i * dpStagger.y),
				-layout.drawPile.layerID + 0.1f * i);
			cd.faceUp = false;
		}
	}
	public void CardClicked(CardClock cd)
	{
		switch (cd.state)
		{
			case cCardState.target:
				break;
			case cCardState.tableau:
				break;
		}
	}
}