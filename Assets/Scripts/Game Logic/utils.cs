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

//[Flags]
public enum Zone
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

public delegate bool Effect(Context context);

public static class Utils 
{
    static Card baseCard = Resources.Load<Card>("Empty");

    public static Card BaseCard { get => baseCard; }

    public static List<Card> GetListByRangeType(Player player, Zone rangeType) => rangeType == Zone.Melee? player.Battlefield.Melee :
                                                                                       rangeType == Zone.Range? player.Battlefield.Range : 
                                                                                                                     player.Battlefield.Siege;
    public static int GetIntByRangeType(Zone rangeType) => rangeType == Zone.Melee ? 0 :
                                                                rangeType == Zone.Range ? 1 : 2;
    public static int GetIntByBattlfieldList(Battlefield field, List<Card> list) => list.Equals(field.Melee) ? 0 :
                                                                                    list.Equals(field.Range) ? 1 : 2;
    public static List<Card> GetListByName(Player player, string name) => name.Contains("Melee") ? player.Battlefield.Melee :
                                                                          name.Contains("Range") ? player.Battlefield.Range :
                                                                          name.Contains("Siege") ? player.Battlefield.Siege :
                                                                          name.Contains("Bonus") ? player.Battlefield.Bonus :
                                                                                                   player.Hand;
    public static Zone GetRangeTypeByList(Player player, List<Card> list) => list.Equals(player.Battlefield.Melee) ? Zone.Melee :
                                                                             list.Equals(player.Battlefield.Range) ? Zone.Range : Zone.Siege;

    public static Player GetPlayerByName(string name) => name == "Fidel" ? Player.Fidel : Player.Batista;
    public static Player GetEnemyByName(string name) => name == "Batista" ? Player.Fidel : Player.Batista;
}