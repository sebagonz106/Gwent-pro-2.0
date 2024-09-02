using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderSkillPanel : MonoBehaviour
{
    [SerializeField] GameObject effectException;
    [SerializeField] GameObject Info;
    [SerializeField] GameObject skillButton;
    [SerializeField] BoardMB board;

    public void LeaderSkillWhenCardSelected(Player player, Card card, List<Card> list)
    {
        if(!player.Leader.Effect(player.context.UpdatePlayerInstance(list, card))) Exception();
        Info.SetActive(true);
        board.board.ValidTurn = true;
        skillButton.SetActive(false);
    }

    public void LeaderSkill()
    {
        Player player = board.board.GetCurrentPlayer();
        LeaderCard leader = player.Leader;
        if (player.LeaderEffectUsedThisRound)
        {
            Exception();
            return;
        }

        if (!leader.NeedsCardSelection)
        {
            if(!leader.Effect(player.context)) Exception();
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

    public void Exception()
    {
        effectException.SetActive(true);
        Info.SetActive(false);
    }
}
