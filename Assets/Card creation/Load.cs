using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Gwent_Interpreter;
using System.IO;

public class Load : MonoBehaviour
{
    [SerializeField] TMP_Text terminal;
    [SerializeField] TMP_Text console;
    [SerializeField] TMP_Text preLoadedEffectsDisplay;
    [SerializeField] TMP_Text preLoadedCardsDisplay;
    [SerializeField] TMP_InputField preLoadedEffectInput;
    [SerializeField] TMP_InputField preLoadedCardInput;
    [SerializeField] TMP_Text preLoadingTerminal;

    List<Card> cards = new List<Card>();
    List<string> preLoadedCards;
    List<string> preLoadedEffects;
    bool preLoadedCompiled = true;

    public void Awake()
    {
        preLoadedCards = new List<string>();
        preLoadedEffects = new List<string>();
        UpdatePreLoadedScripts(preLoadedEffectsDisplay, "Effects");
        UpdatePreLoadedScripts(preLoadedCardsDisplay, "Cards");
    }

    public void Compile()
    {
        string[] paths = Directory.GetFiles("D:\\Gwent-Pro\\Gwent pro v2.0\\Assets\\Card creation\\Interpreter\\Files\\Scripts to compile");
        Interptreter interpreter = new Interptreter(new Printer(terminal), preLoadedCards, preLoadedEffects);
        foreach (var path in paths)
            if (path.Substring(path.Length - 5) == ".meta") continue;
            else interpreter.Evaluate(File.ReadAllText(path));
        cards.AddRange(interpreter.CreatedCards);
    }

    public void CompilePreLoaded()
    {
        Interptreter interpreter = new Interptreter(new Printer(terminal), preLoadedCards, preLoadedEffects);
        cards.AddRange(interpreter.CreatedCards);
    }

    public void AddOrDiscardPreLoadedEffect()
    {
        preLoadedCompiled = false;
        preLoadingTerminal.text = "";
        ReLoad("effect", preLoadedEffectInput, preLoadedEffectsDisplay.text, preLoadedEffects);
    }
    public void AddOrDiscardPreLoadedCard()
    {
        preLoadedCompiled = false;
        preLoadingTerminal.text = "";
        ReLoad("card", preLoadedCardInput, preLoadedCardsDisplay.text, preLoadedCards);
    }

    public void Restore()
    {
        string[] paths = Directory.GetFiles("D:\\Gwent-Pro\\Gwent pro v2.0\\Assets\\Card creation\\Interpreter\\Files\\Effects");
        foreach (var item in paths)
            File.Delete(item);
        paths = Directory.GetFiles("D:\\Gwent-Pro\\Gwent pro v2.0\\Assets\\Card creation\\Interpreter\\Files\\Cards");
        foreach (var item in paths)
            File.Delete(item);
        ResetCards();
        Awake();
    }

    public void AddCardsToGame()
    {
        if (!preLoadedCompiled) CompilePreLoaded();
        foreach (var card in cards)
        {
            if (card is LeaderCard leader) PlayerMB.Leaders.Add(leader.Name, leader);
            else if (card.FactionEnum is Faction.Fidel) CardsWarehouse.RebelCards.Add(card);
            else CardsWarehouse.BatistaCards.Add(card);
        }
        ResetCards();
    }

    void ResetCards()
    {
        cards = new List<Card>();
    }

    void UpdatePreLoadedScripts(TMP_Text text, string folder)
    {
        string[] paths = Directory.GetFiles("D:\\Gwent-Pro\\Gwent pro v2.0\\Assets\\Card creation\\Interpreter\\Files\\" + folder);
        if (paths.Length == 0) text.text = "\n\n                         Vacío";
        else
        {
            text.text = "";
            foreach (var path in paths)
            {
                if (path.Substring(path.Length - 5) == ".meta") continue;
                string[] stepsInPath = path.Substring(0, path.Length - 4).Split('\\');
                string name = stepsInPath[stepsInPath.Length - 1];
                text.text += name + ", ";
            }
            text.text = text.text.Substring(0, text.text.Length - 2);
        }
    }

    void ReLoad(string type, TMP_InputField input, string container, List<string> names)
    {
        preLoadingTerminal.text = "";
        string name = input.text;
        if (!container.Contains(name))
        {
            preLoadingTerminal.text = "The "+type+" '" + name + "' does not exist.";
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
            preLoadingTerminal.text = "The " + type + " '" + name + "' has been loaded.";
            input.text = "";
        }
    }
}
