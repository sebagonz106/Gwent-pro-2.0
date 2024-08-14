using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattlefieldMB : MonoBehaviour
{
    public PlayerMB owner;
    public Battlefield battlefield;

    private void Start()
    {
        this.battlefield = owner.player.Battlefield;
    }

    public void Clear()
    {
        if (!battlefield.Clear()) owner.board.masterController.EffectException();
    }
}
