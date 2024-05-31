using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MasterController : MonoBehaviour
{
    public Camera[] cameras = new Camera[3]; //Camera[0] between rounds, Camera[1] fidel, Camera[2] batista,
    public GameObject playerPanel;
    public GameObject betweenRoundsPanel;
    public GameObject InfoPanel;
    public GameObject LeaderInfoPanel;
    public Dictionary<string, GameObject> ZonesList;
    [SerializeField] GameObject batistaVictory;
    [SerializeField] GameObject fidelVictory;
    [SerializeField] GameObject effectException;
    [SerializeField] GameObject generalException;
    [SerializeField] TMP_Text roundEndedNotification;
    private GameObject[] fidelBody;
    private GameObject[] batistaBody;
    private GameObject[] fidelAvailableSlots;
    private GameObject[] batistaAvailableSlots;
    private GameObject weather;
    public Board board;
    public bool isBatistaPlayingOrAboutToPlay = true;
    public bool validTurn = false;
    public TMP_Text BatistaScoreInPlayersPanel;
    public TMP_Text FidelScoreInPlayersPanel;
    public TMP_Text BatistaScoreInBetweenRoundsPanel;
    public TMP_Text FidelScoreInBetweenRoundsPanel;
    GameObject panelOnWhenInformationDisplayed;

    public GameObject PanelOnWhenInformationDisplayed { get => panelOnWhenInformationDisplayed; set => panelOnWhenInformationDisplayed = value; }

    private void Start()
    {
        fidelBody = GameObject.FindGameObjectsWithTag("Fidel Body");
        batistaBody = GameObject.FindGameObjectsWithTag("Batista Body");
        fidelAvailableSlots = GameObject.FindGameObjectsWithTag("Fidel");
        batistaAvailableSlots = GameObject.FindGameObjectsWithTag("Batista");
        weather = GameObject.FindGameObjectWithTag("Weather");
        ZonesList = new Dictionary<string, GameObject>
        {
            {"Weather",  GameObject.Find("Weather")},
            {"Batista Bonus",  GameObject.Find("Batista Bonus")},
            {"Batista Melee",  GameObject.Find("Batista Melee")},
            {"Batista Range",  GameObject.Find("Batista Range")},
            {"Batista Siege",  GameObject.Find("Batista Siege")},
            {"Fidel Bonus",  GameObject.Find("Fidel Bonus")},
            {"Fidel Melee",  GameObject.Find("Fidel Melee")},
            {"Fidel Range",  GameObject.Find("Fidel Range")},
            {"Fidel Siege",  GameObject.Find("Fidel Siege")},

        };
    }

    public void EndTurn()
    {
        if (!validTurn) return;
        validTurn = false;

        if (isBatistaPlayingOrAboutToPlay)
        {
            cameras[2].gameObject.SetActive(false);
            isBatistaPlayingOrAboutToPlay = board.Fidel.EndRound? true : false;
            playerPanel.SetActive(false);
            betweenRoundsPanel.SetActive(true);
            UpdateScoreInText("Batista");
            DeactivatePlayableSlots(board.Batista);

            foreach (GameObject item in batistaBody)
            {
                item.SetActive(true);
            }
        }
        else
        {
            cameras[1].gameObject.SetActive(false);
            isBatistaPlayingOrAboutToPlay = board.Batista.EndRound? false : true;
            playerPanel.SetActive(false);
            betweenRoundsPanel.SetActive(true);
            UpdateScoreInText("Fidel");
            DeactivatePlayableSlots(board.Fidel);

            foreach (GameObject item in fidelBody)
            {
                item.SetActive(true);
            }
        }

        for (int i = 0; i < weather.transform.childCount; i++)
        {
            if (board.Weather[i].Equals(Utils.BaseCard))
            {
                weather.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        cameras[0].gameObject.SetActive(true);
    }

    public void EndRound()
    {
        if (validTurn) return;
        if (isBatistaPlayingOrAboutToPlay) board.Batista.EndRound = true;
        else board.Fidel.EndRound = true;
        validTurn = true;
        EndTurn();
        board.CheckNextRound();
    }

    public void ReceiveTurn()
    {
        board.StartRoundIfNecesary();
        cameras[0].gameObject.SetActive(false);
        playerPanel.SetActive(true);
        betweenRoundsPanel.SetActive(false);
        board.UpdateView();

        if (isBatistaPlayingOrAboutToPlay && !board.Batista.EndRound)
        {
            cameras[2].gameObject.SetActive(true);
            foreach (GameObject item in batistaBody)
            {
                item.SetActive(false);
            }
        }
        else if (!board.Fidel.EndRound)
        {
            cameras[1].gameObject.SetActive(true);
            foreach (GameObject item in fidelBody)
            {
                item.SetActive(false);
            }
        }
    }

    public void SavePanelOnWhenInformationDisplayed()
    {
        for (int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            if (this.gameObject.transform.GetChild(i).gameObject.activeInHierarchy)
            {
                this.PanelOnWhenInformationDisplayed = this.gameObject.transform.GetChild(i).gameObject;
                break;
            }
        }
    }
    public void EffectException()
    {
        SavePanelOnWhenInformationDisplayed();
        PanelOnWhenInformationDisplayed.SetActive(false);
        effectException.SetActive(true);
    }
    public void GeneralException()
    {
        SavePanelOnWhenInformationDisplayed();
        PanelOnWhenInformationDisplayed.SetActive(false);
        generalException.SetActive(true);
    }
    public void CloseInfo()
    {
        PanelOnWhenInformationDisplayed.SetActive(true);
    }
    public void RoundEndedNotification(string winnerName = "")
    {
        if(CheckIfEnded()) return;

        switch (winnerName)
        {
            case "Fidel":
                roundEndedNotification.text = "Fidel gana la ronda.";
                break;

            case "Batista":
                roundEndedNotification.text = "Batista gana la ronda.";
                break;

            default:
                roundEndedNotification.text = "La ronda termina en empate.";
                break;
        }

        roundEndedNotification.gameObject.transform.parent.gameObject.SetActive(true);
    }
    public void UpdateScoreInText(string playerName = "")
    {
        int damage = 0;

        switch (playerName)
        {
            case "Fidel":
                damage = Convert.ToInt32(board.TotalDamage(board.Fidel));
                FidelScoreInBetweenRoundsPanel.text = "Fidel: " + ((damage < 10) ? "0" + damage.ToString() : damage.ToString());
                FidelScoreInPlayersPanel.text = FidelScoreInBetweenRoundsPanel.text;
                break;

            case "Batista":
                damage = Convert.ToInt32(board.TotalDamage(board.Batista));
                BatistaScoreInBetweenRoundsPanel.text = "Batista: " + ((damage < 10) ? "0" + damage.ToString() : damage.ToString());
                BatistaScoreInPlayersPanel.text = BatistaScoreInBetweenRoundsPanel.text;
                break;

            default:
                UpdateScoreInText("Fidel");
                UpdateScoreInText("Batista");
                break;
        }
    }
    public bool IsAnyInfoActive()
    {
        for (int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            GameObject checkingChild = this.gameObject.transform.GetChild(i).gameObject;
            if (checkingChild.activeInHierarchy && !(checkingChild.name == "Between Rounds panel" || checkingChild.name == "Players panel")) return true;
        }

        return false;
    }
    public void DeactivatePlayableSlots(Player player)
    {
        GameObject[] collection = player.Equals(board.Batista) ? batistaAvailableSlots : fidelAvailableSlots;

        foreach (GameObject item in collection)
        {
            for (int i = 0; i < item.transform.childCount; i++)
            {
                if (item.transform.GetChild(i).gameObject.tag == "LeaderCard") continue;

                if (Utils.GetListByName(player, item.transform.GetChild(i).gameObject.name)[i].Equals(Utils.BaseCard))
                {
                    item.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }
    public void EndGame(string winnerName)
    {
        this.betweenRoundsPanel.SetActive(false);

        switch (winnerName)
        {
            case "Fidel":
                this.fidelVictory.SetActive(true);
                break;

            case "Batista":
                this.batistaVictory.SetActive(true);
                break;

            default:
                BackToMainMenu();
                break;
        }
    }
    public void BackToMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public bool IsPlayersPanelActive() => this.playerPanel.activeInHierarchy;

    private bool CheckIfEnded()
    {
        if (board.RoundCount >= 2)
        {
            if (board.Fidel.Score>=4 && board.Fidel.Score > board.Batista.Score)
            {
                EndGame("Fidel");
                return true;
            }
            else if (board.Batista.Score >= 4 && board.Batista.Score > board.Fidel.Score)
            {
                EndGame("Batista");
                return true;
            }

        }
        return false;
    }
}
