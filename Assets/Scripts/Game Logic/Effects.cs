using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Effects
{ //parameters composition: 0 Board, 1 Player player, 2 Player enemy, 3 List<Card> currentPosition, 4 Card
 //works depending on cards being played before aplying their effects.

    #region Rebel effects

    public static bool StealCard(Context context) => context.CurrentPlayer.GetCard(); //for it to work, the card with this effect must not 
                                                                                                   //be the first card played of a full hand
    public static bool NoOneSurrendersHereGodDamn(Context context)
    {
        context.Board.AlmeidaIsPlayed = true;
        return true;
    }

    public static bool ClearsLineWithFewerCards(Context context) // if there are two or more lists with the same amount of cards played 
                                                                            // this method will clear the first of the lists checked, altough if an enemy
                                                                            // list has the same amount of cards, it will clear the enemy's list.
                                                                            // In case a card set to stay in battlefield is discarded, it will reappear
                                                                            // next round, as soon as battlefield's clear method is called.
                                                                            // Creator's License here: this effect will be able to affect golden cards.
    {
        int minCount = 6; //Max amount of cards this can count
        List<Card> list = new List<Card>();
        Card bonus = Utils.BaseCard;
        Player player = context.EnemyPlayer;

        if (CountCardsInListAndCompareWithMinCount(context.Board.Fidel.Battlefield.Melee, context.Board.Fidel, ref minCount, ref list, context.CurrentPlayer))
        {
            bonus = context.Board.Fidel.Battlefield.Bonus[0];
            player = context.Board.Fidel;
        }
        if (CountCardsInListAndCompareWithMinCount(context.Board.Fidel.Battlefield.Range, context.Board.Fidel, ref minCount, ref list, context.CurrentPlayer))
        {
            bonus = context.Board.Fidel.Battlefield.Bonus[1];
            player = context.Board.Fidel;
        }
        if (CountCardsInListAndCompareWithMinCount(context.Board.Fidel.Battlefield.Siege, context.Board.Fidel, ref minCount, ref list, context.CurrentPlayer))
        {
            bonus = context.Board.Fidel.Battlefield.Bonus[2];
            player = context.Board.Fidel;
        }
        if (CountCardsInListAndCompareWithMinCount(context.Board.Batista.Battlefield.Melee, context.Board.Batista, ref minCount, ref list, context.CurrentPlayer))
        {
            bonus = context.Board.Batista.Battlefield.Bonus[0];
            player = context.Board.Batista;
        }
        if (CountCardsInListAndCompareWithMinCount(context.Board.Batista.Battlefield.Range, context.Board.Batista, ref minCount, ref list, context.CurrentPlayer))
        {
            bonus = context.Board.Batista.Battlefield.Bonus[1];
            player = context.Board.Batista;
        }
        if (CountCardsInListAndCompareWithMinCount(context.Board.Batista.Battlefield.Siege, context.Board.Batista, ref minCount, ref list, context.CurrentPlayer))
        {
            bonus = context.Board.Batista.Battlefield.Bonus[2];
            player = context.Board.Batista;
        }

        player.Battlefield.ToGraveyard(bonus, player.Battlefield.Bonus);
        player.Battlefield.ToGraveyard(list);

        return true;
    }

    public static bool PlaceBonusInLineWhereIsPlayed(Context context)
    {
        List<Card> currentPosition = context.CurrentPosition;
        Player player = context.CurrentPlayer;
        BonusCard riot = Resources.Load<BonusCard>("Huelga Revolucionaria");

        if (player.Battlefield.AddCard(riot, Utils.GetRangeTypeByList(player, currentPosition), Utils.GetIntByBattlfieldList(player.Battlefield, currentPosition)))
        {
            context.Board.UpdateView(true);
            return true;
        }
        else return false;
    }
    #endregion

    #region Batista effects

    public static bool RemovePowerfulCard(Context context)
    {
        (UnitCard, List<Card>) fidelList = Player.Fidel.Battlefield.SilverCardWithHighestOrLowestDamage();
        (UnitCard, List<Card>) batistaList = Player.Batista.Battlefield.SilverCardWithHighestOrLowestDamage();

        if (fidelList.Item1 == null && batistaList.Item1 == null) return false; //checks if there is no unit silver card played

        //checks if Batistas most powerful card outpowers Fidels most powerful card. If so, it sends Batsitas card to graveyard
        if (fidelList.Item1 == null || (fidelList.Item1).InitialDamage < (batistaList.Item1).InitialDamage)
        {
            context.Board.Batista.Battlefield.ToGraveyard((UnitCard)batistaList[0], (List<Card>)batistaList[1]);
        }

        //checks if Fidels most powerful card outpowers Batistas most powerful card. If so, it sends it to graveyard
        else if ((UnitCard)batistaList[0] == null || ((UnitCard)fidelList[0]).InitialDamage > ((UnitCard)batistaList[0]).InitialDamage) 
{
            context.Board.Fidel.Battlefield.ToGraveyard(((UnitCard)fidelList[0]), ((List<Card>)fidelList[1]));
        }

        //checks if Fidels most powerful card has the same damage as Batistas most powerful card. If so, it sends both to graveyard.
        else if (((UnitCard)fidelList[0]).InitialDamage == ((UnitCard)batistaList[0]).InitialDamage)
        {
            context.Board.Batista.Battlefield.ToGraveyard(((UnitCard)batistaList[0]), ((List<Card>)batistaList[1]));
            context.Board.Fidel.Battlefield.ToGraveyard(((UnitCard)fidelList[0]), ((List<Card>)fidelList[1]));
        }

        else return false; //in case of unexpected behaviour
        return true;
    }

    public static bool RemoveEnemyWorstCard(Context context)
    {
        (Card, List<Card>) leastPower = context.EnemyPlayer.Battlefield.SilverCardWithHighestOrLowestDamage();
        if (leastPower.Item1 == null) return false;

        context.EnemyPlayer.Battlefield.ToGraveyard(leastPower.Item1, leastPower.Item2); //sends to graveyard the least powerful card of the given player
        return true;
    }

    public static bool PlaceLightButIrremovableWeatherInEnemysBattlefield(Context context)
    {
        Battlefield enemy = context.EnemyPlayer.Battlefield;

        ReduceDamagePermanently(enemy.Melee);
        ReduceDamagePermanently(enemy.Range);
        ReduceDamagePermanently(enemy.Siege);

        return true;
    }

    public static bool EqualsSilverUnitsDamageToBattlefieldsAverage(Context context)
    {
        List<UnitCard> list = new List<UnitCard>();
        double pureTotalDamage = 0;
        double average = 0;

        StoreUnitsInListAndCountDamage(context.Board.Fidel.Battlefield.Melee, list, ref pureTotalDamage);
        StoreUnitsInListAndCountDamage(context.Board.Fidel.Battlefield.Range, list, ref pureTotalDamage);
        StoreUnitsInListAndCountDamage(context.Board.Fidel.Battlefield.Siege, list, ref pureTotalDamage);
        StoreUnitsInListAndCountDamage(context.Board.Batista.Battlefield.Melee, list, ref pureTotalDamage);
        StoreUnitsInListAndCountDamage(context.Board.Batista.Battlefield.Range, list, ref pureTotalDamage);
        StoreUnitsInListAndCountDamage(context.Board.Batista.Battlefield.Siege, list, ref pureTotalDamage);

        average = pureTotalDamage / list.Count;

        foreach (UnitCard item in list)
        {
            if (item.level == Level.Silver) item.ModifyOnFieldDamage(average, true);
        }

        return true;
    }
    #endregion

    #region Common effects
    public static bool MultipliesHisDamageTimesCardsLikeThis(Context context)
    {
        Player player = context.CurrentPlayer;
        UnitCard card = (UnitCard)context.CurrentCard;
        int count; 
        List<UnitCard> list = new List<UnitCard>();

        //FindEveryCardInList(player.Battlefield.Melee, card, list, ref count); 
        //FindEveryCardInList(player.Battlefield.Range, card, list, ref count);
        //*unnecesary because cards with this effect can only be played in Siege

        FindEveryCardInListAndCountThem(player.Battlefield.Siege, card, list, out count);

        foreach (UnitCard item in list)
        {
            item.ModifyOnFieldDamage(count * item.InitialDamage, true);
        }
        return true;
    }

    public static bool VoidEffect(Context context) => true;
    #endregion

    #region Utils
    static bool CountCardsInListAndCompareWithMinCount(List<Card> toCount, Player checkingPlayer, ref int minCount, ref List<Card> minCountList, Player thisPlayer)
    {
        int tempCount = 0;
        foreach (Card item in toCount)
        {
            if (!item.Equals(Utils.BaseCard)) tempCount++;
        }
        if (!thisPlayer.Battlefield.Bonus[Utils.GetIntByBattlfieldList(thisPlayer.Battlefield, toCount)].Equals(Utils.BaseCard)) tempCount++;

        if (tempCount>0 && (tempCount<minCount||(tempCount==minCount && !checkingPlayer.Equals(thisPlayer))))
        {
            minCountList = toCount;
            minCount = tempCount;
            return true;
        }
        return false;
    }
    static void FindEveryCardInListAndCountThem(List<Card> listToSearch, UnitCard card, List<UnitCard> listToSave, out int count)
    {
        count = 0;
        foreach (Card item in listToSearch)
        {
            if (card.Equals(item))
            {
                count++;
                listToSave.Add((UnitCard)item);
            }
        }
    }
    static void ReduceDamagePermanently(List<Card> list, int toTake = 1)
    {
        foreach (Card item in list)
        {
            if (item is UnitCard unit && unit.level == Level.Silver) unit.ModifyOnFieldDamage(unit.DamageOnField - toTake, true);
        }
    }
    static void StoreUnitsInListAndCountDamage(List<Card> listToCheck, List<UnitCard> listToStore, ref double damage)
    {
        foreach (Card item in listToCheck)
        {
            if (item is UnitCard unit)
            {
                listToStore.Add(unit);
                damage += unit.DamageOnField;
            }
        }
    }
    #endregion
}
