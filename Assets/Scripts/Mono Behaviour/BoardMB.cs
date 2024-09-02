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
    public List<GameObject> list;
    public Dictionary<string, PlayerMB> GetMBPlayerByName { get; private set; }
    public Dictionary<Player, PlayerMB> GetMBPlayer { get; private set; }

    public PlayerMB CurrentPlayer => GetMBPlayer[board.GetCurrentPlayer()];

    public void Start()
    {
        ZonesList = new Dictionary<string, GameObject>();
        list = new List<GameObject>();
        foreach(string item in Utils.ZonesName)
        {
            ZonesList.Add(item, GameObject.Find(item));
            list.Add(GameObject.Find(item));
        }
        Weather = ZonesList["Weather"];
        GetMBPlayerByName = new Dictionary<string, PlayerMB>() { { "Fidel", Fidel }, { "Batista", Batista } };
        GetMBPlayer = new Dictionary<Player, PlayerMB>() { { Player.Fidel, Fidel }, { Player.Batista, Batista } };

        board = Board.Instance;
    }
    #endregion

    #region ViewModification
    public void UpdateView(bool alsoUpdateTexts = false, bool playing = true)
    {
        for (int i = 0; i < Utils.ZonesName.Length; i++)
        {
            if (Utils.ZonesName[i].Contains("Weather")) continue;
            else if (Utils.ZonesName[i].Contains("Bonus")) CardsInBoardViewModificator(ZonesList[Utils.ZonesName[i]],
                                                           board.ZonesList[Utils.ZonesName[i]],
                                                           3, // available slots
                                                           Utils.ZonesName[i].Contains("Batista") ? board.IsBatistaPlayingOrAboutToPlay :
                                                                                                   !board.IsBatistaPlayingOrAboutToPlay);
            else CardsInBoardViewModificator(ZonesList[Utils.ZonesName[i]],
                                        board.ZonesList[Utils.ZonesName[i]],
                                        5, // available slots
                                        Utils.ZonesName[i].Contains("Batista") ? board.IsBatistaPlayingOrAboutToPlay :
                                                                                !board.IsBatistaPlayingOrAboutToPlay);
        }

        CardsInBoardViewModificator(ZonesList["Weather"], Board.Instance.Weather, 3, playing);
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
                child.GetComponent<CardController>().Desoccupy();
                child.SetActive(setActiveOutOfCount);
            }
            else child.GetComponent<CardController>().Occupy(list[i]);
        }
    }
    public void ModifyPlayableSlots(PlayerMB player, bool activate = false)
    {
        foreach (GameObject item in player.AvailableSlots)
        {
            if (item.name.Contains("Hand")) continue;

            for (int i = 0; i < item.transform.childCount; i++)
            {
                if (item.transform.GetChild(i).gameObject.tag == "LeaderCard") continue;

                if (player.player.ListByName[item.name][i].Equals(Utils.BaseCard))
                {
                    item.transform.GetChild(i).gameObject.SetActive(activate);
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

        ModifyPlayableSlots(player);

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
                ModifyPlayableSlots(GetMBPlayerByName[winner]);
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
            ModifyVisibility(Batista.Body, true);
            ModifyVisibility(Fidel.Body, true);
            UpdateView(true, false);
        }
    }

    public void UndoTurn()
    {
        Board.Instance.Undo();
        UpdateView(true);
    }
    #endregion

    public void ModifyVisibility(GameObject[] collection, bool visibility)
    {
        foreach (GameObject item in collection)
        {
            item.SetActive(visibility);
        }
    }
}
