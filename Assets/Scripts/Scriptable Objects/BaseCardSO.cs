using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Base Card", menuName = "Scriptable Objects/Base Card", order = 0)]
public class CardSO : ScriptableObject
{
    public new string name;
    public Faction faction;
    public CardType cardType;
    public List<Zone> availableRange;
    public Material material;
    public Sprite information;
    public List<Card> currentPosition;
}
