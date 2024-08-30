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
    static Card baseCard = new Card("", Faction.Fidel, CardType.Unit, new List<Zone>());
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
}