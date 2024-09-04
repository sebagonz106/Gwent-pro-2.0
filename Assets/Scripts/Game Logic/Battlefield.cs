using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Battlefield 
{
    Player playerThatOwnsThisBattlefield;
    public List<Card> Graveyard = new List<Card>();
    public List<Card> Melee = Enumerable.Repeat<Card>(Utils.BaseCard, 5).ToList<Card>();
    public List<Card> Range = Enumerable.Repeat<Card>(Utils.BaseCard, 5).ToList<Card>();
    public List<Card> Siege = Enumerable.Repeat<Card>(Utils.BaseCard, 5).ToList<Card>();
    public List<Card> Bonus = Enumerable.Repeat<Card>(Utils.BaseCard, 3).ToList<Card>(); //[0] MeleeBonus, [1] RangeBonus, [2] SiegeBonus
    public List<Card>[] Zones { get; private set; }
    List<bool> clearsPlayed = Enumerable.Repeat<bool>(false, 3).ToList<bool>(); //[0] Melee, [1] Range, [2] Siege 
    (Card, List<Card>, int) staysInBattlefieldController; //[0] Card, [1] List of cards where its played, [2] index
    public List<bool> ClearsPlayed { get => clearsPlayed; private set => clearsPlayed = value; }

    public Battlefield(Player player)
    {
        this.playerThatOwnsThisBattlefield = player;
        this.Zones = new List<Card>[] { Melee, Range, Siege };
    }

    #region Clearing methods
    public void ToGraveyard(Card card, List<Card> list)
    {
        if (card.Equals(Utils.BaseCard) || list.Equals(Graveyard)) return;

        if (list.Equals(this.playerThatOwnsThisBattlefield.Hand))
        {
            int index = list.IndexOf(card);
            this.playerThatOwnsThisBattlefield.EmptyHandAt(index);
        }
        else if (list.Equals(this.playerThatOwnsThisBattlefield.Deck))
        {
            this.playerThatOwnsThisBattlefield.Deck.Remove(card);
            Board.Instance.Receive(new RemoveOperation(card, list, list.Count-1, true));
        }
        else
        {
            if (card is ClearCard) RemoveClearEffect(Utils.IndexByZone[this.playerThatOwnsThisBattlefield.ZoneByList[list]]);
            Board.Instance.Receive(new RemoveOperation(card, list, list.IndexOf(card)));
            list[list.IndexOf(card)] = Utils.BaseCard;
        }

        Graveyard.Add(card);
        card.AssignPosition(Graveyard);
        Board.Instance.Receive(new AddOperation(card, Graveyard, Graveyard.Count - 1, true));
        if (card is UnitCard unit) unit.InitializeDamage();
    }

    public void ToGraveyard(List<Card> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            ToGraveyard(list[i], list);
        }
    }

    public void ToGraveyard(Card card)
    {
        if (card.CurrentPosition is null)
        {
            if (Bonus.Contains(card)) this.ToGraveyard(card, Bonus);
            else if (Board.Instance.Weather.Contains(card)) this.ToGraveyard(card, Board.Instance.Weather);
            else if (playerThatOwnsThisBattlefield.Hand.Contains(card)) this.ToGraveyard(card, playerThatOwnsThisBattlefield.Hand);
            else if (playerThatOwnsThisBattlefield.Deck.Contains(card)) this.ToGraveyard(card, playerThatOwnsThisBattlefield.Deck);
            else foreach (var zone in Zones) if (zone.Contains(card)) this.ToGraveyard(card, zone);
            //if card is already in graveyard, it will stay there
        }
        else ToGraveyard(card, card.CurrentPosition);
    }

    public bool Clear()
    {
        for (int i = 0; i < Board.Instance.Weather.Count; i++)
        {
            Card card = Board.Instance.Weather[i];

            //sending to graveyard only the cards on this list played by this player
            if (card.Owner.Equals(playerThatOwnsThisBattlefield))
            {
                this.ToGraveyard(card, Board.Instance.Weather);
            }
        }

        foreach (List<Card> item in Zones)
        {
            ToGraveyard(item);
        }
        ToGraveyard(this.Bonus);

        List<bool> clearsPlayed = Enumerable.Repeat<bool>(false, 3).ToList<bool>();

        if (staysInBattlefieldController.Item1 is Card stayingCard)
        {
            if (stayingCard is WeatherCard || stayingCard is BaitCard)
                staysInBattlefieldController.Item2[staysInBattlefieldController.Item3] = stayingCard;

            else if (!TryAdd(stayingCard, staysInBattlefieldController.Item2, staysInBattlefieldController.Item3))
                return false;
        }

        return true;
    }

    public void RemoveClearEffect(int position)
    {
        clearsPlayed[position] = false;
    }
    #endregion

    #region Adding methods
    public bool AddCard(Card card, Zone rangeType, int position = 0) => TryAdd(card, this.playerThatOwnsThisBattlefield.ListByZone[rangeType], position);

    bool TryAdd(Card card, List<Card> list, int index = 0)
    {
        int bonusAndClearIndex = Utils.IndexByZone[this.playerThatOwnsThisBattlefield.ZoneByList[list]];

        if (card.Type == Type.Bonus)
        {
            if (Bonus[bonusAndClearIndex].Equals(Utils.BaseCard))
            {
                Bonus[bonusAndClearIndex] = card;
                card.AssignPosition(Bonus);
                Board.Instance.Receive(new AddOperation(card, Bonus, bonusAndClearIndex));
                return true;
            }
            else return false;
        }
        else if (list[index].Equals(Utils.BaseCard))
        {
            list[index] = card;
            card.AssignPosition(list);
            Board.Instance.Receive(new AddOperation(card, list, index));
            if (card.Type == Type.Clear) ClearsPlayed[bonusAndClearIndex] = true; //Creator's license here: Clear will only protect from 
                                                                                 //weather effects the battlefield line where it is played
            return true;
        }

        return false;
    }
    #endregion

    #region utils
    public (UnitCard, List<Card>) SilverCardWithHighestOrLowestDamage(bool highestOrLowestDamage) //analizes which is the most or least powerful unit card in the field and returns it alongside the list where it is played
    {
        UnitCard unit = null;
        List<Card> list = null;

        for (int i = 0; i < Zones.Length; i++) //not looking in Bonus
        {
            CompareListsOfCardsToGetTheSilverCardWithHighestOrLowestDamage(ref unit, ref list, Zones[i], highestOrLowestDamage);
        }

        return (unit, list);
    }

    private void CompareListsOfCardsToGetTheSilverCardWithHighestOrLowestDamage(ref UnitCard unit, ref List<Card> listToSave, List<Card> listToCheck, bool HighestOrLowestDamage)
    {
        foreach (Card item in listToCheck)
        {
            if (item is UnitCard unitItem && unitItem.Level == Level.Silver && (unit == null || Compare(unit.InitialDamage, HighestOrLowestDamage, unitItem.InitialDamage)))
            {
                unit = unitItem;
                listToSave = listToCheck;
            }
        }
    }
    public bool StaysInBattlefieldIs(string name)
    {
        if (!(staysInBattlefieldController.Item1 is Card)) return false;
        if (name == staysInBattlefieldController.Item1.Name) return true;
        return false;
    }
    public bool StaysInBattlefieldModifier(Card card, List<Card> list)
    {
        if (card is LeaderCard) return false;

        staysInBattlefieldController = (card, list, list.IndexOf(card));
        return true;
    }
    public void StaysInBattlefieldRemove() => staysInBattlefieldController = (null, null, -1);

    public List<Card> CardsInBattlefield
    {
        get
        {
            List<Card> list = new List<Card>();
            foreach (var zone in Zones)
            {
                foreach (var item in zone)
                {
                    if (!item.Equals(Utils.BaseCard)) list.Add(item);
                }
            }
            foreach (var item in Bonus)
            {
                if (!item.Equals(Utils.BaseCard)) list.Add(item);
            }
            foreach (var card in Board.Instance.Weather)
            {
                if (!card.Equals(Utils.BaseCard) && card is ICardsWithOwner common && common.Owner.Equals(playerThatOwnsThisBattlefield)) list.Add(card);
            }
            return list;
        }
    }

    bool Compare(double a, bool biggest, double b) => biggest ? a > b : a < b;
    #endregion
}
