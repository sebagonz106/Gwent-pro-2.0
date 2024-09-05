using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualInfo
{
    public Sprite Main { get; private set; }
    public Material Material { get; private set; }
    public Sprite Information { get; private set; }

    public VisualInfo(Material material, Sprite information, string faction)
    {
        if (faction != "base" && (material is null || information is null)) this.Receive(GetRandomInfo(faction));
        else
        {
            this.Material = material;
            this.Information = information;
        }
    }

    public VisualInfo(Sprite main, Sprite information, string faction)
    {
        if (main is null || information is null) this.Receive(GetRandomInfo(faction));
        else
        {
            Main = main;
            Information = information;
        }
    }

    public VisualInfo(Sprite main, string faction)
    {
        if (main is null) this.Receive(GetRandomInfo(faction));
        else
        {
            Main = Information = main;
        }
    }

    static VisualInfo GetRandomInfo(string faction) => new VisualInfo(Resources.Load<Sprite>("Random/" + faction + "/" + Random.Range(1, 17)), faction);
    //17 is the amount of random pictures selected. in case of adding pictures, this number must be changed.

    void Receive(VisualInfo visualInfo)
    {
        this.Material = visualInfo.Material;
        this.Main = visualInfo.Main;
        this.Information = visualInfo.Information;
    }
}

public interface IEffect
{
    bool Effect(Context context);
    void Effect(Gwent_AI.IContext context);
}

public interface ICardsWithOwner
{
    Player Owner { get; }
    string OwnerName { get; }
}

public interface ICard : IEffect, ICardsWithOwner
{
    string CardType { get;}
    string Name { get;}
    string Faction { get; }
    double Power { get; }
    List<string> Zones { get; }
}
