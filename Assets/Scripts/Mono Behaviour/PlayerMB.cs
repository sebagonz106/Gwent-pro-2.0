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
    static Dictionary<string, LeaderCard> leaders = new Dictionary<string, LeaderCard>
    {
        {"Camilo Cienfuegos", (LeaderCard)CardsWarehouse.RebelCards[0] },
        {"Ernesto Che Guevara", (LeaderCard)CardsWarehouse.RebelCards[1] },
        {"Eulogio Cantillo", (LeaderCard)CardsWarehouse.BatistaCards[0] },
        {"Francisco Tabernilla", (LeaderCard)CardsWarehouse.BatistaCards[1] }
    };
    public GameObject WonCoin1;
    public GameObject WonCoin2;
    public GameObject[] Body { get; private set; }
    public GameObject[] AvailableSlots { get; private set; }
    public string Name { get; private set; }

    private void Awake()
    {
        Name = this.gameObject.name;
        player = Utils.GetPlayerByName(Name);
        if (PlayerPrefs.GetString(Name + " Leader") == "") player.Leader = Name == "Batista" ? leaders["Francisco Tabernilla"] : leaders["Ernesto Che Guevara"];
        else player.Leader = leaders[PlayerPrefs.GetString(Name + " Leader")];
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
