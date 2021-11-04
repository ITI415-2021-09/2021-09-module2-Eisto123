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
    [Header("Set Dynamically: CardClock")]
    public cCardState state = cCardState.drawpile;
    public int layoutID;
    public cSlotDef slotDef;
    private Vector3 _dragOffset;
    private Camera _cam;

    void Awake()
    {
        _cam = Camera.main;
    }



    override public void OnMouseUpAsButton()
    {
        Clock.S.CardClicked(this);

        base.OnMouseUpAsButton();
    }
    void OnMouseDown()
    {
        Debug.Log("clicked");
        if (state == cCardState.target)
        {
            Debug.Log("clicked");
            _dragOffset = transform.position - GetMousePos();
        }
        
    }

    void OnMouseDrag()
    {
        if (state == cCardState.target)
        {
            Debug.Log("draged");
            transform.position = GetMousePos() + _dragOffset;
        }
    }

    Vector3 GetMousePos()
    {
        var mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    }
    public void OnTriggerEnter(Collider other)
    {
        GameObject a = other.gameObject;
        int id = Clock.S.CheckArea(a);
        if(id == this.rank)
        {
            Clock.S.Swap(this, id);
        }
    }
}
