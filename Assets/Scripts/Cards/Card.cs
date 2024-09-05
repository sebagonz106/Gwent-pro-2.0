using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Gwent_AI;

public class Card : ICard
{
    #region Fields
    public string Name { get; }
    public Faction FactionEnum { get; }
    public Type Type { get; }
    public List<Zone> AvailableRange { get; }
    public List<Card> CurrentPosition { get; private set; }
    public VisualInfo Info { get; private set; }
    public string Description { get; private set; }
    protected Effect effect;
    protected double initialDamage;
    #endregion

    #region Properties to use in compiler
    public double Power
    {
        get => this is UnitCard unit ? unit.DamageOnField : initialDamage;
        set
        {
            if (this is UnitCard unit && unit.Level is Level.Silver) unit.ModifyOnFieldDamage(value);
        }
    }

    public string Faction => Utils.FactionName[FactionEnum];

    public string CardType
    {
        get
        {
            switch (Type)
            {
                case Type.Unit: return ((UnitCard)this).Level is Level.Silver ? "Plata" : "Oro";

                case Type.Bonus: return "Aumento";
                    
                case Type.Leader: return "Lider";
                    
                case Type.Weather: return "Clima";

                case Type.Clear: return "Despeje";
                    
                case Type.Bait: return "Señuelo";
                    
                default: return "";
            }
        }
    }

    public Player Owner => Utils.GetPlayerByFaction(FactionEnum);
    public string OwnerName => Utils.GetPlayerByFaction(FactionEnum).Name;

    public void Effect(IContext context) => throw new NotImplementedException();
    public List<string> Zones => throw new NotImplementedException();

    public double InitialDamage { get => initialDamage;}
    #endregion

    #region Builder and object related methods
    public Card(string name, Faction faction, Type cardType, List<Zone> availableRange, double damage = 0, Effect effect = null)
    {
        this.Name = name;
        this.FactionEnum = faction;
        this.Type = cardType;
        this.AvailableRange = availableRange;
        AssignEffect(effect);
        initialDamage = damage;
    }

    public override bool Equals(object other)
    {
        return other is Card card && this.Name == card.Name 
                                  && this.Type == card .Type 
                                  && this.FactionEnum == card.FactionEnum 
                                  && (CurrentPosition is null? true : 
                                                               card.CurrentPosition is null? true : 
                                                                                             this.CurrentPosition.Equals(card.CurrentPosition));
    }

    public override int GetHashCode()
    {
        return HashCode.Combine<string, string, Type>(Name, Faction, Type);
    }

    public override string ToString()
    {
        return Name + " (" + Faction + ')';
    }
    #endregion

    public virtual bool Effect(Context context)
    {
        try
        {
            return effect is null ? true : effect.Invoke(context);
        }
        catch { return false; }
    }

    #region Assign methods
    public void AssignPosition(List<Card> currentPosition) => this.CurrentPosition = currentPosition is null ? Owner.Hand : currentPosition;

    public void AssignInfo(VisualInfo info) => this.Info = info;

    public void AssignEffect(Effect effect) => this.effect = effect is null ? Effects.VoidEffect : effect;

    public void AssignDescription(string description)
    {
        if (description.Length > 200) description = description.Substring(0, 197) + "...";
        this.Description = description;
    }
    #endregion
}