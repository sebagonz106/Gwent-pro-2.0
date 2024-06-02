using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInitializer : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] BoardMB board;
    [SerializeField] List<CardSO> cardsInfo;
    [SerializeField] List<Card> cards;
    bool cardsStolen;

    void Awake()
    {
        foreach (CardSO item in cardsInfo)
        {
            item.information = Resources.Load<Sprite>($"Info/{(this.player.PlayerName==Faction.Fidel? "Fidel" : "Batista")}/{item.name}");
            item.material = Resources.Load<Material>($"Materials/{(this.player.PlayerName == Faction.Fidel ? "Fidel" : "Batista")}/{item.name}");

            switch (item.cardType)
            {
                case CardType.Bait:
                    cards.Add(new BaitCard(item as BaitCardSO));
                    break;
                case CardType.Bonus:
                    cards.Add(new BonusCard(item as BonusCardSO));
                    break;
                case CardType.Weather:
                    cards.Add(new WeatherCard(item as WeatherCardSO));
                    break;
                case CardType.Clear:
                    cards.Add(new ClearCard(item as ClearCardSO));
                    break;
                case CardType.Unit:
                    cards.Add(new UnitCard(item as UnitCardSO, Effects.GetEffect(item.name)));
                    break;
                default:
                    break;
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

        cards.RemoveRange(25, cardsInfo.Count-25);
        player.Deck.AddRange(this.cards);
    }

    private void OnMouseDown()
    {
        if (!board.masterController.IsPlayersPanelActive()) return;

        if (board.board.GetCurrentPlayer().Equals(player) && 
           !board.masterController.validTurn              && 
            board.board.RoundCount == 1                   && 
           !cardsStolen                                     )
        {
            int index1 = Random.Range(0, player.Hand.Count - 1);
            int index2 = 0;
            do { index2 = Random.Range(0, player.Hand.Count - 1); } while (index1 == index2); //making sure the method wont take the same card

            player.Battlefield.ToGraveyard(player.Hand[index1], player.Hand);
            player.Battlefield.ToGraveyard(player.Hand[index2], player.Hand);
            player.GetCard(2);
            cardsStolen = true;
            board.UpdateView();
        }
        else board.masterController.GeneralException();
    }
}
