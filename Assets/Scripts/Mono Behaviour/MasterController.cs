using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MasterController : MonoBehaviour
{
    public Camera[] cameras = new Camera[3]; //Camera[0] between rounds, Camera[1] fidel, Camera[2] batista

    #region Panels
    [SerializeField] GameObject playerPanel;
    [SerializeField] GameObject betweenRoundsPanel;
    [SerializeField] GameObject InfoPanel;
    [SerializeField] GameObject LeaderInfoPanel;
    [SerializeField] GameObject batistaVictory;
    [SerializeField] GameObject fidelVictory;
    [SerializeField] GameObject effectException;
    [SerializeField] GameObject generalException;
    [SerializeField] TMP_Text roundEndedNotification;
    [SerializeField] TMP_Text BatistaScoreInPlayersPanel;
    [SerializeField] TMP_Text FidelScoreInPlayersPanel;
    [SerializeField] TMP_Text BatistaScoreInBetweenRoundsPanel;
    [SerializeField] TMP_Text FidelScoreInBetweenRoundsPanel;
    Dictionary<string, (TMP_Text, TMP_Text)> score;
    GameObject panelOnWhenInformationDisplayed;
    #endregion

    public BoardMB board;

    public GameObject PanelOnWhenInformationDisplayed { get => panelOnWhenInformationDisplayed; private set => panelOnWhenInformationDisplayed = value; }
    bool isBatistaPlayingOrAboutToPlay => Board.Instance.IsBatistaPlayingOrAboutToPlay;
    bool validTurn => Board.Instance.ValidTurn;

    private void Start()
    {
        score = new Dictionary<string, (TMP_Text, TMP_Text)>
        {
            {"Fidel", (FidelScoreInBetweenRoundsPanel, FidelScoreInPlayersPanel) },
            {"Batista", (BatistaScoreInBetweenRoundsPanel, BatistaScoreInPlayersPanel) }
        };
    }

    public void EndTurn()
    {
        if (!board.EndTurn(board.CurrentPlayer)) return;

        cameras[1].gameObject.SetActive(false);
        cameras[2].gameObject.SetActive(false);
        UpdateScoreInText();
        playerPanel.SetActive(false);
        betweenRoundsPanel.SetActive(true);
        cameras[0].gameObject.SetActive(true);
    }

    public void EndRound()
    {
        if (!board.EndRound(board.CurrentPlayer)) return;

        EndTurn();
    }

    public void ReceiveTurn()
    {
        cameras[0].gameObject.SetActive(false);
        playerPanel.SetActive(true);
        betweenRoundsPanel.SetActive(false);

        if (isBatistaPlayingOrAboutToPlay && !Player.Batista.EndRound)
        {
            cameras[2].gameObject.SetActive(true);
            board.RecieveTurn(board.Batista);
        }
        else if (!Player.Fidel.EndRound)
        {
            cameras[1].gameObject.SetActive(true);
            board.RecieveTurn(board.Fidel);
        }
    }

    public void SavePanelOnWhenInformationDisplayed(bool leaveActive = true)
    {
        for (int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            if (this.gameObject.transform.GetChild(i).gameObject.activeInHierarchy)
            {
                this.PanelOnWhenInformationDisplayed = this.gameObject.transform.GetChild(i).gameObject;
                this.gameObject.transform.GetChild(i).gameObject.SetActive(leaveActive);
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
    public void OpenInfo(Sprite info, bool leader = false)
    {
        GameObject infoPanel = leader ? LeaderInfoPanel : InfoPanel;
        infoPanel.GetComponent<Image>().sprite = info;
        infoPanel.SetActive(true);
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
        if (playerName == "")
        {
            UpdateScoreInText("Fidel");
            playerName = "Batista";
        }

        damage = Convert.ToInt32(board.GetMBPlayerByName[playerName].player.TotalDamage);
        score[playerName].Item1.text = score[playerName].Item2.text = $"{playerName}: " + ((damage < 10) ? "0" + damage.ToString() : damage.ToString());
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
        if (board.board.CheckIfEnded(out string winner))
        {
            EndGame(winner);
            return true;
        }
        return false;
    }
}
