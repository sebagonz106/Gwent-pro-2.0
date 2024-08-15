using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusCard : Card
{
    public double Increase { get; private set; }

    public BonusCard(string name, Faction faction, CardType cardType, List<Zone> availableRange, double increase = 1, Effect effect = null) : base(name, faction, cardType, availableRange, increase, effect)
    {
        Increase = increase == 0 ? 1 : increase >= 10 ? increase / 10 : increase;
    }

    /* This card will have two possible non excluyent behaviours:
     * 1. an effect will activate upon being played
     * 2. a bonus will be placed on the line where this card was played
     * 2.1 bonus value will be given by this bonus' initial damage
     */
}
