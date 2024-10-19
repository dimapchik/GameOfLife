using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pattern : MonoBehaviour {
    
    public string name;
    public Vector3Int [] cells;

    public Pattern(string name, Vector3Int[] cells) {
        this.name = name;
        this.cells = cells;
    }
}
