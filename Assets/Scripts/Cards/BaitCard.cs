using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaitCard : Card
{
    public BaitCard(string name, Faction faction, Type cardType, List<Zone> availableRange, double initialDamage = 0, Effect effect = null) :
               base(name, faction, cardType, availableRange, initialDamage, effect)
    {
    }

    public override bool Effect(Context context)
    {
        bool ok = false;
        try
        {
            ok = Effect(context.CurrentPosition, context.CurrentPosition.IndexOf(context.CurrentCard)) & effect.Invoke(context);
        }
        catch
        {
            ok = false;
        }
        Board.Instance.UpdateTotalDamage();
        Board.Instance.ValidTurn = true;
        return ok;
    }

    public bool Effect(List<Card> list, int index)
    {
        Card card = list[index];
        if (card is BaitCard) return false;
        Owner.Hand[Owner.Hand.IndexOf(this)] = card;
        list[index] = this;
        card.AssignPosition(Owner.Hand);
        this.AssignPosition(list);
        Debug.Log(list[index]);
        Debug.Log(Owner.Name);
        foreach (var item in list)
        {
            Debug.Log(item);
        }
        if (card is UnitCard unit) unit.InitializeDamage(); //in case any permanent effects were applied on this card
        else if (card is ClearCard) Owner.Battlefield.RemoveClearEffect(Utils.IndexByZone[Owner.ZoneByList[list]]);
        Board.Instance.Receive(new BaitOperation(this, card, list, index));
        return true;
    }
}
