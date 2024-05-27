using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardMB : MonoBehaviour
{
    PlayerMB Fidel;
    PlayerMB Batista;
    public Canvas gameController;
    public MasterController masterController => (MasterController)gameController.GetComponent("MasterController");
    //declarar un arreglo que contenga los nombres de las filas y un diccionario que de la fila en forrma de lista y en forma de game object para el update view

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

    public void CheckNextRound()
    {
        if (Board.Instance.CheckNextRound(out string winner))
        {
            if (winner.Length == 0) this.masterController.RoundEndedNotification();
            else
            {
                this.masterController.RoundEndedNotification(winner);
                this.masterController.DeactivatePlayableSlots(Utils.GetEnemyByName(winner));
            }

            #region Activating Coins
            if (Player.Fidel.Score >= 4)
            {
                Fidel.WonCoin2.SetActive(true);
            }
            else if (Player.Fidel.Score >= 2)
            {
                Fidel.WonCoin1.SetActive(true);
            }

            if (Player.Batista.Score >= 4)
            {
                Batista.WonCoin2.SetActive(true);
            }
            else if (Player.Batista.Score >= 2)
            {
                Batista.WonCoin1.SetActive(true);
            }
            #endregion

            Board.Instance.UpdateTotalDamage();
            this.masterController.UpdateScoreInText();
            UpdateView();
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
                child.GetComponent<Renderer>().material = Utils.BaseCard.material;
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
}
