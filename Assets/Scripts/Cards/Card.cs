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
    protected Effect effect;
    protected double initialDamage;

    public int Power
    {
        get => this is UnitCard unit ? Convert.ToInt32(unit.DamageOnField) : Convert.ToInt32(initialDamage);
        set
        {
            if (this is UnitCard unit) unit.ModifyOnFieldDamage(value);
        }
    }

    public string Faction => Utils.FactionName[FactionEnum];

    public Player Owner => Utils.PlayerByFaction[FactionEnum];

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
        return other is Card card && this.Name == card.Name;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode(); //combinar nombre y faccion
    }

    public virtual bool Effect(Context context)
    {
        try
        {
            return effect is null ? true : effect.Invoke(context);
        }
        catch (System.NullReferenceException)
        {
            return false;
        }
    }

    public void AssignPosition(List<Card> currentPosition) => this.CurrentPosition = currentPosition is null ? Owner.Hand : currentPosition;

    public void AssignInfo(VisualInfo info) => this.Info = info;

    public void AssignEffect(Effect effect) => this.effect = effect is null ? Effects.VoidEffect : effect;
}