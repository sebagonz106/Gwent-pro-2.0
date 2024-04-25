using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Battlefield : MonoBehaviour
{
    public Player playerThatOwnsThisBattlefield;
    public List<Card> Graveyard = new List<Card>();
    public List<Card> Melee;
    public List<Card> Range;
    public List<Card> Siege;
    public List<Card> Bonus; //[0] MeleeBonus, [1] RangeBonus, [2] SiegeBonus
    List<bool> clearsPlayed = Enumerable.Repeat<bool>(false, 3).ToList<bool>(); //[0] Melee, [1] Range, [2] Siege 
    List<object> staysInBattlefieldController = Enumerable.Repeat<object>(false, 3).ToList<object>(); //[0] Card, [1] List of cards where its played, [2] index

    private void Awake()
    {
        Melee = Enumerable.Repeat<Card>(Utils.BaseCard, 5).ToList<Card>();
        Range = Enumerable.Repeat<Card>(Utils.BaseCard, 5).ToList<Card>();
        Siege = Enumerable.Repeat<Card>(Utils.BaseCard, 5).ToList<Card>();
        Bonus = Enumerable.Repeat<Card>(Utils.BaseCard, 3).ToList<Card>();
    }

    public List<bool> ClearsPlayed { get => clearsPlayed; private set => clearsPlayed = value; }

    public void ToGraveyard(Card card, List<Card> list)
    {
        if (card.Equals(Utils.BaseCard)) return;

        Graveyard.Add(card);
        if (list.Equals(this.playerThatOwnsThisBattlefield.Hand)) this.playerThatOwnsThisBattlefield.EmptyAt(list.IndexOf(card));
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
            Graveyard.Add(list[i]);
            list[i] = Utils.BaseCard;
        }
    }
    public void Clear()
    {
        for (int i = 0; i < playerThatOwnsThisBattlefield.board.Weather.Count; i++)
        {
            Card card = playerThatOwnsThisBattlefield.board.Weather[i];

            //sending to graveyard only the cards on this list played by this player
            if ((card is BaitCard bait && bait.PlayerThatPlayedThisCard.Equals(playerThatOwnsThisBattlefield)) || (card is WeatherCard weather && weather.PlayerThatPlayedThisCard.Equals(playerThatOwnsThisBattlefield)))
            {
                this.ToGraveyard(card, playerThatOwnsThisBattlefield.board.Weather);
            }
        }

        this.ToGraveyard(this.Melee);
        this.ToGraveyard(this.Range);
        this.ToGraveyard(this.Siege);
        this.ToGraveyard(this.Bonus);
        List<bool> clearsPlayed = Enumerable.Repeat<bool>(false, 3).ToList<bool>();

        if (staysInBattlefieldController[0] is Card stayingCard)
        {
            if (stayingCard is WeatherCard || stayingCard is BaitCard || stayingCard is BonusCard)
                 ((List<Card>)staysInBattlefieldController[1])[(int)staysInBattlefieldController[2]] = stayingCard;

            else if (!TryAdd(true, stayingCard, (List<Card>)staysInBattlefieldController[1], (int)staysInBattlefieldController[2]))
                  playerThatOwnsThisBattlefield.board.masterController.EffectException();
        }
    }
    public bool AddCard(Card card, RangeType rangeType, int position) => TryAdd(rangeType is RangeType.Melee, card, this.Melee, position) ||
                                                                         TryAdd(rangeType is RangeType.Range, card, this.Range, position) ||
                                                                         TryAdd(rangeType is RangeType.Siege, card, this.Siege, position);
    
    public List<object> MostPowerfulSilverCard() //analiza cual es la carta mas poderosa del campo y la devuelve en una lista junto con la fila en la que se encuentra
    {
        UnitCard unit = null;
        List<Card> list = null;

        CompareListsOfCardsToGetTheSilverCardWithHighestOrLowestDamage(ref unit, ref list, this.Melee, true);
        CompareListsOfCardsToGetTheSilverCardWithHighestOrLowestDamage(ref unit, ref list, this.Range, true);
        CompareListsOfCardsToGetTheSilverCardWithHighestOrLowestDamage(ref unit, ref list, this.Siege, true);

        return new List<object> { unit, list };
    }

    public List<object> LeastPowerfulCard() //analiza cual es la carta mmenos poderosa del campo y la devuelve en una lista junto con la fila en la que se encuentra
    {
        UnitCard unit = null;
        List<Card> list = null;

        CompareListsOfCardsToGetTheSilverCardWithHighestOrLowestDamage(ref unit, ref list, this.Melee, false);
        CompareListsOfCardsToGetTheSilverCardWithHighestOrLowestDamage(ref unit, ref list, this.Range, false);
        CompareListsOfCardsToGetTheSilverCardWithHighestOrLowestDamage(ref unit, ref list, this.Siege, false);

        return new List<object> { unit, list };
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
        if(!(staysInBattlefieldController[0] is Card)) return false;
        if (name == ((Card)staysInBattlefieldController[0]).name) return true;
        return false;
    }
    public bool StaysInBattlefieldModifier(Card card, List<Card> list)
    {
        if (card is LeaderCard) return false;
        
        staysInBattlefieldController = new List<object> { card, list, list.IndexOf(card) };
        return true;
    }

    bool TryAdd(bool greenLight, Card card, List<Card> list, int index)
    {
        if (greenLight)
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
                                                     //weather effects the battlrfield line where it is played
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
        }

        return false;
    }
    public void RemoveClearEffect(int position)
    {
        clearsPlayed[position] = false;
    }
}
