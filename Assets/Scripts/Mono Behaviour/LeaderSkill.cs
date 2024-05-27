using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderSkillPanel : MonoBehaviour
{
    [SerializeField] GameObject Info;
    [SerializeField] Board board;

    public void StaysInBattlefieldModifierSkill(Card card, List<Card> list)
    {
        this.GetComponent<MasterController>().board.GetCurrentPlayer().Leader.KeepInBattlefield(card, list);
        Info.SetActive(true);
    }

    public void LeaderSkill()
    {
        LeaderCard leader = this.GetComponent<MasterController>().board.GetCurrentPlayer().Leader;
        if (this.board.GetCurrentPlayer().leaderEffectUsedThisRound) return;

        if (leader.stealCardLeader)
        {
            leader.StealCard();
            board.UpdateView();
        }
        else
        {
            board.GetCurrentPlayer().LeaderCardSelected = true;
            Info.SetActive(false);
        }
    }
}
