using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Effects
{ //parameters composition: 0 Board, 1 Player player, 2 Player enemy, 3 List<Card> currentPosition, 4 Card
 //works depending on cards being played before aplying their effects.

    #region Rebel effects

    public static bool StealCard(params object[] parameters) => ((Player)parameters[1]).GetCard(); //for it to work, the card with this effect must not 
                                                                                                   //be the first card played of a full hand
    public static bool NoOneSurrendersHereGodDamn(params object[] parameters)
    {
        ((Board)parameters[0]).AlmeidaIsPlayed = true;
        return true;
    }

    public static bool ClearsLineWithFewerCards(params object[] parameters) // if there are two or more lists with the same amount of cards played 
                                                                            // this method will clear the first of the lists checked, altough if an enemy
                                                                            // list has the same amount of cards, it will clear the enemy's list.
                                                                            // In case a card set to stay in battlefield is discarded, it will reappear
                                                                            // next round, as soon as battlefield's clear method is called.
                                                                            // Creator's License here: this effect will be able to affect golden cards.
    {
        Board board = (Board)parameters[0];
        Player thisPlayer = (Player)parameters[1];
        int minCount = 6; //Max amount of cards this can count
        List<Card> list = new List<Card>();
        Card bonus = Utils.BaseCard;
        Player player = (Player)parameters[2];

        if (CountCardsInListAndCompareWithMinCount(board.Fidel.Battlefield.Melee, board.Fidel, ref minCount, ref list, thisPlayer))
        {
            bonus = board.Fidel.Battlefield.Bonus[0];
            player = board.Fidel;
        }
        if (CountCardsInListAndCompareWithMinCount(board.Fidel.Battlefield.Range, board.Fidel, ref minCount, ref list, thisPlayer))
        {
            bonus = board.Fidel.Battlefield.Bonus[1];
            player = board.Fidel;
        }
        if (CountCardsInListAndCompareWithMinCount(board.Fidel.Battlefield.Siege, board.Fidel, ref minCount, ref list, thisPlayer))
        {
            bonus = board.Fidel.Battlefield.Bonus[2];
            player = board.Fidel;
        }
        if (CountCardsInListAndCompareWithMinCount(board.Batista.Battlefield.Melee, board.Batista, ref minCount, ref list, thisPlayer))
        {
            bonus = board.Batista.Battlefield.Bonus[0];
            player = board.Batista;
        }
        if (CountCardsInListAndCompareWithMinCount(board.Batista.Battlefield.Range, board.Batista, ref minCount, ref list, thisPlayer))
        {
            bonus = board.Batista.Battlefield.Bonus[1];
            player = board.Batista;
        }
        if (CountCardsInListAndCompareWithMinCount(board.Batista.Battlefield.Siege, board.Batista, ref minCount, ref list, thisPlayer))
        {
            bonus = board.Batista.Battlefield.Bonus[2];
            player = board.Batista;
        }

        player.Battlefield.ToGraveyard(bonus, player.Battlefield.Bonus);
        player.Battlefield.ToGraveyard(list);

        return true;
    }

    public static bool PlaceBonusInLineWhereIsPlayed(params object[] parameters)
    {
        List<Card> currentPosition = (List<Card>)parameters[3];
        Player player = (Player)parameters[1];
        BonusCard riot = Resources.Load<BonusCard>("Huelga Revolucionaria");

        if (player.Battlefield.AddCard(riot, Utils.GetRangeTypeByList(player, currentPosition), Utils.GetIntByBattlfieldList(player.Battlefield, currentPosition)))
        {
            ((Board)parameters[0]).UpdateView(true);
            return true;
        }
        else return false;
    }
    #endregion

    #region Batista effects

    public static bool RemovePowerfulCard(params object[] parameters)
    {
        List<object> fidelList = ((Board)parameters[0]).Fidel.Battlefield.MostPowerfulSilverCard();
        List<object> batistaList = ((Board)parameters[0]).Batista.Battlefield.MostPowerfulSilverCard();

        if ((UnitCard)fidelList[0] == null && (UnitCard)batistaList[0] == null) return false; //checks if there is no unit silver card played

        //checks if Batistas most powerful card outpowers Fidels most powerful card. If so, it sends Batsitas card to graveyard
        if ((UnitCard)fidelList[0] == null || ((UnitCard)fidelList[0]).InitialDamage < ((UnitCard)batistaList[0]).InitialDamage)
        {
            ((Board)parameters[0]).Batista.Battlefield.ToGraveyard((UnitCard)batistaList[0], (List<Card>)batistaList[1]);
        }

        //checks if Fidels most powerful card outpowers Batistas most powerful card. If so, it sends it to graveyard
        else if ((UnitCard)batistaList[0] == null || ((UnitCard)fidelList[0]).InitialDamage > ((UnitCard)batistaList[0]).InitialDamage) 
{
            ((Board)parameters[0]).Fidel.Battlefield.ToGraveyard(((UnitCard)fidelList[0]), ((List<Card>)fidelList[1]));
        }

        //checks if Fidels most powerful card has the same damage as Batistas most powerful card. If so, it sends both to graveyard.
        else if (((UnitCard)fidelList[0]).InitialDamage == ((UnitCard)batistaList[0]).InitialDamage)
        {
            ((Board)parameters[0]).Batista.Battlefield.ToGraveyard(((UnitCard)batistaList[0]), ((List<Card>)batistaList[1]));
            ((Board)parameters[0]).Fidel.Battlefield.ToGraveyard(((UnitCard)fidelList[0]), ((List<Card>)fidelList[1]));
        }

        else return false; //in case of unexpected behaviour
        return true;
    }

    public static bool RemoveEnemyWorstCard(params object[] parameters)
    {
        if (((Player)parameters[2]).Battlefield.LeastPowerfulCard()[0] == null) return false;

        ((Player)parameters[2]).Battlefield.ToGraveyard(((UnitCard)((Player)parameters[2]).Battlefield.LeastPowerfulCard()[0]), ((List<Card>)((Player)parameters[2]).Battlefield.LeastPowerfulCard()[1])); //sends to graveyard the least powerful card of the given player
        return true;
    }

    public static bool PlaceLightButIrremovableWeatherInEnemysBattlefield(params object[] parameters)
    {
        Battlefield enemy = ((Player)parameters[2]).Battlefield;

        ReduceDamagePermanently(enemy.Melee);
        ReduceDamagePermanently(enemy.Range);
        ReduceDamagePermanently(enemy.Siege);

        return true;
    }

    public static bool EqualsSilverUnitsDamageToBattlefieldsAverage(params object[] parameters)
    {
        Board board = (Board)parameters[0];
        List<UnitCard> list = new List<UnitCard>();
        double pureTotalDamage = 0;
        double average = 0;

        StoreUnitsInListAndModifyTotalDamageOfTheCardsInTheList(board.Fidel.Battlefield.Melee, list, ref pureTotalDamage);
        StoreUnitsInListAndModifyTotalDamageOfTheCardsInTheList(board.Fidel.Battlefield.Range, list, ref pureTotalDamage);
        StoreUnitsInListAndModifyTotalDamageOfTheCardsInTheList(board.Fidel.Battlefield.Siege, list, ref pureTotalDamage);
        StoreUnitsInListAndModifyTotalDamageOfTheCardsInTheList(board.Batista.Battlefield.Melee, list, ref pureTotalDamage);
        StoreUnitsInListAndModifyTotalDamageOfTheCardsInTheList(board.Batista.Battlefield.Range, list, ref pureTotalDamage);
        StoreUnitsInListAndModifyTotalDamageOfTheCardsInTheList(board.Batista.Battlefield.Siege, list, ref pureTotalDamage);

        average = pureTotalDamage / list.Count;

        foreach (UnitCard item in list)
        {
            if (item.level == Level.Silver) item.ModifyOnFieldDamage(average, true);
        }

        return true;
    }
    #endregion

    #region Common effects
    public static bool MultipliesHisDamageTimesTheAmountOfCardsLikeThisOneInBattlefield(params object[] parameters)
    {
        Player player = (Player)parameters[1];
        UnitCard card = (UnitCard)parameters[4];
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

    public static bool VoidEffect(params object[] parameters) => true;
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
    static void StoreUnitsInListAndModifyTotalDamageOfTheCardsInTheList(List<Card> listToCheck, List<UnitCard> listToStore, ref double damage)
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
