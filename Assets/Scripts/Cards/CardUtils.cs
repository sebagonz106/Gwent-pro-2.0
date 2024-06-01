using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualInfo
{
    public Material Material { get; private set; }
    public Sprite Information { get; private set; }

    public VisualInfo(Material material, Sprite information)
    {
        this.Material = material;
        this.Information = information;
    }
    public VisualInfo(CardSO card)
    {
        this.Material = card.material;
        this.Information = card.information;
    }
}

public interface IEffect
{
    bool Effect(Context context);
}

public interface ICardsPlayableInCommonPositions
{
    Player PlayerThatPlayedThisCard { get; set; }
}
