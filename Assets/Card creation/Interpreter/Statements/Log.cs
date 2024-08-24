using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Gwent_Interpreter.Statements
{
    class Log : IStatement
    {
        public IExpression Value { get; private set; }

        public (int, int) Coordinates { get; }

        public Log((int, int) coordinates, IExpression value)
        {
            Value = value;
            Coordinates = coordinates;
        }
        public void Execute()
        {
            Debug.Log(Value.Evaluate());
        }

        public bool CheckSemantic(out List<string> errors)
        {
            errors = new List<string>();

            if (!Value.CheckSemantic(out string error)) errors.Add(error);
            else return true;

            return false;
        }
    }
}
