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
    public float reloadDelay = 2f;
    public GameObject panel;


    [Header("Set Dynamically")]
    public cDeck					deck;
    public cLayout layout;
    public List<CardClock> drawPile;
    public List<CardClock> Pile;
    public List<List<CardClock>> clock = new List<List<CardClock>>();
    public Transform layoutAnchor;
    public bool kingClear = true;
    public GameObject area;
    public Text gameOverText;



    void Awake(){
        S = this;
        SetUpUITexts();
    }
    void SetUpUITexts()
    {
        GameObject go = GameObject.Find("GameOver");
        if (go != null)
        {
            gameOverText = go.GetComponent<Text>();
        }

        ShowResultsUI(false);
    }

    void ShowResultsUI(bool show)
    {
        gameOverText.gameObject.SetActive(show);
        panel.SetActive(show);
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
                    -13);
            setDetectArea(pos);
            area.GetComponent<Area>().areaID = tSD.id;
            clock.Add(clockPile);
            
        }
        
        CardClock iniCard = clock[12][0];
        iniCard.faceUp = true;
        iniCard.state = cCardState.target;
        iniCard.SetSortingLayerName("layer2");
        iniCard.transform.position = new Vector3(iniCard.transform.position.x, iniCard.transform.position.y, -13);
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
            clock[tSD.id-1][0].SetSortingLayerName("layer1");
        }
    }
    public int CheckArea(GameObject a)
    {
        int id = a.GetComponent<Area>().areaID;
        return id;
    }

    public void Swap(CardClock cc)
    {
        //Debug.Log("called");
        
        int id = cc.rank;
        if (id == 13)
        {
            if (CheckForKingClear())
            {
                if (!clock[12][0].faceUp)
                {
                    clock[12][0].faceUp = true;
                }
                CheckForGameOver();
                clock[id - 1].Add(cc);
                cc.state = cCardState.tableau;
                cc.SetSortingLayerName("Default");
                UpdateDeck();
                return;
            }
        }
        clock[id - 1].Add(cc);
        cc.state = cCardState.tableau;
        cc.SetSortingLayerName("Default");
        UpdateDeck();
        CardClock targetCard = clock[id - 1][0];
        targetCard.faceUp = true;
        targetCard.state = cCardState.target;
        targetCard.transform.position = new Vector3(targetCard.transform.position.x, targetCard.transform.position.y, -13);
        targetCard.SetSortingLayerName("layer2");
        clock[id - 1].Remove(targetCard);
        clock[id - 1][0].SetSortingLayerName("layer1");
    }
    
    public bool CheckForKingClear()
    {
        kingClear = true;
        for (int i = 0; i < clock[12].Count; i++)
        {
            if (clock[12][i].rank != 13)
            {
                kingClear = false;
            }
        }
        return kingClear;
    }
    void CheckForGameOver()
    {
        for (int i = 0; i < clock.Count; i++)
            {
                for (int c = 0; c < clock[i].Count; c++)
                {
                    if (!clock[i][c].faceUp) {
                        GameOver(false);
                        return;
                    }
                }
            }
        GameOver(true);
        return;
    }

    //Called when the game is over. Simple for now, but expandable
    void GameOver(bool won)
    {
        if (won)
        {
            gameOverText.text = "You Won";
            ShowResultsUI(true);
        }
        else
        {
            gameOverText.text = "Try Again";
            ShowResultsUI(true);
        }
        //SceneManager.LoadScene("_Prospector_Scene_0");
        Invoke("ReloadLevel", reloadDelay);
    }
    void ReloadLevel()
    {
        SceneManager.LoadScene("__Clock_Scene_0");
    }

}