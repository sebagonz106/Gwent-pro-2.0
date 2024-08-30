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
    public GameObject WonCoin1;
    public GameObject WonCoin2;
    public GameObject[] Body { get; private set; }
    public GameObject[] AvailableSlots { get; private set; }
    public string Name { get; private set; }

    private void Awake()
    {
        Name = this.gameObject.name;
        player = Utils.GetPlayerByName(Name);
        if (PlayerPrefs.GetString(Name + " Leader") == "") player.Leader = Name == "Batista" ? Player.Leaders["Francisco Tabernilla"] : Player.Leaders["Ernesto Che Guevara"];
        else player.Leader = Player.Leaders[PlayerPrefs.GetString(Name + " Leader")];

        if (player.Leader.Info is null) player.Leader.AssignInfo(new VisualInfo(Resources.Load<Material>($"Materials/{Name}/{player.Leader.Name}"),
                                                                                Resources.Load<Sprite>($"Info/{Name}/{player.Leader.Name}"), 
                                                                                player.Leader.Faction));
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
