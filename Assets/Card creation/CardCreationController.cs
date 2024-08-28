using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Gwent_Interpreter;
using System.IO;

public class CardCreationController : MonoBehaviour
{
    #region Fields
    [SerializeField] Material cardMaterial;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject menu;
    [SerializeField] GameObject consoleMenu;
    [SerializeField] TMP_Text terminal;
    [SerializeField] TMP_Text preLoadedEffectsDisplay;
    [SerializeField] TMP_Text preLoadedCardsDisplay;
    [SerializeField] TMP_Text preLoadingTerminal;
    [SerializeField] TMP_InputField console;
    [SerializeField] TMP_InputField preLoadedEffectInput;
    [SerializeField] TMP_InputField preLoadedCardInput;
    [SerializeField] TMP_InputField scriptLoadingPathInput;
    [SerializeField] TMP_InputField imageLoadingPathInput;
    [SerializeField] TMP_InputField infoLoadingPathInput;

    Gwent_Interpreter.Interptreter interpreter;
    VisualAssigner visualAssigner;
    List<Card> cards;
    List<string> cardsAdded;
    List<string> preLoadedCards;
    List<string> preLoadedEffects;
    string mainPath = "C:\\Users\\Public\\Documents\\1958 Files";
    string scriptLoadingPath = "D:\\Gwent-Pro\\Gwent pro v2.0\\Assets\\Card creation\\Interpreter\\Files\\New\\Scripts";
    string imageLoadingPath = "D:\\Gwent-Pro\\Gwent pro v2.0\\Assets\\Card creation\\Interpreter\\Files\\New\\Main image";
    string infoLoadingPath = "D:\\Gwent-Pro\\Gwent pro v2.0\\Assets\\Card creation\\Interpreter\\Files\\New\\Info";
    bool PreLoadedCompiled = true;
    #endregion

    public void Awake()
    {
        Directory.CreateDirectory(mainPath + "\\Cards\\Scripts");
        Directory.CreateDirectory(mainPath + "\\Cards\\Main image");
        Directory.CreateDirectory(mainPath + "\\Cards\\Info");
        Directory.CreateDirectory(mainPath + "\\Effects\\Scripts");
        ResetCards();
        cardsAdded = new List<string>();
        preLoadedEffects = new List<string>();
        UpdatePreLoadedScripts(preLoadedEffectsDisplay, "Effects");
        UpdatePreLoadedScripts(preLoadedCardsDisplay, "Cards");
        if (CardsWarehouse.RebelCards.Count > 22) CardsWarehouse.RebelCards.RemoveRange(22, CardsWarehouse.RebelCards.Count - 22);
        if (CardsWarehouse.BatistaCards.Count > 22) CardsWarehouse.BatistaCards.RemoveRange(22, CardsWarehouse.BatistaCards.Count - 22);
    }

    public void CompileScripts()
    {
        string[] paths = Directory.GetFiles(scriptLoadingPath);
        Interptreter interpreter = new Interptreter(new Printer(terminal), preLoadedCards, preLoadedEffects);
        PreLoadedCompiled = true;
        foreach (var path in paths)
            if (path.Substring(path.Length - 5) == ".meta") continue;
            else if (!interpreter.Evaluate(File.ReadAllText(path)))
            {
                consoleMenu.SetActive(true);
                menu.SetActive(false);
            }
        
            AddCards(interpreter.CreatedCards, imageLoadingPath, infoLoadingPath, true);
    }

    public void CompilePreLoaded()
    {
        interpreter = new Interptreter(new Printer(terminal), preLoadedCards, preLoadedEffects);
        if (interpreter.ValidLoad)
        {
            PreLoadedCompiled = true;
            AddCards(interpreter.CreatedCards, mainPath + "\\Cards\\Main image", mainPath + "\\Cards\\Info", false);
        }
        else
        {
            consoleMenu.SetActive(true);
            menu.SetActive(false);
        }
    }

    public void CheckSemanticOnConsole()
    {
        if (PreLoadedCompiled) interpreter.CheckSemantic(console.text);
    }

    public void CompileOnConsole()
    {
        if (console.text == "") return;

        if (PreLoadedCompiled)
        {
            if(interpreter.Evaluate(console.text)) console.text = "";
            AddCards(interpreter.CreatedCards, imageLoadingPath, infoLoadingPath, true);
        }
    }

    public void AddCardsToGame()
    {
        if (!PreLoadedCompiled) CompilePreLoaded();
        foreach (var card in cards)
        {
            if (card is LeaderCard leader)
            {
                Player.Leaders.Add(leader.Name, leader);
            }
            else if (card.FactionEnum is Faction.Fidel) CardsWarehouse.RebelCards.Add(card);
            else CardsWarehouse.BatistaCards.Add(card);

            cardsAdded.Add(card.Name);
        }
        UpdatePreLoadedScripts(preLoadedEffectsDisplay, "Effects");
        UpdatePreLoadedScripts(preLoadedCardsDisplay, "Cards");
        ResetCards();
    }

    #region Other buttons
    public void BackFromCardCreation()
    {
        AddCardsToGame();
        if (interpreter is null || interpreter.ValidLoad)
        {
            mainMenu.SetActive(true);
            menu.SetActive(false);
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
    public void ChangeScriptPath()
    {
        if (scriptLoadingPathInput.text.Length > 0) scriptLoadingPath = scriptLoadingPathInput.text;
        else scriptLoadingPath = scriptLoadingPathInput.placeholder.GetComponent<TMP_Text>().text;
    }
    public void ChangeImagePath()
    {
        if (imageLoadingPathInput.text.Length > 0) imageLoadingPath = imageLoadingPathInput.text;
        else imageLoadingPath = imageLoadingPathInput.placeholder.GetComponent<TMP_Text>().text;
    }
    public void ChangeInfoPath()
    {
        if (infoLoadingPathInput.text.Length > 0) infoLoadingPath = infoLoadingPathInput.text;
        else infoLoadingPath = infoLoadingPathInput.placeholder.GetComponent<TMP_Text>().text;
    }

    public void DefaultDeck()
    {
        ResetCards();
        Awake();
    }

    public void DeleteFiles()
    {
        List<string> paths = new List<string>();
        paths.AddRange(Directory.GetFiles(mainPath +"\\Effects\\Scripts"));
        paths.AddRange(Directory.GetFiles(mainPath + "\\Cards\\Scripts"));
        paths.AddRange(Directory.GetFiles(mainPath + "\\Cards\\Main Image"));
        paths.AddRange(Directory.GetFiles(mainPath + "\\Cards\\Info"));

        foreach (var item in paths)
            File.Delete(item);

        UpdatePreLoadedScripts(preLoadedEffectsDisplay, "Effects");
        UpdatePreLoadedScripts(preLoadedCardsDisplay, "Cards");
    }
    #endregion

    #region Utils
    void ResetCards()
    {
        cards = new List<Card>();
        preLoadedCards = new List<string>();
        visualAssigner = new VisualAssigner(cardMaterial);
    }

    void UpdatePreLoadedScripts(TMP_Text text, string folder)
    {
        text.text = "";
        string[] paths = Directory.GetFiles(mainPath + "\\" + folder + "\\Scripts");
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

    void AddCards(List<Card> list, string imagePath, string infoPath, bool save)
    {
        foreach (var card in list)
        {
            this.visualAssigner.AssignVisual(card, imagePath, infoPath, save);
            cards.Add(card);
        }
    }
    #endregion
}
