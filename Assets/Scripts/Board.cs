using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Player Fidel;
    public Player Batista;
    public Canvas gameController;
    public MasterController masterController => (MasterController)gameController.GetComponent("MasterController");
    int roundCount = 0;
    bool newRound = true;
    public List<Card> Weather = new List<Card>(3);
    public bool AlmeidaIsPlayed = false;
    Material emptySlot;

    public int RoundCount { get => roundCount; private set => roundCount = value; }
    public Material EmptySlot { get => emptySlot; set => emptySlot = value; }

    private void Awake()
    {
        this.Weather.AddRange(Enumerable.Repeat<Card>(Utils.BaseCard, 3));
        this.emptySlot = Resources.Load<Material>("DiselectedBattlefieldCard");
    }
    public void UpdateView( bool alsoUpdateTexts = false)
    {
        CardsInBoardViewModificator("Weather", this.Weather, 3);
        CardsInBoardViewModificator("Batista Bonus", this.Batista.Battlefield.Bonus, 3, masterController.isBatistaPlayingOrAboutToPlay);
        CardsInBoardViewModificator("Fidel Bonus", this.Fidel.Battlefield.Bonus, 3, !masterController.isBatistaPlayingOrAboutToPlay);
        CardsInBoardViewModificator("Batista Hand", this.Batista.Hand, 10, false);
        CardsInBoardViewModificator("Fidel Hand", this.Fidel.Hand, 10, false);
        CardsInBoardViewModificator("Batista Melee", this.Batista.Battlefield.Melee, 5, masterController.isBatistaPlayingOrAboutToPlay); 
        CardsInBoardViewModificator("Batista Range", this.Batista.Battlefield.Range, 5, masterController.isBatistaPlayingOrAboutToPlay);
        CardsInBoardViewModificator("Batista Siege", this.Batista.Battlefield.Siege, 5, masterController.isBatistaPlayingOrAboutToPlay); 
        CardsInBoardViewModificator("Fidel Melee", this.Fidel.Battlefield.Melee, 5, !masterController.isBatistaPlayingOrAboutToPlay);
        CardsInBoardViewModificator("Fidel Range", this.Fidel.Battlefield.Range, 5, !masterController.isBatistaPlayingOrAboutToPlay); 
        CardsInBoardViewModificator("Fidel Siege", this.Fidel.Battlefield.Siege, 5, !masterController.isBatistaPlayingOrAboutToPlay);
        if(alsoUpdateTexts) this.masterController.UpdateScoreInText();
    }

    public void StartRoundIfNecesary()
    {
        if (newRound)
        {
            if (RoundCount == 0)
            {
                Fidel.GetCard(10);
                Batista.GetCard(10);
            }
            else
            {
                Fidel.GetCard(2);
                Batista.GetCard(2);
                if (this.Fidel.Leader.stealCardLeader) this.Fidel.leaderEffectUsedThisRound = false;
                if (this.Batista.Leader.stealCardLeader) this.Batista.leaderEffectUsedThisRound = false;
            }

            RoundCount++;
            newRound = false;
        }
    }

    public double TotalDamage(Player player)
    {
        double totalDamage = 0;
        double meleeBonus = (player.Battlefield.Bonus[0] is BonusCard)? ((BonusCard)player.Battlefield.Bonus[0]).increase : 1;
        double rangeBonus = (player.Battlefield.Bonus[1] is BonusCard)? ((BonusCard)player.Battlefield.Bonus[1]).increase : 1;
        double siegeBonus = (player.Battlefield.Bonus[2] is BonusCard)? ((BonusCard)player.Battlefield.Bonus[2]).increase : 1;

        foreach (Card card in this.Weather) //applies weather effects
        {
            if(card is WeatherCard weather) weather.Effect(this);
        }

        foreach (Card item in player.Battlefield.Melee) //sums the damage of the unit melee cards
        {
            if (item is UnitCard unit)
            {
                totalDamage += unit.level==Level.Golden? unit.Damage : meleeBonus * unit.Damage;
                unit.ResetDamage();
            }
        }

        foreach (Card item in player.Battlefield.Range) //sums the damage of the unit range cards
        {
            if (item is UnitCard unit)
            {
                totalDamage += unit.level == Level.Golden ? unit.Damage : rangeBonus * unit.Damage;
                unit.ResetDamage();
            }
        }

        foreach (Card item in player.Battlefield.Siege) //sums the damage of the unit siege cards
        {
            if (item is UnitCard unit)
            {
                totalDamage += unit.level == Level.Golden ? unit.Damage : siegeBonus * unit.Damage;
                unit.ResetDamage();
            }
        }

        return totalDamage;
    }

    public void CheckNextRound()
    {
        if (this.Fidel.EndRound && this.Batista.EndRound)
        {
            double fidelTotalDamage = TotalDamage(this.Fidel);
            double batistaTotalDamage = TotalDamage(this.Batista);

            if (fidelTotalDamage > batistaTotalDamage || (this.AlmeidaIsPlayed && batistaTotalDamage - fidelTotalDamage <= 3)) //fidel wins
            {
                this.Fidel.Score += 2;
                this.Batista.StartedPlaying = false;
                this.Fidel.StartedPlaying = true;
                this.masterController.isBatistaPlayingOrAboutToPlay = false;
                this.masterController.DeactivatePlayableSlots(this.Batista);
                this.masterController.RoundEndedNotification("Fidel");
            }
            else if (fidelTotalDamage < batistaTotalDamage) //batista wins
            {
                this.Batista.Score += 2;
                this.Batista.StartedPlaying = true;
                this.Fidel.StartedPlaying = false;
                this.masterController.isBatistaPlayingOrAboutToPlay = true;
                this.masterController.DeactivatePlayableSlots(this.Fidel);
                this.masterController.RoundEndedNotification("Batista");
            }
            else //draw
            {
                this.Fidel.Score += 1;
                this.Batista.Score += 1;

                this.Batista.StartedPlaying = !this.Batista.StartedPlaying; //if tied and Batista started playing last round, next round fidel will start
                this.Fidel.StartedPlaying = !this.Fidel.StartedPlaying; //if tied and Fidel started playing last round, next round Batista will start
                this.masterController.isBatistaPlayingOrAboutToPlay = this.Batista.StartedPlaying;
                this.masterController.RoundEndedNotification();
            }

            if (this.Fidel.Score >= 4)
            {
                this.Fidel.WonCoin2.SetActive(true);
            }
            else if (this.Fidel.Score >= 2)
            {
                this.Fidel.WonCoin1.SetActive(true);
            }

            if (this.Batista.Score >= 4)
            {
                this.Batista.WonCoin2.SetActive(true);
            }
            else if (this.Batista.Score >= 2)
            {
                this.Batista.WonCoin1.SetActive(true);
            }

            this.Fidel.Battlefield.Clear();
            this.Batista.Battlefield.Clear();
            this.masterController.UpdateScoreInText();
            UpdateView();
            this.Fidel.EndRound = false;
            this.Batista.EndRound = false;
            this.newRound = true;
            if (!this.Fidel.Battlefield.StaysInBattlefieldIs("Almeida")) //checks if almeida will stay in battlefield to modify its boolean
            {
                this.AlmeidaIsPlayed = false;
            }
        }
    }
    void CardsInBoardViewModificator(string name, List<Card> list, int maxCapacity, bool setActiveOutOfCount = true)
    {
        GameObject gameObjectsList = GameObject.Find(name);
        GameObject child;

        for (int i = 0; i < maxCapacity; i++)
        {
            child = gameObjectsList.transform.GetChild(i).gameObject;

            if (list[i].name == Utils.BaseCard.name)
            {
                child.GetComponent<Renderer>().material = emptySlot;
                child.GetComponent<CardController>().IsOccupied = false;
                child.SetActive(setActiveOutOfCount);
            }
            else
            {
                child.SetActive(true);
                child.GetComponent<Renderer>().material = list[i].material;
                child.GetComponent<CardController>().Info = list[i].information;
                if (name.Contains("Hand")) child.GetComponent<CardController>().AssignRangeForHandCard(list[i].availableRange);
            }
        }
    }

    public Player GetCurrentPlayer() => this.masterController.isBatistaPlayingOrAboutToPlay ? this.Batista : this.Fidel;
}
