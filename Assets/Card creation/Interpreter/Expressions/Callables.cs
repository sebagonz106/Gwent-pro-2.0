using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Gwent_Interpreter.GameLogic;
using Gwent_Interpreter.Utils;

namespace Gwent_Interpreter.Expressions
{
    abstract class Callable : Expr<object>
    {
        public IExpression callee;
        public Token caller;

        public override ReturnType Return => ReturnType.Object;
        public override bool CheckSemantic(out string error)
        {
            error = "";
            if (!this.CheckSemantic(out List<string> errors))
                for (int i = 0; i < errors.Count; i++)
                {
                    error += errors[i];
                    if (i != errors.Count - 1) error += "\n";
                }
            else return true;

            return false;
        }

        public override (int, int) Coordinates { get => caller.Coordinates; protected set => throw new NotImplementedException(); }
    }

    class Property : Callable
    {
        public Property(Token caller, IExpression callee)
        {
            this.caller = caller;
            this.callee = callee;
        }

        public override bool CheckSemantic(out List<string> error) => throw new Warning($"You must make sure object at {caller.Coordinates.Item1}:{caller.Coordinates.Item2-1} contains the requested property or a compile time error may occur");

        public override object Evaluate()
        {
            object callee = this.callee.Evaluate();

            Type type;
            if (callee is GwentInterpreterContext) type = typeof(GwentInterpreterContext);
            else if (callee is GwentList) type = typeof(GwentList);
            else if (callee is Card) type = typeof(Card);
            else if (callee is Str) type = typeof(Str);
            else if (callee is Num) type = typeof(Num);
            else type = typeof(object);

            if (type.GetProperty(caller.Value) != null)
            {
                object result = type.GetProperty(caller.Value).GetValue(callee);
                if (result is double || result is int) return new Num(Convert.ToDouble(result));
                else if (result is string sResult) return new Str(sResult);
                else return result;
            }
            else throw new EvaluationError($"Property not found at {caller.Coordinates.Item1}:{caller.Coordinates.Item2}");
        }
    }
    class Method : Callable
    {
        public IExpression[] arguments;

        public Method(Token caller, IExpression callee, IExpression[] arguments = null)
        {
            this.caller = caller;
            this.callee = callee;
            this.arguments = arguments;
        }

        public override bool CheckSemantic (out List<string> errors)
        {
            errors = new List<string>();
            string warning = $"You must make sure object at {caller.Coordinates.Item1}:{caller.Coordinates.Item2 - 1} contains the requested method or a compile time error may occur.\n";
            try
            {
                callee.CheckSemantic(out errors);
            }
            catch (Warning) { }

            if(!(arguments is null)) foreach (var item in arguments)
                {
                    try
                    {
                        if (!item.CheckSemantic(out string error)) errors.Add(error);
                    }
                    catch(Warning warn) { warning += warn.Message + '\n'; }
                }

            if (errors.Count > 0) return false;
            else throw new Warning(warning);
        }
        public override object Evaluate()
        {
            object callee = this.callee.Evaluate();
            object[] arguments = new object[this.arguments.Length];

            for (int i = 0; i < arguments.Length; i++)
            {
                arguments[i] = this.arguments[i].Evaluate();
            }

            Type type;
            if (callee is GwentInterpreterContext) type = typeof(GwentInterpreterContext);
            else if (callee is GwentList) type = typeof(GwentList);
            else if (callee is Card) type = typeof(Card);
            else if (callee is Str) type = typeof(Str);
            else if (callee is Num) type = typeof(Num);
            else type = typeof(object);

            MethodInfo method = null;

            try
            {
                method = type.GetMethod(caller.Value);
            }
            catch (AmbiguousMatchException)
            {
                method = type.GetMethod(caller.Value, new Type[0]);
            }

            if (method is null) throw new EvaluationError($"Method not found at {caller.Coordinates.Item1}:{caller.Coordinates.Item2}");
            else
            {
                try
                {
                    if(method.ReturnType != typeof(void))
                    {
                        object result = method.Invoke(callee, this.arguments);
                        if (result is double || result is int) return new Num(Convert.ToDouble(result));
                        else if (result is string sResult) return new Str(sResult);
                        else return result;
                    }
                    else method.Invoke(callee, this.arguments);
                    return null;
                }
                catch (ArgumentException)
                {
                    throw new EvaluationError($"Invalid arguments given at {caller.Coordinates.Item1}:{caller.Coordinates.Item2}");
                }
            }
        }
    }
}
