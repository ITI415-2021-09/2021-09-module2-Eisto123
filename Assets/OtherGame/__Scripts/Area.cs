using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{
    static public Area S;
    // Start is called before the first frame update
    public int areaID;
    void Awake()
    {
        S = this;
    }
}


