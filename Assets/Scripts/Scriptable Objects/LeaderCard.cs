using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Leader Card", menuName = "Scriptable Objects/Leader Card")]
public class LeaderCard : Card
{
    public bool stealCardLeader = true;

    public LeaderCard(string name, Material material) : base(name, material)
    {
    }

    public LeaderCard(string name, Faction faction, CardType cardType, List<RangeType> availableRange, Material material, Sprite information, List<Card> currentPosition) : base(name, faction, cardType, availableRange, material, information, currentPosition)
    {
    }

    public void KeepInBattlefield(Card card, List<Card> list)
    {
        Player player = this.faction == Faction.Batista ? GameObject.Find("Batista").GetComponent<Player>() : GameObject.Find("Fidel").GetComponent<Player>();

        if (player.leaderEffectUsedThisRound || this.stealCardLeader || !player.Battlefield.StaysInBattlefieldModifier(card, list))
        {
            player.board.masterController.EffectException();
            return;
        }

        player.leaderEffectUsedThisRound = true;
        player.board.masterController.validTurn = true;
    }

    public void StealCard()
    {
        Player player = this.faction == Faction.Batista ? GameObject.Find("Batista").GetComponent<Player>() : GameObject.Find("Fidel").GetComponent<Player>();

        if (player.leaderEffectUsedThisRound || !this.stealCardLeader || !player.GetCard())
        {
            player.board.masterController.EffectException();
            return;
        }

        player.leaderEffectUsedThisRound = true;
        player.board.masterController.validTurn = true;
    }
}
