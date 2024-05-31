using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board
{
    #region Fields, properties and builder
    int roundCount = 0;
    bool newRound = true;
    public List<Card> Weather = Enumerable.Repeat<Card>(Utils.BaseCard, 3).ToList<Card>();
    public bool AlmeidaIsPlayed = false;
    private static Board instance;

    public bool IsBatistaPlayingOrAboutToPlay { get; private set; }
    public static Board Instance => instance.Equals(null) ? SetBoard() : instance;
    public int RoundCount { get => roundCount; private set => roundCount = value; }
    private Board() { }

    private static Board SetBoard()
    {
        instance = new Board();
        return instance;
    }
    #endregion

    #region Functions
    public void StartRoundIfNecesary()
    {
        if (newRound)
        {
            if (RoundCount == 0)
            {
                Player.Fidel.GetCard(10);
                Player.Batista.GetCard(10);
            }
            else
            {
                Player.Fidel.GetCard(2);
                Player.Batista.GetCard(2);
                if (Player.Fidel.Leader.stealCardLeader) Player.Fidel.leaderEffectUsedThisRound = false;
                if (Player.Batista.Leader.stealCardLeader) Player.Batista.leaderEffectUsedThisRound = false;
            }

            RoundCount++;
            newRound = false;
        }
    }

    public void UpdateTotalDamage(Player player = null)
    {
        if(player.Equals(null))
        {
            UpdateTotalDamage(Player.Batista);
            player = Player.Fidel;
        }

        player.TotalDamage = 0;
        double meleeBonus = (player.Battlefield.Bonus[0] is BonusCard) ? ((BonusCard)player.Battlefield.Bonus[0]).increase : 1;
        double rangeBonus = (player.Battlefield.Bonus[1] is BonusCard) ? ((BonusCard)player.Battlefield.Bonus[1]).increase : 1;
        double siegeBonus = (player.Battlefield.Bonus[2] is BonusCard) ? ((BonusCard)player.Battlefield.Bonus[2]).increase : 1;

        foreach (Card card in this.Weather) //applies weather effects
        {
            if (card is WeatherCard weather) weather.Effect(this);
        }

        foreach (Card item in player.Battlefield.Melee) //sums the damage of the unit melee cards
        {
            if (item is UnitCard unit)
            {
                player.TotalDamage += unit.level == Level.Golden ? unit.Damage : meleeBonus * unit.Damage;
                unit.ResetDamage();
            }
        }

        foreach (Card item in player.Battlefield.Range) //sums the damage of the unit range cards
        {
            if (item is UnitCard unit)
            {
                player.TotalDamage += unit.level == Level.Golden ? unit.Damage : rangeBonus * unit.Damage;
                unit.ResetDamage();
            }
        }

        foreach (Card item in player.Battlefield.Siege) //sums the damage of the unit siege cards
        {
            if (item is UnitCard unit)
            {
                player.TotalDamage += unit.level == Level.Golden ? unit.Damage : siegeBonus * unit.Damage;
                unit.ResetDamage();
            }
        }
    }

    public bool CheckNextRound(out string winner)
    {
        if (Player.Fidel.EndRound && Player.Batista.EndRound)
        {

            if (Player.Fidel.TotalDamage > Player.Batista.TotalDamage || (this.AlmeidaIsPlayed && Player.Batista.TotalDamage - Player.Fidel.TotalDamage <= 3)) //fidel wins
            {
                SumScore(Player.Fidel, 2, Player.Batista, 0);
                IsBatistaPlayingOrAboutToPlay = false;
                winner = "Fidel";
            }
            else if (Player.Fidel.TotalDamage < Player.Batista.TotalDamage) //batista wins
            {
                SumScore(Player.Batista, 2, Player.Fidel, 0);
                IsBatistaPlayingOrAboutToPlay = true;
                winner = "Batista";
            }
            else //draw
            {
                SumScore(Player.Fidel.StartedPlaying ? Player.Batista : Player.Fidel, 1, Player.Batista.StartedPlaying ? Player.Fidel : Player.Batista, 1);
                IsBatistaPlayingOrAboutToPlay = Player.Batista.StartedPlaying;
                winner = "";
            }

            Player.Fidel.Battlefield.Clear();
            Player.Batista.Battlefield.Clear();
            Player.Fidel.EndRound = false;
            Player.Batista.EndRound = false;
            newRound = true;

            if (!Player.Fidel.Battlefield.StaysInBattlefieldIs("Almeida")) //checks if almeida will stay in battlefield to modify its boolean
            {
                this.AlmeidaIsPlayed = false;
            }

            return true;
        }

        winner = "";
        return false;
    }

    public Player GetCurrentPlayer() => IsBatistaPlayingOrAboutToPlay ? Player.Batista : Player.Fidel;
    public Player GetCurrentEnemy() => IsBatistaPlayingOrAboutToPlay ? Player.Fidel : Player.Batista;

    private void SumScore (Player winner, int scoreWinner, Player looser, int scoreLooser)
    {
        winner.Score += scoreWinner;
        winner.StartedPlaying = true;
        looser.Score += scoreLooser;
        looser.StartedPlaying = false;
    }
    #endregion
}