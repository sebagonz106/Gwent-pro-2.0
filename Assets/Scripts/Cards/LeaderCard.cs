using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderCard : Card, IEffect
{
    public bool StealCardLeader { get; private set; }

    public LeaderCard(LeaderCardSO leader) : base(leader)
    {
        StealCardLeader = leader.stealCardLeader;
    }

    public LeaderCard(string name, Faction faction, CardType cardType, List<Zone> availableRange, VisualInfo info, List<Card> currentPosition, bool stealCardLeader) : base(name, faction, cardType, availableRange, info, currentPosition)
    {
        StealCardLeader = stealCardLeader;
    }

    public bool Effect (Context context) => StealCardLeader? StealCard(context.CurrentPlayer) : KeepInBattlefield(context.CurrentPlayer, context.CurrentCard, context.CurrentPosition);


    private bool KeepInBattlefield(Player player, Card card, List<Card> list)
    {
        if (player.LeaderEffectUsedThisRound || this.StealCardLeader || !player.Battlefield.StaysInBattlefieldModifier(card, list)) return false;

        player.LeaderEffectUsedThisRound = true;
        return true;
    }

    private bool StealCard(Player player)
    {
        if (player.LeaderEffectUsedThisRound || !this.StealCardLeader || !player.GetCard()) return false;

        player.LeaderEffectUsedThisRound = true;
        return true;
    }
}
