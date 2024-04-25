using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Base Card", menuName = "Scriptable Objects/Base Card", order = 0)]
public class Card : ScriptableObject
{
    public new string name;
    public Faction faction;
    public CardType cardType;
    public List<RangeType> availableRange;
    public Material material;
    public Sprite information;
    public List<Card> currentPosition;
    
    public Card(string name, Faction faction, CardType cardType, List<RangeType> availableRange, Material material, Sprite information, List<Card> currentPosition)
    {
        this.name = name;
        this.faction = faction;
        this.cardType = cardType;
        this.availableRange = availableRange;
        this.material = material;
        this.information = information;
        this.currentPosition = currentPosition;
    }

    public Card(string name, Material material)
    {
        this.name = name;
        this.material = material;
    }

    public override bool Equals(object other)
    {
        return other is Card card && this.name == card.name;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
