using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInitializer : MonoBehaviour
{
    [SerializeField] PlayerMB playerMB;
    [SerializeField] BoardMB board;
    List<Card> cards;
    bool cardsStolen;
    Player player => playerMB.player;

    void Start()
    {
        cards = CardsWarehouse.GetDeck(playerMB.Name);
        foreach (Card item in cards) AssignInfo(item);

        Utils.ShuffleList(cards);

        cards.RemoveRange(25, cards.Count-25);
        player.Deck.AddRange(this.cards);
    }

    private void OnMouseDown()
    {
        if (!board.masterController.IsPlayersPanelActive()) return;

        if (board.board.GetCurrentPlayer().Equals(player) && 
           !board.board.ValidTurn                         && 
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
            board.board.NewTurn();
        }
        else board.masterController.GeneralException();
    }

    void AssignInfo(Card card)
    {
        if(card.Info is null) card.AssignInfo(new VisualInfo(Resources.Load<Material>($"Materials/{playerMB.Name}/{card.Name}"),
                                                             Resources.Load<Sprite>($"Info/{playerMB.Name}/{card.Name}"), 
                                                             card.Faction));
    }
}
