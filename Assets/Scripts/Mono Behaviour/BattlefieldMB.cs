using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattlefieldMB : MonoBehaviour
{
    public PlayerMB playerThatOwnsThisBattlefield;
    public Battlefield battlefield;

    private void Awake()
    {
        this.battlefield = playerThatOwnsThisBattlefield.player.Battlefield;
    }

    public void Clear()
    {
        if (!battlefield.Clear()) playerThatOwnsThisBattlefield.board.masterController.EffectException();
    }

    public bool AddCard(Card card, Zone rangeType, int position) => TryAdd(rangeType is Zone.Melee, card, this.Melee, position) ||
                                                                         TryAdd(rangeType is Zone.Range, card, this.Range, position) ||
                                                                         TryAdd(rangeType is Zone.Siege, card, this.Siege, position);
    
    public List<object> MostPowerfulSilverCard() //analiza cual es la carta mas poderosa del campo y la devuelve en una lista junto con la fila en la que se encuentra
    {
        UnitCard unit = null;
        List<Card> list = null;

        CompareListsOfCardsToGetTheSilverCardWithHighestOrLowestDamage(ref unit, ref list, this.Melee, true);
        CompareListsOfCardsToGetTheSilverCardWithHighestOrLowestDamage(ref unit, ref list, this.Range, true);
        CompareListsOfCardsToGetTheSilverCardWithHighestOrLowestDamage(ref unit, ref list, this.Siege, true);

        return new List<object> { unit, list };
    }

    public (Card, List<Card>) LeastPowerfulCard() //analiza cual es la carta mmenos poderosa del campo y la devuelve en una lista junto con la fila en la que se encuentra
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
