using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Battlefield 
{
    public Player playerThatOwnsThisBattlefield;
    public List<Card> Graveyard = new List<Card>();
    public List<Card> Melee = Enumerable.Repeat<Card>(Utils.BaseCard, 5).ToList<Card>();
    public List<Card> Range = Enumerable.Repeat<Card>(Utils.BaseCard, 5).ToList<Card>();
    public List<Card> Siege = Enumerable.Repeat<Card>(Utils.BaseCard, 5).ToList<Card>();
    public List<Card> Bonus = Enumerable.Repeat<Card>(Utils.BaseCard, 3).ToList<Card>(); //[0] MeleeBonus, [1] RangeBonus, [2] SiegeBonus
    List<bool> clearsPlayed = Enumerable.Repeat<bool>(false, 3).ToList<bool>(); //[0] Melee, [1] Range, [2] Siege 
    (Card, List<Card>, int) staysInBattlefieldController; //[0] Card, [1] List of cards where its played, [2] index
    public List<bool> ClearsPlayed { get => clearsPlayed; private set => clearsPlayed = value; }

    public void ToGraveyard(Card card, List<Card> list)
    {
        if (card.Equals(Utils.BaseCard)) return;

        Graveyard.Add(card);
        if (list.Equals(this.playerThatOwnsThisBattlefield.Hand)) this.playerThatOwnsThisBattlefield.EmptyHandAt(list.IndexOf(card));
        else
        {
            if (card is ClearCard) RemoveClearEffect(Utils.GetIntByBattlfieldList(this, list));
            list[list.IndexOf(card)] = Utils.BaseCard;
        }
    }

    public void ToGraveyard(List<Card> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            ToGraveyard(list[i], list);
        }
    }
    public bool Clear()
    {
        for (int i = 0; i < Board.Instance.Weather.Count; i++)
        {
            Card card = Board.Instance.Weather[i];

            //sending to graveyard only the cards on this list played by this player
            if ((card is BaitCard bait && bait.PlayerThatPlayedThisCard.Equals(playerThatOwnsThisBattlefield)) || (card is WeatherCard weather && weather.PlayerThatPlayedThisCard.Equals(playerThatOwnsThisBattlefield)))
            {
                this.ToGraveyard(card, Board.Instance.Weather);
            }
        }

        this.ToGraveyard(this.Melee);
        this.ToGraveyard(this.Range);
        this.ToGraveyard(this.Siege);
        this.ToGraveyard(this.Bonus);
        List<bool> clearsPlayed = Enumerable.Repeat<bool>(false, 3).ToList<bool>();

        if (staysInBattlefieldController.Item1 is Card stayingCard)
        {
            if (stayingCard is WeatherCard || stayingCard is BaitCard || stayingCard is BonusCard)
                staysInBattlefieldController.Item2[staysInBattlefieldController.Item3] = stayingCard;

            else if (!TryAdd(stayingCard, staysInBattlefieldController.Item2, staysInBattlefieldController.Item3))
                return false;
        }

        return true;
    }

    public bool AddCard(Card card, Zone rangeType, int position) => TryAdd(card, Utils.GetListByRangeType(playerThatOwnsThisBattlefield, rangeType), position);

    public (Card, List<Card>) MostPowerfulSilverCard() //analizes which is the most powerful unit card in the field and returns it alongside the list where it is played
    {
        UnitCard unit = null;
        List<Card> list = null;

        CompareListsOfCardsToGetTheSilverCardWithHighestOrLowestDamage(ref unit, ref list, this.Melee, true);
        CompareListsOfCardsToGetTheSilverCardWithHighestOrLowestDamage(ref unit, ref list, this.Range, true);
        CompareListsOfCardsToGetTheSilverCardWithHighestOrLowestDamage(ref unit, ref list, this.Siege, true);

        return (unit, list);
    }

    public (Card, List<Card>) LeastPowerfulCard() //analizes which is the least powerful unit card in the field and returns it alongside the list where it is played
    {
        UnitCard unit = null;
        List<Card> list = null;

        CompareListsOfCardsToGetTheSilverCardWithHighestOrLowestDamage(ref unit, ref list, this.Melee, false);
        CompareListsOfCardsToGetTheSilverCardWithHighestOrLowestDamage(ref unit, ref list, this.Range, false);
        CompareListsOfCardsToGetTheSilverCardWithHighestOrLowestDamage(ref unit, ref list, this.Siege, false);

        return (unit, list);
    }
    private void CompareListsOfCardsToGetTheSilverCardWithHighestOrLowestDamage(ref UnitCard unit, ref List<Card> listToSave, List<Card> listToCheck, bool HighestOrLowestDamage)
    {
        if (HighestOrLowestDamage) //gets card with higher damage
        {
            foreach (Card item in listToCheck)
            {
                if (item is UnitCard unitItem && unitItem.level == Level.Silver && (unit == null || unit.InitialDamage < unitItem.InitialDamage))
                {
                    unit = unitItem;
                    listToSave = listToCheck;
                }
            }
        }
        else //gets card with lower damage
        {
            foreach (Card item in listToCheck)
            {
                if (item is UnitCard unitItem && unitItem.level == Level.Silver && (unit == null || unit.InitialDamage > unitItem.InitialDamage))
                {
                    unit = unitItem;
                    listToSave = listToCheck;
                }
            }
        }
    }
    public bool StaysInBattlefieldIs(string name)
    {
        if (!(staysInBattlefieldController.Item1 is Card)) return false;
        if (name == staysInBattlefieldController.Item1.name) return true;
        return false;
    }
    public bool StaysInBattlefieldModifier(Card card, List<Card> list)
    {
        if (card is LeaderCard) return false;

        staysInBattlefieldController = (card, list, list.IndexOf(card));
        return true;
    }

    bool TryAdd(Card card, List<Card> list, int index)
    {
        int bonusAndClearIndex = Utils.GetIntByBattlfieldList(this, list);

        if (list[index].Equals(Utils.BaseCard))
        {
            if (card.cardType == CardType.Unit)
            {
                list[index] = card;
                return true;
            }
            if (card.cardType == CardType.Clear) //Creator's license here: Clear will only protect from 
                                                 //weather effects the battlefield line where it is played
            {
                list[index] = card;
                ClearsPlayed[bonusAndClearIndex] = true;
                return true;
            }
        }
        if (card.cardType == CardType.Bonus && Bonus[bonusAndClearIndex].Equals(Utils.BaseCard))
        {
            Bonus[bonusAndClearIndex] = card;
            return true;
        }

        return false;
    }
    public void RemoveClearEffect(int position)
    {
        clearsPlayed[position] = false;
    }
}
