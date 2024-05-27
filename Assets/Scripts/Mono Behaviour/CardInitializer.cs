using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInitializer : MonoBehaviour
{
    public Player player;
    public List<Card> cards;
    bool cardsStolen;

    void Awake()
    {
        foreach (Card item in cards)
        {
            item.information = Resources.Load<Sprite>($"Info/{(this.player.PlayerName==Faction.Fidel? "Fidel" : "Batista")}/{item.name}");
            item.material = Resources.Load<Material>($"Materials/{(this.player.PlayerName == Faction.Fidel ? "Fidel" : "Batista")}/{item.name}");

            if (item is UnitCard unit)
            {
                unit.InitializeDamage();

                switch (unit.name) //effect asssignation
                {
                    #region Rebel effects
                    case "Juan Almeida":
                        unit.AssignEffect(Effects.NoOneSurrendersHereGodDamn);
                        break;
                    case "Raul Castro":
                        unit.AssignEffect(Effects.StealCard);
                        break;
                    case "Celia Sanchez":
                        unit.AssignEffect(Effects.ClearsLineWithFewerCards);
                        break;
                    case "Luchadores clandestinos":
                        unit.AssignEffect(Effects.PlaceBonusInLineWhereIsPlayed);
                        break;
                    case "Guajiros":
                        unit.AssignEffect(Effects.MultipliesHisDamageTimesCardsLikeThis);
                        break;
                    #endregion

                    #region Batista effects
                    case "Martin Diaz Tamayo":
                        unit.AssignEffect(Effects.EqualsSilverUnitsDamageToBattlefieldsAverage);
                        break;
                    case "Pilar Garcia":
                        unit.AssignEffect(Effects.RemovePowerfulCard);
                        break;
                    case "Chivatos":
                        unit.AssignEffect(Effects.RemoveEnemyWorstCard);
                        break;
                    case "Tanque de guerra":
                        unit.AssignEffect(Effects.PlaceLightButIrremovableWeatherInEnemysBattlefield);
                        break;
                    case "Cuarteles":
                        unit.AssignEffect(Effects.MultipliesHisDamageTimesCardsLikeThis);
                        break;
                    #endregion

                    default:
                        unit.AssignEffect(Effects.VoidEffect);
                        break;
                }
            }
        }

        #region Modern Fisher-Yates shuffle algorithm
            int randomNumber;
            Card swapCard;

            for (int i = cards.Count - 1; i >= 0; i--)
            {
                randomNumber = Random.Range(0, cards.Count-1);
                swapCard = cards[randomNumber];
                cards[randomNumber] = cards[i];
                cards[i] = swapCard;
            }
        #endregion

        cards.RemoveRange(25, cards.Count-25);
        player.Deck.AddRange(this.cards);
    }

    private void OnMouseDown()
    {
        if (!player.board.masterController.IsPlayersPanelActive()) return;

        if (player.board.GetCurrentPlayer().Equals(player) && 
            !player.board.masterController.validTurn && 
            player.board.RoundCount == 1 && !cardsStolen)
        {
            int index1 = Random.Range(0, player.Hand.Count - 1);
            int index2 = 0;
            do { index2 = Random.Range(0, player.Hand.Count - 1); } while (index1 == index2); //making sure the method wont take the same card

            this.player.Battlefield.ToGraveyard(player.Hand[index1], player.Hand);
            this.player.Battlefield.ToGraveyard(player.Hand[index2], player.Hand);
            this.player.GetCard(2);
            this.cardsStolen = true;
            this.player.board.UpdateView();
        }
        else player.board.masterController.GeneralException();
    }
}
