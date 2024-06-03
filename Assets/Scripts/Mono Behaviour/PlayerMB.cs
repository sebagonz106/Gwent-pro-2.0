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
    public GameObject[] Body { get; private set; }
    public GameObject[] AvailableSlots { get; private set; }
    public string Name { get; private set; }

    private void Awake()
    {
        Name = this.gameObject.name;
        if (Name == "Fidel")
        {
            player = Player.Fidel;
            if (stealCardLeader.Name == PlayerPrefs.GetString("Rebel Leader")) player.Leader = stealCardLeader;
            else player.Leader = cardStaysLeader;
        }

        else if (Name == "Batista")
        {
            player = Player.Batista;

            if (stealCardLeader.Name == PlayerPrefs.GetString("Batista Leader")) player.Leader = stealCardLeader;
            else player.Leader = cardStaysLeader;
        }
    }

    private void Start()
    {
        Body = GameObject.FindGameObjectsWithTag($"{this.gameObject.name} Body");
        AvailableSlots = GameObject.FindGameObjectsWithTag(this.gameObject.name);
    }

    public bool PlayCard(int originPosition, int targetPosition, Zone rangeType)
    {
        if(player.PlayCard(originPosition, targetPosition, rangeType, out bool effectFailed))
        {
            if(effectFailed) board.masterController.EffectException();
            board.UpdateView(true);
            return true;
        }
        return false;
    }
}
