using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualInfo
{
    public Sprite Main { get; private set; }
    public Material Material { get; private set; }
    public Sprite Information { get; private set; }

    public VisualInfo(Material material, Sprite information)
    {
        this.Material = material;
        this.Information = information;
    }

    public VisualInfo(Sprite main, Sprite information)
    {
        Main = main;
        Information = information;
    }
}

public interface IEffect
{
    bool Effect(Context context);
}

public interface ICardsWithOwner
{
    Player Owner { get; }
}
