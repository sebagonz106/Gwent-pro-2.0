using System.Collections;
using System.Collections.Generic;
using Gwent_AI;
using UnityEngine;
using TMPro;

public class AIController : MonoBehaviour
{
    [SerializeField] GameObject playedNotification;
    [SerializeField] TMP_Text playedNotificationText;
    [SerializeField] MasterController masterController;
    public PlayerMB Player { get; private set; }
    public PlayerMB Enemy { get; private set; }
    public bool Initialized { get; private set; }
    GwentAI aI;
    AIContext context;

    private void Awake()
    {
        Initialized = false;
    }

    public void Initialize()
    {
        Initialized = true;
        Player = masterController.board.GetMBPlayerByName[PlayerPrefs.GetString("AI")];
        PlayerPrefs.SetString("AI", "");
        Enemy = masterController.board.GetMBPlayer[Utils.GetEnemyOf(Player.player)];
        aI = new GwentAI(Player.Name);
        context = new AIContext(Player.player);
        masterController.board.ModifyPlayableSlots(Player);
    }

    public void StartRound()
    {
        if (!Initialized) Initialize();
        masterController.OpenPlayer(Enemy.Name);
        masterController.board.ModifyVisibility(Enemy.Body, false);
        masterController.board.ModifyPlayableSlots(Player);
        masterController.board.ModifyPlayableSlots(Enemy, true);

        if (Board.Instance.GetCurrentPlayer().Equals(Player.player))
        {
            masterController.board.RecieveTurn(Player);
            DirectPlay();
        }
        else masterController.board.RecieveTurn(Enemy);
    }

    public void EndTurn()
    {
        if (!Board.Instance.EndTurn(Board.Instance.GetCurrentPlayer())) return;

        if (!Player.player.EndRound && masterController.board.CurrentPlayer.Equals(Player)) DirectPlay();
        else if (!Enemy.player.EndRound) masterController.board.ModifyPlayableSlots(Enemy, true);
    }

    public void EndRound(bool aI = false)
    {
        if (aI) Board.Instance.ValidTurn = false;
        if (!Board.Instance.EndRound()) return;
        if (!aI) CheckNext();
    }

    public void BackToBetweenRounds()
    {
        masterController.OpenBetweenRounds();
        masterController.board.CheckNextRound();
        masterController.board.ModifyPlayableSlots(Player);
        masterController.board.ModifyPlayableSlots(Enemy);
    }

    public void CheckNext()
    {
        if (Player.player.EndRound && Enemy.player.EndRound) BackToBetweenRounds();
        else EndTurn();
    }

    public void ClosePlayedNotifiaction()
    {
        playedNotification.SetActive(false);
        CheckNext();
    }

    public void Play()
    {
        if (this.Player.player.EndRound) return;

        Player player = this.Player.player;
        (Card,Zone,Card) value = aI.Play(context, out bool leaderEffect);

        Card card = value.Item1;
        Zone zone = value.Item2;
        Card target = value.Item3;

        if (leaderEffect) player.Leader.Effect(player.context.UpdatePlayerInstance(target.CurrentPosition, target));
        else if (card is BaitCard bait) bait.Effect(player.context.UpdatePlayerInstance(target.CurrentPosition, target));
        else if (!(card is null)) player.PlayCard(player.Hand.IndexOf(card), player.ListByZone[zone].IndexOf(Utils.BaseCard), zone, out bool effectFailed);
        else player.EndRound = true;

        playedNotification.SetActive(true);
    }

    public void DirectPlay()
    {
        string info = "";
        if (this.Player.player.EndRound) return;
        else if (!aI.DirectPlay(context, out info)) EndRound(true);
        else Board.Instance.ValidTurn = true;

        playedNotificationText.text = info;
        playedNotification.SetActive(true);
        masterController.board.UpdateView(true);
        masterController.board.ModifyPlayableSlots(Player);
    }
}
