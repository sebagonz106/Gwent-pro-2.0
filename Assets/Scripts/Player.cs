using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Faction playerName;
    public List<Card> Hand = new List<Card>(10);
    private List<int> emptySlotsInHand = new List<int>(10);
    public List<Card> Deck = new List<Card>(25);
    [SerializeField] LeaderCard stealCardLeader;
    [SerializeField] LeaderCard cardStaysLeader;
    LeaderCard leader;
    private bool leaderCardSelected = false;
    public bool leaderEffectUsedThisRound = false;
    public Battlefield Battlefield;
    public bool EndRound = false;
    public bool StartedPlaying = false;
    public int Score = 0;
    public GameObject WonCoin1;
    public GameObject WonCoin2;
    public Board board;
    public Context context = new Context(Board.board.Fidel);

    public Faction PlayerName { get => playerName; }
    public LeaderCard Leader { get => leader; set => leader = value; }
    public bool LeaderCardSelected { get => leaderCardSelected; set => leaderCardSelected = value; }

    private void Awake()
    {
        Hand = Enumerable.Repeat<Card>(Utils.BaseCard, 10).ToList<Card>();
        for (int i = 0; i < 10; i++)
        {
            emptySlotsInHand.Add(i);
        }

        if (this.playerName==Faction.Fidel)
        {
            if (stealCardLeader.name == PlayerPrefs.GetString("Rebel Leader")) Leader = stealCardLeader;
            else Leader = cardStaysLeader;

            Leader.information = Resources.Load<Sprite>($"Info/Fidel/{Leader.name}");
        }
        else
        {
            if (stealCardLeader.name == PlayerPrefs.GetString("Batista Leader")) Leader = stealCardLeader;
            else Leader = cardStaysLeader;

            Leader.information = Resources.Load<Sprite>($"Info/Batista/{Leader.name}");
        }
    }

    public bool GetCard(int cardsToSteal = 1)
    {
        if (Deck.Count<=cardsToSteal) //If there are no longer cards left in deck, this conditional shuffles
                                     //graveyard cards and add them to the deck so the game goes on
        {
            int randomNumber;
            Card swapCard;

            for (int k = Battlefield.Graveyard.Count - 1; k >= 0; k--)
            {
                randomNumber = Random.Range(0, Battlefield.Graveyard.Count - 1);
                swapCard = Battlefield.Graveyard[randomNumber];
                Battlefield.Graveyard[randomNumber] = Battlefield.Graveyard[k];
                Battlefield.Graveyard[k] = swapCard;
            }

            Deck.AddRange(Battlefield.Graveyard);
            Battlefield.Graveyard.Clear();
        }

        while (emptySlotsInHand.Count < cardsToSteal) //if cardsToSteal is bigger than the amount of empty slots in players hand, this sends 
                                                      //a random card in deck to graveyard until a proper value for cardsToSteal is achieved
        {
            int index = Random.Range(0, Deck.Count - 1);
            Battlefield.Graveyard.Add(Deck[index]);
            Deck.RemoveAt(index);
            cardsToSteal--;
        }

        if (cardsToSteal == 0) return false;

        for (int count = cardsToSteal; count > 0; count--)
        {
            int index = Random.Range(0, Deck.Count - 1);
            Hand[emptySlotsInHand[0]] = Deck[index];
            Deck.RemoveAt(index);
            emptySlotsInHand.RemoveAt(0);
        }

        return true;
    }

    public bool PlayCard(int originPosition, int targetPosition, Zone rangeType)
    {
        if(!(this.Hand[originPosition] is Card card) || card.Equals(Utils.BaseCard) || card is BaitCard) //in case of unexpected behaviours. bait cards will be played through their effect
        {
            return false;
        }

        if (card is WeatherCard weather && this.board.Weather[targetPosition].Equals(Utils.BaseCard)) //play weather card
        {
            this.board.Weather[targetPosition] = weather;
            weather.PlayerThatPlayedThisCard = this;
        }
        else if (this.Battlefield.AddCard(card, rangeType, targetPosition)) //play unit, clear and bonus card
        {
            if (card is UnitCard unit) //activating unit cart effect
            {
                if(!unit.Effect(this.context.UpdateInstance(Utils.GetListByRangeType(this, rangeType), unit)))
                {
                    board.masterController.EffectException();
                }
            }
        }
        else
        {
            return false;
        }

        EmptyAt(originPosition);
        this.board.UpdateView(true);
        return true;
    }

    public void EmptyAt(int index)
    {
        this.emptySlotsInHand.Add(index);
        this.Hand[index] = Utils.BaseCard;
    }

    public override bool Equals(object other)
    {
        return other is Player otherPlayer && this.PlayerName==otherPlayer.PlayerName;
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
