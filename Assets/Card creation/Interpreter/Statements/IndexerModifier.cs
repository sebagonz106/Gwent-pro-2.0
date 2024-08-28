using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gwent_Interpreter.Expressions;

namespace Gwent_Interpreter.Statements
{
    class IndexerModifier : IStatement
    {
        Indexer indexer;
        Token operation;
        IExpression value;

        public IndexerModifier(Indexer indexer, Token operation, IExpression value=null)
        {
            this.indexer = indexer;
            this.operation = operation;
            this.value = value;
        }

        public (int, int) Coordinates => operation.Coordinates;

        public bool CheckSemantic(out List<string> errors)
        {
            errors = new List<string>();
            string warning = "";
            try
            {
                indexer.CheckSemantic(out errors);
            }
            catch(Warning warn)
            {
                warning += warn.Message + '\n';
            }

            if (operation.Value != "=") errors.Add("Invalid operation at " + operation.Coordinates.Item1 + ":" + operation.Coordinates.Item2 + " (only '=' is allowed).");
            if (value is null) errors.Add("Invalid operation at " + operation.Coordinates.Item1 + ":" + operation.Coordinates.Item2 + " (value missing).");

            if (value.Return is ReturnType.Object) warning += $"You must make sure object at {value.Coordinates.Item1}:{value.Coordinates.Item2 - 1} is a card or a compile time error may occur";
            else if (value.Return != ReturnType.Card) errors.Add("Invalid operation at " + operation.Coordinates.Item1 + ":" + operation.Coordinates.Item2 + " (value is not a card).");

            try
            {
                if (!value.CheckSemantic(out string error)) errors.Add(error);
            }
            catch (Warning warn)
            {
                warning += warn.Message;
            }

            if (warning.Length > 0) throw new Warning(warning);
            else return errors.Count != 0;
        }

        public void Execute()
        {
            indexer.SetValue(value);
        }
    }
}
