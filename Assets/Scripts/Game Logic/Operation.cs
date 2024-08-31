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
    public AddOperation (Card card, List<Card> list, int position)
    {
        this.card = card;
        this.list = list;
        this.position = position;
    }
    public override void Restore()
    {
        list[position] = Utils.BaseCard;
        if (card is UnitCard unit) unit.RestoreLast();
    }
}

public class RemoveOperation : Operation
{
    public RemoveOperation(Card card, List<Card> list, int position)
    {
        this.card = card;
        this.list = list;
        this.position = position;
    }
    public override void Restore()
    {
        list[position] = card;
        card.AssignPosition(list);
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
        list[position] = removedCard;
        removedCard.AssignPosition(list);
        if (removedCard is UnitCard unit) unit.RestoreLast();

    }
}

public class TurnInfo
{
    Stack<Operation> operations;
    public bool LeaderEffectApplied { get; private set; }

    public TurnInfo()
    {
        operations = new Stack<Operation>();
    }

    public void Receive(Operation operation)
    {
        operations.Push(operation);
    }

    public void LeaderEffect() => LeaderEffectApplied = true;

    public void Restore()
    {
        while (operations.Count > 0)
        {
            Operation operation = operations.Pop();
            operation.Restore();
        }
        LeaderEffectApplied = false;
    }
}
