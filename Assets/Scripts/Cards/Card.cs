using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Card : IEffect, ICardsWithOwner
{
    public string Name { get; }
    public Faction FactionEnum { get; }
    public CardType CardType { get; }
    public List<Zone> AvailableRange { get; }
    public List<Card> CurrentPosition { get; private set; }
    public VisualInfo Info { get; private set; }
    public string Description { get; private set; }
    protected Effect effect;
    protected double initialDamage;

    public double Power
    {
        get => this is UnitCard unit ? unit.DamageOnField : initialDamage;
        set
        {
            if (this is UnitCard unit) unit.ModifyOnFieldDamage(value);
        }
    }

    public string Faction => Utils.FactionName[FactionEnum];

    public Player Owner => Utils.GetPlayerByFaction(FactionEnum);

    public double InitialDamage { get => initialDamage;}

    public Card(string name, Faction faction, CardType cardType, List<Zone> availableRange, double damage = 0, Effect effect = null)
    {
        this.Name = name;
        this.FactionEnum = faction;
        this.CardType = cardType;
        this.AvailableRange = availableRange;
        AssignEffect(effect);
        initialDamage = damage;
    }

    public override bool Equals(object other)
    {
        return other is Card card && this.Name == card.Name 
                                  && this.CardType == card .CardType 
                                  && this.FactionEnum == card.FactionEnum 
                                  && (CurrentPosition is null? true : this.CurrentPosition.Equals(card.CurrentPosition));
    }

    public override int GetHashCode()
    {
        return HashCode.Combine<string, string, CardType>(Name, Faction, CardType);
    }

    public override string ToString()
    {
        return Name + " (" + Faction + ')';
    }

    public virtual bool Effect(Context context)
    {
        return effect is null ? true : effect.Invoke(context);
    }

    public void AssignPosition(List<Card> currentPosition) => this.CurrentPosition = currentPosition is null ? Owner.Hand : currentPosition;

    public void AssignInfo(VisualInfo info) => this.Info = info;

    public void AssignEffect(Effect effect) => this.effect = effect is null ? Effects.VoidEffect : effect;

    public void AssignDescription(string description)
    {
        if (description.Length > 200) description = description.Substring(0, 197) + "...";
        this.Description = description;
    }
}