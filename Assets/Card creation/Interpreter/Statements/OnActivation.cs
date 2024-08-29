using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Gwent_Interpreter.Statements
{
    class OnActivation : IStatement
    {
        (int, int) coordinates;
        List<(EffectActivation, EffectActivation)> effects;

        public OnActivation((int, int) coordinates, List<(EffectActivation, EffectActivation)> effects)
        {
            this.coordinates = coordinates;
            this.effects = effects;
        }

        public (int, int) Coordinates => coordinates;

        public bool CheckSemantic(out List<string> errors)
        {
            errors = new List<string>();
            string warnings = "";

            foreach (var item in effects)
            {
                try
                {
                    item.Item1.CheckSemantic(out errors);
                }
                catch(Warning warning)
                {
                    warnings += warning.Message + '\n';
                }
                
                if (item.Item2.Coordinates != (0,0))
                {
                    try
                    {
                        item.Item2.CheckSemantic(out List<string> temp); //postAction
                        errors.AddRange(temp);
                    }
                    catch (Warning warning)
                    {
                        warnings += warning.Message + '\n';
                    }
                }
            }

            if (warnings.Length > 0) throw new Warning(warnings);
            return errors.Count == 0;
        }

        public void Execute() //falla al jugar la carta luego de recuperarla con un señuelo, intentar resetear el scope despues de la ejecucion
        {
            foreach (var item in effects)
            {
                try
                {
                    item.Item1.Execute();
                }
                catch(EvaluationError error)
                {
                    Debug.Log(item.Item1.Coordinates);
                    Debug.Log(error.Message);
                }

                try
                {
                    if (item.Item2.Coordinates != (0, 0)) item.Item2.Execute(); //postAction
                }
                catch (EvaluationError error)
                {
                    Debug.Log(error.Message);
                }
            }
        }
    }
}
