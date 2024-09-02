using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class Operation
{
    protected Card card;
    protected List<Card> list;
    protected int position;

    public abstract void Restore();
}

public class AddOperation : Operation
{
    bool deckOrGraveyard;
    public AddOperation (Card card, List<Card> list, int position, bool deckOrGraveyard = false)
    {
        this.card = card;
        this.list = list;
        this.position = position;
        this.deckOrGraveyard = deckOrGraveyard;
    }
    public override void Restore()
    {
        if (deckOrGraveyard) list.RemoveAt(position);
        else list[position] = Utils.BaseCard;

        if (card is ClearCard clear && !deckOrGraveyard && !list.Equals(clear.Owner.Hand)) clear.Owner.Battlefield.RemoveClearEffect(Utils.IndexByZone[clear.Owner.ZoneByList[list]]);
    }
}

public class RemoveOperation : Operation
{
    bool deckOrGraveyard;
    public RemoveOperation(Card card, List<Card> list, int position, bool deckOrGraveyard = false)
    {
        this.card = card;
        this.list = list;
        this.position = position;
        this.deckOrGraveyard = deckOrGraveyard;
    }
    public override void Restore()
    {
        if (deckOrGraveyard) list.Add(card);
        else list[position] = card;

        card.AssignPosition(list);
        if(card is ClearCard clear && !deckOrGraveyard && !list.Equals(clear.Owner.Hand)) clear.Owner.Battlefield.ClearsPlayed[Utils.IndexByZone[clear.Owner.ZoneByList[list]]] = true;
    }
}

public class BaitOperation : Operation
{
    Card removedCard;
    public BaitOperation(BaitCard card, Card removedCard, List<Card> list, int position)
    {
        this.removedCard = removedCard;
        this.card = card;
        this.list = list;
        this.position = position;
    }
    public override void Restore()
    {
        card.Owner.Hand[card.Owner.Hand.IndexOf(removedCard)] = card;
        list[position] = removedCard;
        removedCard.AssignPosition(list);
        card.AssignPosition(card.Owner.Hand);
        if (card is ClearCard clear) clear.Owner.Battlefield.ClearsPlayed[Utils.IndexByZone[clear.Owner.ZoneByList[list]]] = true;
    }
}

public class DamageModification : Operation
{
    public DamageModification(UnitCard unit)
    {
        this.card = unit;
    }

    public override void Restore()
    {
        ((UnitCard)card).RestoreLast();
    }
}

public class LeaderEffectOperation : Operation
{
    public LeaderEffectOperation(LeaderCard card)
    {
        this.card = card;
    }

    public override void Restore()
    {
        card.Owner.LeaderEffectUsedThisRound = false;
        if (((LeaderCard)card).NeedsCardSelection) card.Owner.Battlefield.StaysInBattlefieldRemove();
    }
}

public class ShuffleOperation : Operation
{
    List<Card> previousOrder;

    public ShuffleOperation(List<Card> previousOrder, List<Card> list)
    {
        this.list = list;
        this.previousOrder = previousOrder;
    }

    public override void Restore()
    {
        for (int i = 0; i < previousOrder.Count; i++)
        {
            list[i] = previousOrder[i];
        }
    }
}

public class TurnInfo
{
    Stack<Operation> operations;

    public TurnInfo()
    {
        operations = new Stack<Operation>();
    }

    public void Receive(Operation operation)
    {
        operations.Push(operation);
    }

    public void Restore()
    {
        while (operations.Count > 0)
        {
            Operation operation = operations.Pop();
            operation.Restore();
        }
    }
}
