using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardMB : MonoBehaviour
{
    #region Fields and Start
    public Board board;
    public PlayerMB Fidel;
    public PlayerMB Batista;
    public GameObject Weather;
    public Canvas gameController;
    public MasterController masterController => (MasterController)gameController.GetComponent("MasterController");
    public Dictionary<string, GameObject> ZonesList { get; private set; }
    public Dictionary<string, PlayerMB> GetMBPlayerByName { get; private set; }
    public Dictionary<Player, PlayerMB> GetMBPlayer { get; private set; }

    public PlayerMB CurrentPlayer => GetMBPlayer[board.GetCurrentPlayer()];

    public void Start()
    {
        ZonesList = new Dictionary<string, GameObject>();
        foreach(string item in Utils.ZonesName)
        {
            ZonesList.Add(item, GameObject.Find(item));
        }
        Weather = ZonesList["Weather"];
        GetMBPlayerByName = new Dictionary<string, PlayerMB>() { { "Fidel", Fidel }, { "Batista", Batista } };
        GetMBPlayer = new Dictionary<Player, PlayerMB>() { { Player.Fidel, Fidel }, { Player.Batista, Batista } };

        board = Board.Instance;
    }
    #endregion

    #region ViewModification
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
    void CardsInBoardViewModificator(GameObject gameObjectsList, List<Card> list, int maxCapacity, bool setActiveOutOfCount = true)
    {
        GameObject child;

        for (int i = 0; i < maxCapacity; i++)
        {
            child = gameObjectsList.transform.GetChild(i).gameObject;

            if (list[i].Name == Utils.BaseCard.Name)
            {
                child.GetComponent<Renderer>().material = Utils.BaseCard.Info.Material;
                child.GetComponent<CardController>().IsOccupied = false;
                child.SetActive(setActiveOutOfCount);
            }
            else
            {
                child.SetActive(true);
                child.GetComponent<Renderer>().material = list[i].Info.Material;
                child.GetComponent<CardController>().Info = list[i].Info.Information;
                if (name.Contains("Hand")) child.GetComponent<CardController>().AssignRangeForHandCard(list[i].AvailableRange);
            }
        }
    }
    public void DeactivatePlayableSlots(PlayerMB player)
    {
        foreach (GameObject item in player.AvailableSlots)
        {
            for (int i = 0; i < item.transform.childCount; i++)
            {
                if (item.transform.GetChild(i).gameObject.tag == "LeaderCard") continue;

                if (player.player.ListByName[item.name][i].Equals(Utils.BaseCard))
                {
                    item.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }
    #endregion

    #region Turns Control
    public void RecieveTurn(PlayerMB player)
    {
        board.StartRoundIfNecesary();
        UpdateView();
        ModifyVisibility(player.Body, false);
    }

    public bool EndTurn(PlayerMB player)
    {
        if(!board.EndTurn(player.player)) return false;

        DeactivatePlayableSlots(player);

        for (int i = 0; i < Weather.transform.childCount; i++)
        {
            if (board.Weather[i].Equals(Utils.BaseCard))
            {
                Weather.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        ModifyVisibility(player.Body, true);

        return true;
    }

    public bool EndRound(PlayerMB player)
    {
        if (!board.EndRound(player.player)) return false;
        CheckNextRound();
        return true;
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
                DeactivatePlayableSlots(GetMBPlayerByName[winner]);
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
            UpdateView(true);
        }
    }

    #endregion

    void ModifyVisibility(GameObject[] collection, bool visibility)
    {
        foreach (GameObject item in collection)
        {
            item.SetActive(visibility);
        }
    }
}
