using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Faction
{
    Fidel,
    Batista,
}

public enum CardType
{
    Unit,
    Bonus,
    Leader,
    Weather,
    Clear,
    Bait,
}

public enum RangeType
{
    Melee,
    Range,
    Siege,
}

public enum Level
{
    Silver,
    Golden,
}

public delegate bool Effect(params object[] parameters);

public static class Utils 
{
    static Card baseCard = Resources.Load<Card>("Empty");

    public static Card BaseCard { get => baseCard; }

    public static List<Card> GetListByRangeType(Player player, RangeType rangeType) => rangeType == RangeType.Melee? player.Battlefield.Melee :
                                                                                       rangeType == RangeType.Range? player.Battlefield.Range : 
                                                                                                                     player.Battlefield.Siege;
    public static int GetIntByRangeType(RangeType rangeType) => rangeType == RangeType.Melee ? 0 :
                                                                rangeType == RangeType.Range ? 1 : 2;
    public static int GetIntByBattlfieldList(Battlefield field, List<Card> list) => list.Equals(field.Melee) ? 0 :
                                                                                    list.Equals(field.Range) ? 1 : 2;
    public static List<Card> GetListByName(Player player, string name) => name.Contains("Melee") ? player.Battlefield.Melee :
                                                                          name.Contains("Range") ? player.Battlefield.Range :
                                                                          name.Contains("Siege") ? player.Battlefield.Siege :
                                                                          name.Contains("Bonus") ? player.Battlefield.Bonus :
                                                                                                   player.Hand;
    public static RangeType GetRangeTypeByList(Player player, List<Card> list) => list.Equals(player.Battlefield.Melee) ? RangeType.Melee :
                                                                                  list.Equals(player.Battlefield.Range) ? RangeType.Range : RangeType.Siege;
}