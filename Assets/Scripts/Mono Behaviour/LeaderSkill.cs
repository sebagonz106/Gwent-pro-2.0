using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderSkillPanel : MonoBehaviour
{
    [SerializeField] GameObject Info;
    [SerializeField] GameObject skillButton;
    [SerializeField] BoardMB board;

    public void LeaderSkillWhenCardSelected(Player player, Card card, List<Card> list)
    {
        player.Leader.Effect(player.context.UpdatePlayerInstance(list, card));
        Info.SetActive(true);
        board.board.ValidTurn = true;
        skillButton.SetActive(false);
    }

    public void LeaderSkill()
    {
        Player player = board.board.GetCurrentPlayer();
        LeaderCard leader = player.Leader;
        if (player.LeaderEffectUsedThisRound) return;

        if (!leader.NeedsCardSelection)
        {
            leader.Effect(player.context);
            Board.Instance.UpdateTotalDamage();
            board.UpdateView(true);
            board.board.ValidTurn = true;
            skillButton.SetActive(false);
        }
        else
        {
            player.LeaderCardSelected = true;
            Info.SetActive(false);
        }
    }
}
