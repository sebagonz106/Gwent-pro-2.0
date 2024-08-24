using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Gwent_Interpreter;
using System.IO;

public class CardCreationController : MonoBehaviour
{
    [SerializeField] TMP_Text terminal;
    [SerializeField] TMP_InputField console;
    [SerializeField] TMP_Text preLoadedEffectsDisplay;
    [SerializeField] TMP_Text preLoadedCardsDisplay;
    [SerializeField] TMP_InputField preLoadedEffectInput;
    [SerializeField] TMP_InputField preLoadedCardInput;
    [SerializeField] TMP_Text preLoadingTerminal;

    Gwent_Interpreter.Interptreter interpreter;
    List<Card> cards;
    List<string> cardsAdded;
    List<string> preLoadedCards;
    List<string> preLoadedEffects;
    bool PreLoadedCompiled = true;
    public bool LeaderAdded;

    public void Awake()
    {
        ResetCards();
        LeaderAdded = false;
        cardsAdded = new List<string>();
        preLoadedEffects = new List<string>();
        UpdatePreLoadedScripts(preLoadedEffectsDisplay, "Effects");
        UpdatePreLoadedScripts(preLoadedCardsDisplay, "Cards");
        if (CardsWarehouse.RebelCards.Count > 18) CardsWarehouse.RebelCards.RemoveRange(18, CardsWarehouse.RebelCards.Count - 18);
        if (CardsWarehouse.BatistaCards.Count > 18) CardsWarehouse.BatistaCards.RemoveRange(18, CardsWarehouse.BatistaCards.Count - 18);
    }

    public void Compile()
    {
        string[] paths = Directory.GetFiles("D:\\Gwent-Pro\\Gwent pro v2.0\\Assets\\Card creation\\Interpreter\\Files\\Scripts to compile");
        Interptreter interpreter = new Interptreter(new Printer(terminal), preLoadedCards, preLoadedEffects);
        PreLoadedCompiled = true;
        foreach (var path in paths)
            if (path.Substring(path.Length - 5) == ".meta") continue;
            else interpreter.Evaluate(File.ReadAllText(path));
        cards.AddRange(interpreter.CreatedCards);
    }

    public void CompilePreLoaded()
    {
        interpreter = new Interptreter(new Printer(terminal), preLoadedCards, preLoadedEffects);
        if (interpreter.ValidLoad)
        {
            PreLoadedCompiled = true;
            cards.AddRange(interpreter.CreatedCards);
        }
    }

    public void CheckSemanticOnConsole()
    {
        if (PreLoadedCompiled) interpreter.CheckSemantic(console.text);
    }

    public void CompileOnConsole()
    {
        if (PreLoadedCompiled)
        {
            interpreter.Evaluate(console.text);
            cards.AddRange(interpreter.CreatedCards);
        }
    }

    public void AddOrDiscardPreLoadedEffect()
    {
        preLoadingTerminal.text = "";
        ReLoad("effect", preLoadedEffectInput, preLoadedEffectsDisplay.text, preLoadedEffects);
    }
    public void AddOrDiscardPreLoadedCard()
    {
        preLoadingTerminal.text = "";
        ReLoad("card", preLoadedCardInput, preLoadedCardsDisplay.text, preLoadedCards);
    }

    public void DefaultDeck()
    {
        ResetCards();
        Awake();
    }

    public void DeleteFiles()
    {
        string[] paths = Directory.GetFiles("D:\\Gwent-Pro\\Gwent pro v2.0\\Assets\\Card creation\\Interpreter\\Files\\Effects");
        foreach (var item in paths)
            File.Delete(item);
        paths = Directory.GetFiles("D:\\Gwent-Pro\\Gwent pro v2.0\\Assets\\Card creation\\Interpreter\\Files\\Cards");
        foreach (var item in paths)
            File.Delete(item);

        UpdatePreLoadedScripts(preLoadedEffectsDisplay, "Effects");
        UpdatePreLoadedScripts(preLoadedCardsDisplay, "Cards");
    }

    public void AddCardsToGame()
    {
        if (!PreLoadedCompiled) CompilePreLoaded();
        foreach (var card in cards)
        {
            if (card is LeaderCard leader)
            {
                PlayerMB.Leaders.Add(leader.Name, leader);
                LeaderAdded = true;
            }
            else if (card.FactionEnum is Faction.Fidel) CardsWarehouse.RebelCards.Add(card);
            else CardsWarehouse.BatistaCards.Add(card);

            cardsAdded.Add(card.Name);
        }
        UpdatePreLoadedScripts(preLoadedEffectsDisplay, "Effects");
        UpdatePreLoadedScripts(preLoadedCardsDisplay, "Cards");
        ResetCards();
    }

    void ResetCards()
    {
        cards = new List<Card>();
        preLoadedCards = new List<string>();
    }

    void UpdatePreLoadedScripts(TMP_Text text, string folder)
    {
        text.text = "";
        string[] paths = Directory.GetFiles("D:\\Gwent-Pro\\Gwent pro v2.0\\Assets\\Card creation\\Interpreter\\Files\\" + folder);
        if (paths.Length != 0)
        {
            foreach (var path in paths)
            {
                if (path.Substring(path.Length - 5) == ".meta") continue;
                string[] stepsInPath = path.Substring(0, path.Length - 4).Split('\\');
                string name = stepsInPath[stepsInPath.Length - 1];
                if(!(folder == "Cards" && cardsAdded.Contains(name))) text.text += name + ", ";
            }
        }
        if (text.text.Length == 0) text.text = "\n\n                         Vacío";
        else text.text = text.text.Substring(0, text.text.Length - 2);
    }

    void ReLoad(string type, TMP_InputField input, string container, List<string> names)
    {
        preLoadingTerminal.text = "";
        string[] inputNames = input.text.Split(',', System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string item in inputNames)
        {
            string name = item.Trim();
            if (!container.Contains(name))
            {
                preLoadingTerminal.text = "The " + type + " '" + name + "' does not exist.";
            }
            else if (names.Contains(name))
            {
                names.Remove(name);
                preLoadingTerminal.text = "The " + type + " '" + name + "' has been unloaded.";
                input.text = "";
            }
            else
            {
                names.Add(name);
                PreLoadedCompiled = false;
                preLoadingTerminal.text = "The " + type + " '" + name + "' has been loaded.";
                input.text = "";
            }
        }
    }
}
