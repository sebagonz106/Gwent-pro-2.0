using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardsWarehouse
{
    public static List<Card> RebelCards = new List<Card>
    {
        new LeaderCard("Camilo Cienfuegos", Faction.Fidel, CardType.Leader, false),
        new LeaderCard("Ernesto Che Guevara", Faction.Fidel, CardType.Leader, true),
        new BaitCard("Retirada estrategica", Faction.Fidel, CardType.Bait, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}),
        new ClearCard("Capas y linternas", Faction.Fidel, CardType.Clear, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}),
        new ClearCard("Mantas", Faction.Fidel, CardType.Clear, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}),
        new ClearCard("Santeria", Faction.Fidel, CardType.Clear, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}),
        new WeatherCard("Lluvia", Faction.Fidel, CardType.Weather, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}),
        new WeatherCard("Frente Frio", Faction.Fidel, CardType.Weather, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}),
        new WeatherCard("Niebla", Faction.Fidel, CardType.Weather, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}),
    };
    public static List<Card> BatistaCards = new List<Card>
    {
        new LeaderCard("Eulogio Cantillo", Faction.Batista, CardType.Leader, false),
        new LeaderCard("Francisco Tabernilla", Faction.Batista, CardType.Leader, true),
        new BaitCard("Retirada estrategica", Faction.Batista, CardType.Bait, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}),
        new ClearCard("Capas y linternas", Faction.Batista, CardType.Clear, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}),
        new ClearCard("Mantas", Faction.Batista, CardType.Clear, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}),
        new ClearCard("Santeria", Faction.Batista, CardType.Clear, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}),
        new WeatherCard("Lluvia", Faction.Batista, CardType.Weather, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}),
        new WeatherCard("Frente Frio", Faction.Batista, CardType.Weather, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}),
        new WeatherCard("Niebla", Faction.Batista, CardType.Weather, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}),
    };
}
