using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderCard : Card
{
    public bool NeedsCardSelection { get; private set; }

    public LeaderCard(string name, Faction faction, Type cardType, bool needsCardSelection = false, double initialDamage = 0, Effect effect = null) :
                 base(name, faction, cardType, new List<Zone>(), initialDamage, effect)
    {
        NeedsCardSelection = needsCardSelection;
    }

    public override bool Effect(Context context)
    {
        Board.Instance.Receive(this);
        try
        {
            if (effect is null) return NeedsCardSelection ? KeepInBattlefield(context.CurrentPlayer, context.CurrentCard, context.CurrentPosition) :
                                                            StealCard(context.CurrentPlayer);
            else return effect.Invoke(context);
        }
        catch 
        {
            return false;
        }
    }

    private bool KeepInBattlefield(Player player, Card card, List<Card> list)
    {
        if (player.LeaderEffectUsedThisRound || !this.NeedsCardSelection || !player.Battlefield.StaysInBattlefieldModifier(card, list)) return false;

        player.LeaderEffectUsedThisRound = true;
        return true;
    }

    private bool StealCard(Player player)
    {
        if (player.LeaderEffectUsedThisRound || this.NeedsCardSelection || !player.GetCard()) return false;

        player.LeaderEffectUsedThisRound = true;
        return true;
    }
}
