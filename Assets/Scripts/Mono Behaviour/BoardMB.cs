using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardMB : MonoBehaviour
{
    public PlayerMB Fidel;
    public PlayerMB Batista;
    public GameObject Weather;
    public Canvas gameController;
    public MasterController masterController => (MasterController)gameController.GetComponent("MasterController");
    public Dictionary<string, GameObject> ZonesList;

    public void Start()
    {
        ZonesList = new Dictionary<string, GameObject>();
        foreach(string item in Utils.ZonesName)
        {
            ZonesList.Add(item, GameObject.Find(item));
        }
        Weather = ZonesList["Weather"];
    }
    public void UpdateView( bool alsoUpdateTexts = false)
    {
        for (int i = 0; i < Utils.ZonesName.Length; i++)
        {
            if (Utils.ZonesName[i].Contains("Weather") || Utils.ZonesName[i].Contains("Bonus")) continue;

            CardsInBoardViewModificator(ZonesList[Utils.ZonesName[i]],
                                        Board.Instance.ZonesList[Utils.ZonesName[i]],
                                        5, // available slots
                                        Utils.ZonesName[i].Contains("Batista") ? Board.Instance.IsBatistaPlayingOrAboutToPlay :
                                                                                !Board.Instance.IsBatistaPlayingOrAboutToPlay);
        }

        CardsInBoardViewModificator(ZonesList["Weather"], Board.Instance.Weather, 3);
        CardsInBoardViewModificator(Batista.Hand, Player.Batista.Hand, 10, false);
        CardsInBoardViewModificator(Fidel.Hand, Player.Fidel.Hand, 10, false);

        if(alsoUpdateTexts) this.masterController.UpdateScoreInText();
    }

    public void CheckNextRound()
    {
        if (Board.Instance.CheckNextRound(out string winner))
        {
            Fidel.battlefield.Clear();
            Batista.battlefield.Clear();
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

    void CardsInBoardViewModificator (GameObject gameObjectsList, List<Card> list, int maxCapacity, bool setActiveOutOfCount = true)
    {
        GameObject child;

        for (int i = 0; i < maxCapacity; i++)
        {
            child = gameObjectsList.transform.GetChild(i).gameObject;

            if (list[i].Name == Utils.BaseCard.Name)
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
                if (name.Contains("Hand")) child.GetComponent<CardController>().AssignRangeForHandCard(list[i].AvailableRange);
            }
        }
    }
}
