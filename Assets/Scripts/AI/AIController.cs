using System.Collections;
using System.Collections.Generic;
using Gwent_AI;
using UnityEngine;
using TMPro;

public class AIController : MonoBehaviour
{
    [SerializeField] GameObject playedNotification;
    [SerializeField] TMP_Text playedNotificationText;
    PlayerMB player;
    GwentAI aI;
    AIContext context;

    public void Initialize(PlayerMB player)
    {
        this.player = player;
        aI = new GwentAI(player.Name);
        context = new AIContext(player.player);
    }

    public void StartRound()
    {

    }

    public void EndRound()
    {

    }

    public void EndTurn()
    {

    }

    public void CheckNextRound()
    {

    }

    public void ClosePlayedNotifiaction()
    {
        playedNotification.SetActive(false);

        if (Utils.GetEnemyOf(player.player).EndRound)
            if (!player.player.EndRound) DirectPlay();
            else CheckNextRound();
    }

    public void Play()
    {
        if (this.player.player.EndRound) return;

        Player player = this.player.player;
        (Card,Zone,Card) value = aI.Play(context, out bool leaderEffect);

        Card card = value.Item1;
        Zone zone = value.Item2;
        Card target = value.Item3;

        if (leaderEffect) player.Leader.Effect(player.context.UpdatePlayerInstance(target.CurrentPosition, target));
        else if (card is BaitCard bait) bait.Effect(player.context.UpdatePlayerInstance(target.CurrentPosition, target));
        else if (!(card is null)) player.PlayCard(player.Hand.IndexOf(card), player.ListByZone[zone].IndexOf(Utils.BaseCard), zone, out bool effectFailed);
        else player.EndRound = true;
    }

    public void DirectPlay()
    {
        string info = "";
        if (this.player.player.EndRound) return;
        else if (!aI.DIrectPlay(context, out info)) this.player.player.EndRound = true;

        playedNotificationText.text = info;
        playedNotification.SetActive(true);
    }
}
