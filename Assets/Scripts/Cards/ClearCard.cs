using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCard : Card
{
    public ClearCard(ClearCardSO clear) : base(clear)
    {
    }

    public ClearCard(string name, Faction faction, CardType cardType, List<Zone> availableRange, VisualInfo info, List<Card> currentPosition) : base(name, faction, cardType, availableRange, info, currentPosition)
    {
    }
}