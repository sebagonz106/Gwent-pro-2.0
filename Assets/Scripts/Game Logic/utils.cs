using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Faction
{
    Fidel,
    Batista,
}

public enum Type
{
    Unit,
    Bonus,
    Leader,
    Weather,
    Clear,
    Bait,
}

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
    static Card baseCard = new Card("", Faction.Fidel, Type.Unit, new List<Zone>());
    public static Card BaseCard
    {
        get
        {
            if(baseCard.Info is null) baseCard.AssignInfo(new VisualInfo(Resources.Load<Material>("DiselectedBattlefieldCard"), null, "base"));
            return baseCard;
        }
    }

    public static string[] ZonesName = { "Weather", "Batista Bonus", "Batista Melee", "Batista Range", "Batista Siege", "Fidel Bonus", "Fidel Melee", "Fidel Range", "Fidel Siege" };
    public static Dictionary<Zone, int> IndexByZone = new Dictionary<Zone, int> { { Zone.Melee, 0 }, { Zone.Range, 1 }, { Zone.Siege, 2 } };
    public static Dictionary<Faction, string> FactionName = new Dictionary<Faction, string> { { Faction.Fidel, "Fidel" }, { Faction.Batista, "Batista" } };

    public static Player GetPlayerByFaction(Faction faction) => faction is Faction.Fidel ? Player.Fidel : Player.Batista;
    public static Player GetPlayerByName(string name) => name == "Fidel" ? Player.Fidel : Player.Batista;
    public static Player GetEnemyByName(string name) => name == "Batista" ? Player.Fidel : Player.Batista;
    public static Player GetEnemyOf(Player player) => Player.Fidel.Equals(player) ? Player.Batista : Player.Fidel;

    public static void ShuffleList(List<Card> list, bool saveOperation = false) //Modern Fisher-Yates shuffle algorithm
    {
        List<Card> initialList = new List<Card>();
        foreach (var card in list) initialList.Add(card);

        int randomNumber;
        Card swapCard;

        for (int i = list.Count - 1; i >= 0; i--)
        {
            randomNumber = (new System.Random()).Next(list.Count - 1);
            swapCard = list[randomNumber];
            list[randomNumber] = list[i];
            list[i] = swapCard;
        }

        if (saveOperation) Board.Instance.Receive(new ShuffleOperation(initialList, list));
    }
}