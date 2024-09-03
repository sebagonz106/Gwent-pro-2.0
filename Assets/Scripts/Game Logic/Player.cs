using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player
{
    #region Fields, properties and builder
    Faction playerFaction;
    public List<Card> Hand = new List<Card>(10);
    private List<int> emptySlotsInHand = new List<int>(10);
    public List<Card> Deck = new List<Card>(25);
    public LeaderCard Leader;
    public bool LeaderCardSelected = false;
    public bool LeaderEffectUsedThisRound = false;
    public bool EndRound = false;
    public bool StartedPlaying = false;
    public double TotalDamage = 0;
    public int Score = 0;
    public Battlefield Battlefield;
    public Context context;
    private static Player fidel;
    private static Player batista;

    public static Dictionary<string, LeaderCard> Leaders = new Dictionary<string, LeaderCard>
    {
        {"Camilo Cienfuegos", (LeaderCard)CardsWarehouse.RebelCards[0] },
        {"Ernesto Che Guevara", (LeaderCard)CardsWarehouse.RebelCards[1] },
        {"Eulogio Cantillo", (LeaderCard)CardsWarehouse.BatistaCards[0] },
        {"Francisco Tabernilla", (LeaderCard)CardsWarehouse.BatistaCards[1] }
    };

    public static Player Fidel => fidel == null ? SetPlayer(ref fidel, batista, Faction.Fidel) : fidel;
    public static Player Batista => batista == null ? SetPlayer(ref batista, fidel, Faction.Batista) : batista;
    public Faction PlayerFaction => playerFaction;
    public string Name { get; }

    public Dictionary<string, List<Card>> ListByName;
    public Dictionary<Zone, List<Card>> ListByZone;
    public Dictionary<List<Card>, Zone> ZoneByList;

    private Player(Faction faction)
    {
        Hand = Enumerable.Repeat<Card>(Utils.BaseCard, 10).ToList<Card>();
        playerFaction = faction;
        Name = faction is Faction.Fidel ? "Fidel" : "Batista";
        for (int i = 0; i < 10; i++)
        {
            emptySlotsInHand.Add(i);
        }
    }

    private static Player SetPlayer(ref Player player, Player enemy, Faction faction)
    {
        player = new Player(faction);
        player.Battlefield = new Battlefield(player);
        player.ListByName = new Dictionary<string, List<Card>> {
                                                                { $"{player.Name} Melee", player.Battlefield.Melee },
                                                                { $"{player.Name} Range", player.Battlefield.Range },
                                                                { $"{player.Name} Siege", player.Battlefield.Siege },
                                                                { $"{player.Name} Bonus", player.Battlefield.Bonus },
                                                                { $"Weather", Board.Instance.Weather },
                                                                { $"{player.Name} Hand", player.Hand }
                                                               };
        player.ListByZone = new Dictionary<Zone, List<Card>> {
                                                                { Zone.Melee, player.Battlefield.Melee },
                                                                { Zone.Range, player.Battlefield.Range },
                                                                { Zone.Siege, player.Battlefield.Siege }
                                                             };
        player.ZoneByList = new Dictionary<List<Card>, Zone> {
                                                                { player.Battlefield.Melee, Zone.Melee },
                                                                { player.Battlefield.Range, Zone.Range },
                                                                { player.Battlefield.Siege, Zone.Siege }
                                                             };
        if(!(enemy is null))
        {
            player.context = new Context(player, enemy);
            enemy.context = new Context(enemy, player);
        }
        return player;
    }
    #endregion

    #region Functions
    public bool GetCard(int cardsToSteal = 1)
    {
        if (Deck.Count <= cardsToSteal) //If there are no longer cards left in deck, this conditional shuffles
                                        //graveyard cards and add them to the deck so the game goes on
        {
            Utils.ShuffleList(Battlefield.Graveyard);

            foreach (var card in Battlefield.Graveyard)
            {
                card.AssignPosition(Deck);
                Deck.Add(card);
            }
            Battlefield.Graveyard.Clear();
        }

        while (emptySlotsInHand.Count < cardsToSteal) //if cardsToSteal is bigger than the amount of empty slots in players hand, this sends 
                                                      //a random card in deck to graveyard until a proper value for cardsToSteal is achieved
        {
            int index = new System.Random().Next(Deck.Count - 1);
            Battlefield.ToGraveyard(Deck[index], Deck);
            cardsToSteal--;
        }

        if (cardsToSteal == 0 || emptySlotsInHand.Count<cardsToSteal) return false;

        while (cardsToSteal> 0)
        {
            int index = Deck.Count - 1;
            AddToHand(Deck[index]);
            Board.Instance.Receive(new RemoveOperation(Deck[index], Deck, index, true));
            Deck.RemoveAt(index);
            cardsToSteal--;
        }

        return true;
    }


    public bool PlayCard(int originPosition, int targetPosition, Zone rangeType, out bool effectFailed)
    {
        effectFailed = false;
        if (targetPosition < 0 || originPosition < 0) return false;

        try
        {
            /* i'm sorry but i have to say it: i f***ing hate Unity. i don't know why, every time i load a compiled card, the context of the player 
             * that uses it is deleted and when trying to use the effect of the played card, the game breaks. this might be a hell of a patch
             * but I assure you i haven't found another way and I need this damn thing up and running. forgive my language, i've been looking
             * for this piece of sh*t error for a week and i was already freaking out. god bless you with a long live without having to use Unity <3
             */
            if (context is null) context = new Context(this, Board.Instance.GetCurrentEnemy());

            if (!(this.Hand[originPosition] is Card card) || card.Equals(Utils.BaseCard) || card is BaitCard || Board.Instance.ValidTurn) //in case of unexpected behaviours. bait cards will be played through their effect
            {
                return false;
            }

            if (card is WeatherCard weather && Board.Instance.Weather[targetPosition].Equals(Utils.BaseCard)) //play weather card
            {
                Board.Instance.Weather[targetPosition] = weather;
                weather.AssignPosition(Board.Instance.Weather);
                Board.Instance.Receive(new AddOperation(weather, Board.Instance.Weather, targetPosition));
            }
            else if (!this.Battlefield.AddCard(card, rangeType, targetPosition)) //play unit, clear and bonus card
            {
                return false;
            }
            EmptyHandAt(originPosition);
            effectFailed = !card.Effect(context.UpdatePlayerInstance(this.ListByZone[rangeType], card));

            Board.Instance.ValidTurn = true;
            Board.Instance.UpdateTotalDamage();
            UpdateEmptySlots();
            return true;
        }
        catch { return false; }
    }

    public void AddToHand(Card card)
    {
        if (emptySlotsInHand.Count == 0)
        {
            Battlefield.ToGraveyard(card, card.CurrentPosition);
            return;
        }

        Hand[emptySlotsInHand[0]] = card;
        card.AssignPosition(Hand);
        Board.Instance.Receive(new AddOperation(card, Hand, emptySlotsInHand[0]));
        emptySlotsInHand.RemoveAt(0);
    }

    public void EmptyHandAt(int index)
    {
        Board.Instance.Receive(new RemoveOperation(Hand[index], Hand, index));
        this.emptySlotsInHand.Add(index);
        this.Hand[index] = Utils.BaseCard;
    }

    public void UpdateEmptySlots()
    {
        emptySlotsInHand = new List<int>(10);
        for (int i = 0; i < Hand.Count; i++)
        {
            if (Hand[i].Equals(Utils.BaseCard)) emptySlotsInHand.Add(i);
        }
    }

    public static void Reset()
    {
        SetPlayer(ref fidel, batista, Faction.Fidel);
        SetPlayer(ref batista, fidel, Faction.Batista);
    }

    public override bool Equals(object other)
    {
        return other is Player otherPlayer && this.PlayerFaction == otherPlayer.PlayerFaction;
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    #endregion
}