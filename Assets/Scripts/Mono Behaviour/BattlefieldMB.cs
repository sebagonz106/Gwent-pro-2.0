using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattlefieldMB : MonoBehaviour
{
    public PlayerMB playerThatOwnsThisBattlefield;
    public Battlefield battlefield;

    private void Start()
    {
        this.battlefield = playerThatOwnsThisBattlefield.player.Battlefield;
    }

    public void Clear()
    {
        if (!battlefield.Clear()) playerThatOwnsThisBattlefield.board.masterController.EffectException();
    }
}
