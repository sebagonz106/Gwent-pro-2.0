using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bonus Card", menuName = "Scriptable Objects/Bonus Card")]
public class BonusCard : Card
{
    public double increase;

    public BonusCard(string name, Material material) : base(name, material)
    {
    }

    public BonusCard(string name, Faction faction, CardType cardType, List<RangeType> availableRange, Material material, Sprite information, List<Card> currentPosition, double increase) : base(name, faction, cardType, availableRange, material, information, currentPosition)
    {
        this.increase = increase;
    }
}
