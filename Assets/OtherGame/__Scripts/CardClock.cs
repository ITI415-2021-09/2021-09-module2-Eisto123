using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum cCardState
{
    drawpile,
    tableau,
    target,
}
public class CardClock : Card
{
    // Start is called before the first frame update
    [Header("Set Dynamically: CardProspector")]
    public cCardState state = cCardState.drawpile;
    public int layoutID;
    public cSlotDef slotDef;

    override public void OnMouseUpAsButton()
    {
        Clock.S.CardClicked(this);

        base.OnMouseUpAsButton();
    }
}
