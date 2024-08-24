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
        Statements.Input main;
        string mainPath = "D:\\Gwent-Pro\\Gwent pro v2.0\\Assets\\Card creation\\Interpreter\\Files\\";
        public bool ValidLoad = true;

        public Interptreter(Printer printer, List<string> previousCards = null, List<string> previousEffects = null, string path = "")
        {
            Reset();

            this.printer = printer;
            if (path != "") mainPath = path;

            try
            {
                if (!(previousEffects is null)) foreach (var item in previousEffects)
                    {
                        StreamReader sr = new StreamReader(mainPath + "Effects\\Scripts\\" + item + ".gwf");
                        this.Evaluate(sr.ReadLine());
                        sr.Close();
                    }

                if (!(previousCards is null)) foreach (var item in previousCards)
                    {
                        StreamReader sr = new StreamReader(mainPath + "Cards\\Scripts\\" + item + ".gwc");
                        if (!this.Evaluate(sr.ReadLine()))
                        {
                            RemoveUnwantedMessage();
                            Log("Invalid load of previous declarations. There is an unloaded effect used in a card. Reload it with the proper effects and try again");
                            ValidLoad = false;
                        }
                        sr.Close();
                    }
            }
            catch(FileNotFoundException error)
            {
                RemoveUnwantedMessage();
                Log("Invalid load of previous declarations. " + error.Message);
                ValidLoad = false;
            }

            if (ValidLoad) RemoveUnwantedMessage();
        }

        public bool Evaluate(string input)
        {
            if (CheckSemantic(input))
            {
                try
                {
                    main.Execute();
                }
                catch (EvaluationError error)
                {
                    Log(error.Message);
                    return false;
                }
                catch (Warning warn)
                {
                    Log(warn.Message);
                }
                CreatedCards.AddRange(main.CreatedCards());
                return true;
            }
            else return false;
        }

        public bool CheckSemantic(string input)
        {
            if (!ValidLoad) return false;
            else RemoveUnwantedMessage();

            List<Token> list = lexer.Tokenize(input, out string[] lexicalErrors);

            if (lexicalErrors.Length > 0)
            {
                for (int i = 0; i < lexicalErrors.Length; i++)
                {
                    Log($"{i + 1}. {lexicalErrors[i]}");
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
                    else return true;
                }
            }
        }

        public List<Card> CreatedCards { get; private set; }

        void Log(string text) => printer.Print(text);
        void RemoveUnwantedMessage() => printer.Clear();

        void Reset()
        {
            CreatedCards = new List<Card>();
            CardStatement.Reset();
            EffectStatement.Reset();
            Statements.Input.Reset();
        }
    }
}