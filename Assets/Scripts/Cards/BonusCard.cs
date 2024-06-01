using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusCard : Card
{
    public double Increase { get; private set; }
    public BonusCard(BonusCardSO bonus) : base(bonus)
    {
        this.Increase = bonus.increase;
    }

    public BonusCard(string name, Faction faction, CardType cardType, List<Zone> availableRange, VisualInfo info, List<Card> currentPosition, double increase) : base(name, faction, cardType, availableRange, info, currentPosition)
    {
        this.Increase = increase;
    }
}
