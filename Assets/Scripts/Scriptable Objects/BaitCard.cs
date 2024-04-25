using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bait Card", menuName = "Scriptable Objects/Bait Card")]
public class BaitCard : Card
{
    Player playerThatPlayedThisCard;
    public Player PlayerThatPlayedThisCard { get => playerThatPlayedThisCard; }

    public BaitCard(string name, Material material) : base(name, material)
    {
    }

    public BaitCard(string name, Faction faction, CardType cardType, List<RangeType> availableRange, Material material, Sprite information, List<Card> currentPosition) : base(name, faction, cardType, availableRange, material, information, currentPosition)
    {
    }

    public bool Effect(Player player, List<Card> list, int index)
    {
        Card card = list[index];
        if (card is BaitCard) return false;
        list[index] = this;
        player.Hand[player.Hand.IndexOf(this)] = card;
        if (card is UnitCard unit) unit.InitializeDamage(); //in case any permanent effects were applied on this card
        if (card is ClearCard) player.Battlefield.RemoveClearEffect(Utils.GetIntByBattlfieldList(player.Battlefield, list));
        this.playerThatPlayedThisCard = player;
        return true;
    }
}
