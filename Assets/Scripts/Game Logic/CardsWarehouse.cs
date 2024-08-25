using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardsWarehouse
{
    public static List<Card> RebelCards = new List<Card>
    {
        new LeaderCard("Camilo Cienfuegos", Faction.Fidel, CardType.Leader, false),
        new LeaderCard("Ernesto Che Guevara", Faction.Fidel, CardType.Leader, true),

        new BaitCard("Resistencia organizada", Faction.Fidel, CardType.Bait, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}),

        new ClearCard("Capas y linternas", Faction.Fidel, CardType.Clear, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}),
        new ClearCard("Mantas", Faction.Fidel, CardType.Clear, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}),
        new ClearCard("Santeria", Faction.Fidel, CardType.Clear, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}),

        new WeatherCard("Lluvia", Faction.Fidel, CardType.Weather, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}),
        new WeatherCard("Frente Frio", Faction.Fidel, CardType.Weather, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}),
        new WeatherCard("Niebla", Faction.Fidel, CardType.Weather, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}),

        new BonusCard("Huelga revolucionaria", Faction.Fidel, CardType.Bonus, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}, 1.8),
        new BonusCard("Expedicion revolucionaria", Faction.Fidel, CardType.Bonus, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}, 1.5),
        new BonusCard("Transmision de Radio Rebelde", Faction.Fidel, CardType.Bonus, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}, 1.2),

        new UnitCard("Luchadores clandestinos", Faction.Fidel, CardType.Unit, new List<Zone>{Zone.Melee, Zone.Siege }, Level.Golden, 8, Effects.PlaceBonusInLineWhereIsPlayed),
        new UnitCard("Raul Castro", Faction.Fidel, CardType.Unit, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege }, Level.Golden, 8, Effects.StealCard),
        new UnitCard("Celia Sanchez", Faction.Fidel, CardType.Unit, new List<Zone>{Zone.Siege, Zone.Range }, Level.Golden, 7, Effects.ClearsLineWithFewerCards),
        new UnitCard("Juan Almeida", Faction.Fidel, CardType.Unit, new List<Zone>{Zone.Range, Zone.Melee }, Level.Golden, 7, Effects.NoOneSurrendersHereGodDamn),

        new UnitCard("Rifleros", Faction.Fidel, CardType.Unit, new List<Zone>{Zone.Melee, Zone.Range }, Level.Silver, 6),
        new UnitCard("Francotiradores", Faction.Fidel, CardType.Unit, new List<Zone>{Zone.Siege, Zone.Range }, Level.Silver, 6),
        new UnitCard("Lanzamisiles", Faction.Fidel, CardType.Unit, new List<Zone>{Zone.Melee, Zone.Siege }, Level.Silver, 6),
        new UnitCard("Subfusiles", Faction.Fidel, CardType.Unit, new List<Zone>{Zone.Melee }, Level.Silver, 5),
        new UnitCard("Marianas", Faction.Fidel, CardType.Unit, new List<Zone>{Zone.Range }, Level.Silver, 5),
        new UnitCard("Guajiros", Faction.Fidel, CardType.Unit, new List<Zone>{Zone.Siege }, Level.Silver, 4, Effects.MultipliesHisDamageTimesCardsLikeThis),
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

        new BonusCard("Asesoria estadounidense", Faction.Batista, CardType.Bonus, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}, 1.8),
        new BonusCard("Casinos y hoteles", Faction.Batista, CardType.Bonus, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}, 1.5),
        new BonusCard("Ayuda de la burguesia nacional", Faction.Batista, CardType.Bonus, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege}, 1.2),

        new UnitCard("Martin Diaz Tamayo", Faction.Batista, CardType.Unit, new List<Zone>{Zone.Melee, Zone.Range }, Level.Golden, 8, Effects.EqualsSilverUnitsDamageToBattlefieldsAverage),
        new UnitCard("Tanque de guerra", Faction.Batista, CardType.Unit, new List<Zone>{Zone.Melee, Zone.Range, Zone.Siege }, Level.Golden, 8, Effects.PlaceLightButIrremovableWeatherInEnemysBattlefield),
        new UnitCard("Chivatos", Faction.Batista, CardType.Unit, new List<Zone>{Zone.Siege, Zone.Range }, Level.Golden, 7, Effects.RemoveEnemyWorstCard),
        new UnitCard("Pilar Garcia", Faction.Batista, CardType.Unit, new List<Zone>{Zone.Siege, Zone.Melee }, Level.Golden, 7, Effects.RemovePowerfulCard),

        new UnitCard("Aviones", Faction.Batista, CardType.Unit, new List<Zone>{Zone.Siege, Zone.Range }, Level.Silver, 6),
        new UnitCard("Ametralladora ligera", Faction.Batista, CardType.Unit, new List<Zone>{Zone.Siege, Zone.Melee }, Level.Silver, 6),
        new UnitCard("Blindados ligeros", Faction.Batista, CardType.Unit, new List<Zone>{Zone.Melee, Zone.Range }, Level.Silver, 6),
        new UnitCard("Infanteria batistiana", Faction.Batista, CardType.Unit, new List<Zone>{Zone.Melee }, Level.Silver, 5),
        new UnitCard("Policia", Faction.Batista, CardType.Unit, new List<Zone>{Zone.Range }, Level.Silver, 5),
        new UnitCard("Cuarteles", Faction.Batista, CardType.Unit, new List<Zone>{Zone.Siege }, Level.Silver, 4, Effects.MultipliesHisDamageTimesCardsLikeThis),
    };

    static List<Card> FullDeck(List<Card> list)
    {
        List<Card> temp = new List<Card>();
        List<LeaderCard> leaders = new List<LeaderCard>();

        foreach (var card in list)
            if (card is UnitCard unit && unit.Level is Level.Silver)
                temp.AddRange(new Card[] { unit, unit });
            else if (card is LeaderCard leader)
                leaders.Add(leader);

        temp.AddRange(list);
        foreach (var item in leaders)
            temp.Remove(item);
        return temp;
    }
    public static List<Card> GetDeck(string name) => name == "Batista" ? FullDeck(BatistaCards) : FullDeck(RebelCards);
}
