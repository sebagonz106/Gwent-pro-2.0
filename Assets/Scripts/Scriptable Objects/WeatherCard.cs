using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weather Card", menuName = "Scriptable Objects/Weather Card")]
public class WeatherCard : Card
{
    private Player playerThatPlayedThisCard;
    public Player PlayerThatPlayedThisCard { get => playerThatPlayedThisCard; set => playerThatPlayedThisCard = value; }

    public WeatherCard(string name, Material material) : base(name, material)
    {
    }

    public void Effect(Board board) //reduces the damage of every silver unit card played:
    {
        ReduceSilverUnitCardPowerInLineIfPossible(board, Zone.Melee); //if affects melee
        ReduceSilverUnitCardPowerInLineIfPossible(board, Zone.Range); //if affects range
        ReduceSilverUnitCardPowerInLineIfPossible(board, Zone.Siege); //if affects siege
    }

    private void ReduceSilverUnitCardPowerInLineIfPossible(Board board, Zone rangeType)
    {
        if (this.availableRange.Contains(rangeType))
        {
            List<Card> fidel = Utils.GetListByRangeType(board.Fidel, rangeType);
            List<Card> batista = Utils.GetListByRangeType(board.Batista, rangeType);

            for (int i = 0; i < 5; i++)
            {
                if (batista[i] is UnitCard batistaUnit && batistaUnit.level == Level.Silver && !board.Batista.Battlefield.ClearsPlayed[Utils.GetIntByRangeType(rangeType)])
                {
                    batistaUnit.Damage -= batistaUnit.Damage < 2 ? batistaUnit.Damage : 2;
                }
                if (fidel[i] is UnitCard fidelUnit && fidelUnit.level == Level.Silver && !board.Fidel.Battlefield.ClearsPlayed[Utils.GetIntByRangeType(rangeType)])
                {
                    fidelUnit.Damage -= fidelUnit.Damage < 2 ? fidelUnit.Damage : 2;
                }
            }
        }
    }
}
