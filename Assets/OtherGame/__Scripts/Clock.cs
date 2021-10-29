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
	public List<Card> drawPile;
	public List<Card> clockPile;
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
		drawPile = deck.cards;
		LayoutGame();
	}

	List<CardProspector> ConvertListCardsToListCardProspectors(List<Card> lCD)
	{
		List<CardProspector> lCP = new List<CardProspector>();
		CardProspector tCP;
		foreach (Card tCD in lCD)
		{
			tCP = tCD as CardProspector;
			lCP.Add(tCP);
		}
		return (lCP);
	}
	Card Draw()
	{
		Card cd = drawPile[0];
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
		Card card;
		foreach (cSlotDef tSD in layout.slotDefs)
        {
			List<Card> clockPile = new List<Card>();
			for (int i = 0; i<4; i++)
            {
				card = Draw();
				clockPile.Add(card);
			}
			for (int i = 0; i < clockPile.Count; i++)
			{
				Card cc = clockPile[i];
			

				cc.transform.parent = layoutAnchor;

				Vector2 dpStagger = layout.slotDefs[tSD.layerID].stagger;
				cc.transform.localPosition = new Vector3(
					layout.multiplier.x * (layout.slotDefs[tSD.layerID].x + i * dpStagger.x),
					layout.multiplier.y * (layout.slotDefs[tSD.layerID].y + i * dpStagger.y),
					-layout.slotDefs[tSD.layerID].layerID + 0.1f * i);
				cc.faceUp = tSD.faceUp;
			}
		}
			UpdateDrawPile();
	}
	void UpdateDrawPile()
	{
		Card cd;

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
}