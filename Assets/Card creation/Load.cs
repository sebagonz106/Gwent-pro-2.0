using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Gwent_Interpreter;
using System.IO;

public class Load : MonoBehaviour
{
    [SerializeField] TMP_Text terminal;
    List<Card> cards = new List<Card>();

    public void Compile()
    {
        string[] paths = Directory.GetFiles("D:\\Gwent-Pro\\Gwent pro v2.0\\Assets\\Card creation\\Interpreter\\Files\\Scripts to compile");
        Interptreter interpreter = new Interptreter(new Printer(terminal));
        foreach (var path in paths)
            if (path.Substring(path.Length - 5) == ".meta") continue;
            else if (interpreter.Evaluate(File.ReadAllText(path))) cards.AddRange(interpreter.CreatedCards);
    }

    public void ResetCards()
    {
        cards = new List<Card>();
    }
}
