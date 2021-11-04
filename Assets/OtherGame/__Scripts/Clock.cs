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
	public GameObject detactAreaPref;


	[Header("Set Dynamically")]
	public cDeck					deck;
	public cLayout layout;
	public List<CardClock> drawPile;
	public List<CardClock> Pile;
	public List<List<CardClock>> clock = new List<List<CardClock>>();
	public Transform layoutAnchor;
	public bool draggingMode = false;
	public bool cardMatch = false;
	public GameObject area;


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
			Vector3 pos = new Vector3(
					layout.multiplier.x * (layout.slotDefs[tSD.layerID].x),
					layout.multiplier.y * (layout.slotDefs[tSD.layerID].y),
					-12);
			setDetectArea(pos);
			area.GetComponent<Area>().areaID = tSD.id;
			clock.Add(clockPile);
			
		}
		
		CardClock iniCard = clock[12][0];
		iniCard.faceUp = true;
		iniCard.state = cCardState.target;
		iniCard.SetSortingLayerName("layer1");
		clock[12].Remove(iniCard);
		UpdateDrawPile();
	}

	void setDetectArea(Vector3 pos)
    {
		area = Instantiate(detactAreaPref) as GameObject;
		area.transform.position = pos;
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
	void UpdateDeck()
	{
		foreach (cSlotDef tSD in layout.slotDefs)
		{
			List<CardClock> clockPile = clock[tSD.id - 1];
			for (int i = 0; i < clockPile.Count; i++)
			{
				CardClock cc = clockPile[i];
				cc.transform.parent = layoutAnchor;

				Vector2 dpStagger = layout.slotDefs[tSD.layerID].stagger;
				cc.transform.localPosition = new Vector3(
					layout.multiplier.x * (layout.slotDefs[tSD.layerID].x + i * dpStagger.x),
					layout.multiplier.y * (layout.slotDefs[tSD.layerID].y + i * dpStagger.y),
					-layout.slotDefs[tSD.layerID].layerID + 0.1f * i);
			}
		}
	}
	public int CheckArea(GameObject a)
    {
		int id = a.GetComponent<Area>().areaID;
		return id;
    }

	public void Swap(CardClock cc, int i)
    {
		Debug.Log("called");
		int id = cc.rank;
		clock[id - 1].Add(cc);
		cc.state = cCardState.tableau;
		cc.SetSortingLayerName("Default");
		UpdateDeck();
		CardClock targetCard = clock[id - 1][0];
		targetCard.faceUp = true;
		targetCard.state = cCardState.target;
		targetCard.SetSortingLayerName("layer1");
		clock[id - 1].Remove(targetCard);
	}
	public void CardClicked(CardClock cd)
	{
		switch (cd.state)
		{
			case cCardState.target:
				draggingMode = true;
				break;
			case cCardState.tableau:
				break;
		}
	}
	private void Update()
	{
		if (!draggingMode) return;
	}
}