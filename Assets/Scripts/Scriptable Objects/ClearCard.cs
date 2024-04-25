using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Clear Card", menuName = "Scriptable Objects/Clear Card")]
public class ClearCard : Card
{
    public ClearCard(string name, Material material) : base(name, material)
    {
    }

    public ClearCard(string name, Faction faction, CardType cardType, List<RangeType> availableRange, Material material, Sprite information, List<Card> currentPosition) : base(name, faction, cardType, availableRange, material, information, currentPosition)
    {
    }
}
