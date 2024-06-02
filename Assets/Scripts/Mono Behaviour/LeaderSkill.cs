using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderSkillPanel : MonoBehaviour
{
    [SerializeField] GameObject Info;
    [SerializeField] BoardMB board;

    public void LeaderSkillWhenCardSelected(Player player, Card card, List<Card> list)
    {
        player.Leader.Effect(player.context.UpdatePlayerInstance(list, card));
        Info.SetActive(true);
    }

    public void LeaderSkill()
    {
        Player player = board.board.GetCurrentPlayer();
        LeaderCard leader = player.Leader;
        if (player.LeaderEffectUsedThisRound) return;

        if (!leader.NeedsCardSelection)
        {
            leader.Effect(player.context);
            board.UpdateView();
        }
        else
        {
            player.LeaderCardSelected = true;
            Info.SetActive(false);
        }
    }
}
