using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherCard : Card, IEffect
{
    public Player PlayerThatPlayedThisCard { get; set; }

    public WeatherCard(WeatherCardSO weather) : base(weather)
    {
    }

    public WeatherCard(string name, Faction faction, CardType cardType, List<Zone> availableRange, VisualInfo info, List<Card> currentPosition) : base (name, faction, cardType, availableRange, info, currentPosition)
    {
    }

    public bool Effect(Context context) //reduces the damage of every silver unit card played:
    {
        ReduceSilverUnitCardPowerInLineIfPossible(context.Board, Zone.Melee); //if affects melee
        ReduceSilverUnitCardPowerInLineIfPossible(context.Board, Zone.Range); //if affects range
        ReduceSilverUnitCardPowerInLineIfPossible(context.Board, Zone.Siege); //if affects siege
        return true;
    }

    private void ReduceSilverUnitCardPowerInLineIfPossible(Board board, Zone rangeType)
    {
        if (this.AvailableRange.Contains(rangeType))
        {
            List<Card> fidel = Utils.GetListByRangeType(Player.Fidel, rangeType);
            List<Card> batista = Utils.GetListByRangeType(Player.Batista, rangeType);

            for (int i = 0; i < 5; i++)
            {
                if (batista[i] is UnitCard batistaUnit && batistaUnit.Level == Level.Silver && !Player.Batista.Battlefield.ClearsPlayed[Utils.GetIntByRangeType(rangeType)])
                {
                    batistaUnit.Damage -= batistaUnit.Damage < 2 ? batistaUnit.Damage : 2;
                }
                if (fidel[i] is UnitCard fidelUnit && fidelUnit.Level == Level.Silver && !Player.Fidel.Battlefield.ClearsPlayed[Utils.GetIntByRangeType(rangeType)])
                {
                    fidelUnit.Damage -= fidelUnit.Damage < 2 ? fidelUnit.Damage : 2;
                }
            }
        }
    }
}