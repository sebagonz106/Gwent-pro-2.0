using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsPositionWarehouse : MonoBehaviour
{
    public GameObject defaultPosition;
    public GameObject[] positions = new GameObject[10];
    public bool[] isTaken = new bool[10];
}
