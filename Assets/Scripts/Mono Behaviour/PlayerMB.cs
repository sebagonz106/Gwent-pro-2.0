using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMB : MonoBehaviour
{
    public Player player;
    public BoardMB board;
    public BattlefieldMB battlefield;
    public GameObject Hand;
    [SerializeField] LeaderCard stealCardLeader;
    [SerializeField] LeaderCard cardStaysLeader;
    public GameObject WonCoin1;
    public GameObject WonCoin2;

    private void Awake()
    {
        if (this.gameObject.name == "Fidel")
        {
            player = Player.Fidel;
            if (stealCardLeader.name == PlayerPrefs.GetString("Rebel Leader")) player.Leader = stealCardLeader;
            else player.Leader = cardStaysLeader;

            player.Leader.information = Resources.Load<Sprite>($"Info/Fidel/{player.Leader.name}");
        }

        else if (this.gameObject.name == "Batista")
        {
            player = Player.Batista;

            if (stealCardLeader.name == PlayerPrefs.GetString("Batista Leader")) player.Leader = stealCardLeader;
            else player.Leader = cardStaysLeader;

            player.Leader.information = Resources.Load<Sprite>($"Info/Batista/{player.Leader.name}");
        }
    }

    public bool PlayCard(int originPosition, int targetPosition, Zone rangeType)
    {
        if(player.PlayCard(originPosition, targetPosition, rangeType, out bool effectFailed))
        {
            if(effectFailed) board.masterController.EffectException();

            return true;
        }
        return false;
    }
}
