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

    public void Initialize(PlayerMB player)
    {
        Initialized = true;
        Player = player;
        Enemy = masterController.board.GetMBPlayer[Utils.GetEnemyOf(Player.player)];
        aI = new GwentAI(player.Name);
        context = new AIContext(player.player);
        masterController.board.DeactivatePlayableSlots(Player);
    }

    public void StartRound()
    {
        if (Board.Instance.GetCurrentPlayer().Equals(Player.player))
        {
            DirectPlay();
        }
    }

    public void EndTurn()
    {
        if (!Board.Instance.EndTurn(Utils.GetEnemyOf(Player.player))) return;
        masterController.board.UpdateView(true);
        StartRound();
    }

    public void ClosePlayedNotifiaction()
    {
        playedNotification.SetActive(false);

        if (Utils.GetEnemyOf(Player.player).EndRound)
            if (!Player.player.EndRound) DirectPlay();
            else masterController.EndRound();
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
        else if (!aI.DirectPlay(context, out info))
        {
            playedNotificationText.text = info;
            playedNotification.SetActive(true);
            masterController.board.EndRound(Player);
        }

        playedNotificationText.text = info;
        playedNotification.SetActive(true);
        Board.Instance.EndTurn(Player.player);
        masterController.board.UpdateView(true);
        masterController.board.DeactivatePlayableSlots(Player);
    }
}
