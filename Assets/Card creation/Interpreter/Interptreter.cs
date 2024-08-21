using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Gwent_Interpreter.Statements;
using Gwent_Interpreter.Expressions;

namespace Gwent_Interpreter
{
    class Interptreter
    {
        Lexer lexer = new Lexer();
        Parser parser;
        Printer printer;
        Input main;
        string mainPath = "D:\\Gwent-Pro\\Gwent pro v2.0\\Assets\\Card creation\\Interpreter\\Files\\";
        bool validLoad = true;

        public Interptreter(Printer printer, List<string> previousCards = null, List<string> previousEffects = null, string path = "")
        {
            Reset();

            this.printer = printer;
            if (path != "") mainPath = path;

            try
            {
                if (!(previousEffects is null)) foreach (var item in previousEffects)
                    {
                        StreamReader sr = new StreamReader(mainPath + "Effects\\" + item + ".gwf");
                        this.Evaluate(sr.ReadLine());
                        sr.Close();
                    }

                if (!(previousCards is null)) foreach (var item in previousCards)
                    {
                        StreamReader sr = new StreamReader(mainPath + "Cards\\" + item + ".gwc");
                        if (!this.Evaluate(sr.ReadLine()))
                        {
                            Log("Invalid load of previous declarations. There is an unloaded effect used in a card.");
                            validLoad = false;
                        }
                        sr.Close();
                    }
            }
            catch(FileNotFoundException error)
            {
                Log("Invalid load of previous declarations. " + error.Message);
                validLoad = false;
            }

            if (validLoad) RemoveUnwantedMessage();
        }

        public bool Evaluate(string input)
        {
            if (!validLoad) return false;

            List<Token> list = lexer.Tokenize(input, out string[] lexicalErrors);

            if (lexicalErrors.Length > 0)
            {
                for (int i = 0; i < lexicalErrors.Length; i++)
                {
                    Log($"{i+1}. {lexicalErrors[i]}");
                }
                return false;
            }
            else
            {
                parser = new Parser(list);

                main = parser.Parse();

                if (parser.Errors.Count > 0)
                {
                    foreach (var error in parser.Errors) Log(error);
                    return false;
                }
                else
                {
                    List<string> semanticErrors = new List<string>();

                    try
                    {
                        main.CheckSemantic(out semanticErrors);
                    }
                    catch (Warning warning)
                    {
                        Log(warning.Message);
                    }

                    if (semanticErrors.Count > 0)
                    {
                        foreach (var error in semanticErrors) Log(error);
                        return false;
                    }
                    else
                    {
                        try
                        {
                            main.Execute();
                        }
                        catch (MyException error)
                        {
                            Log(error.Message);
                            return false;
                        }
                    }
                }
                foreach (var item in CardStatement.Cards) //testing
                {
                    try
                    {
                        item.Effect(Player.Fidel.context);
                    }
                    catch (EvaluationError error)
                    {
                        Log(error.Message);
                    }
                }
            }
            return true;
        }
        public List<Card> CreatedCards => main.CreatedCards();

        void Log(string text) => printer.Print(text);
        void RemoveUnwantedMessage() => Console.Clear();

        static void Reset()
        {
            CardStatement.Reset();
            EffectStatement.Reset();
            Input.Reset();
        }
    }
}